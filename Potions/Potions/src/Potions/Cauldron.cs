using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Potions.APIs;
using Potions.Juicing;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potions;

public class Cauldron: MonoBehaviour, IInteractibleConstant
{
    public bool ignited;
    private bool dead;
    private PhotonView photonView;
    private int storedItems;
    private const int maxStoredItems = 2;
    private const float igniteDurationSeconds = 180f;
    private Transform itemsRoot;
    private List<string> itemsStored = [];
    private Bubbles bubbles;
    private float timeRemaining = 0;
    private double igniteEndTimestamp;
    private AudioSource audioSource;
    private readonly List<ParticleSystem> enableWhenLitParticles = new();
    private readonly List<Transform> itemSlots = new();
    private Coroutine brewRoutine;

    private void Awake()
    {
        bubbles = transform.Find("Cylinder").Find("Bubbles").GetComponent<Bubbles>();
        itemsRoot = transform.Find("Items");
        photonView = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
        if (itemsRoot != null)
        {
            foreach (Transform slot in itemsRoot)
            {
                itemSlots.Add(slot);
            }
        }
        foreach (Transform particleSysTf in transform.Find("EnableWhenLit"))
        {
            var particleSys = particleSysTf.GetComponent<ParticleSystem>();
            if (particleSys == null) continue;
            enableWhenLitParticles.Add(particleSys);
            particleSys.Stop();
        }
    }

    private void Update()
    {
        if (!ignited) return;

        var remaining = igniteEndTimestamp - GetNetworkTime();
        timeRemaining = Mathf.Max(0f, (float)remaining);
        if (timeRemaining > 0f) return;

        if (HasNetworkAuthority)
        {
            HandleFireExpired();
            if (HasNetwork)
            {
                photonView.RPC("RPC_HandleFireExpired", RpcTarget.Others);
            }
        }
    }

    public bool IsInteractible(Character interactor)
    {
        if (dead) return false;
        if (storedItems >= maxStoredItems) return true;
        return ignited switch
        {
            true => interactor.data.currentItem != null,
            false => true
        };
    }

    public void Interact(Character interactor) {}

    public void HoverEnter() {}
    
    public void HoverExit() {}

    public Vector3 Center()
    {
        return GetComponent<MeshRenderer>().bounds.center;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public string GetInteractionText()
    {
        if (storedItems == maxStoredItems) return "boil";
        switch (ignited)
        {
            case true:
                return "add";
            case false:
                return "light";
        }
    }

    public string GetName()
    {
        return "CAULDRON";
    }

    public bool IsConstantlyInteractable(Character interactor)
    {
        return IsInteractible(interactor);
    }

    public float GetInteractTime(Character interactor)
    {
        return ignited switch
        {
            true => 1f,
            false => 3f
        };
    }

    public void Interact_CastFinished(Character interactor)
    {
        if (storedItems == maxStoredItems)
        {
            if (HasNetwork)
            {
                photonView.RPC("RPC_RequestBrew", RpcTarget.MasterClient, GetViewId(interactor));
            }
            else
            {
                HandleBrewOffline(interactor);
            }
            return;
        }

        if (ignited)
        {
            var held = interactor.data.currentItem;
            if (held == null) return;

            var heldView = held.GetComponent<PhotonView>();
            if (HasNetwork)
            {
                if (heldView == null)
                {
                    Plugin.Log.LogError("Held item missing PhotonView; cannot add to cauldron across network.");
                    return;
                }

                held.gameObject.SetActive(false);
                photonView.RPC("RPC_RequestAddItem", RpcTarget.MasterClient, heldView.ViewID, held.UIData.itemName);
            }
            else
            {
                HandleAddItemOffline(held, held.UIData.itemName);
            }

            interactor.player.EmptySlot(interactor.refs.items.currentSelectedSlot);
            return;
        }

        if (HasNetwork)
        {
            photonView.RPC("RPC_RequestIgnite", RpcTarget.MasterClient, GetViewId(interactor));
        }
        else
        {
            var endTimestamp = GetNetworkTime() + igniteDurationSeconds;
            ApplyIgnitedState(endTimestamp);
        }
    }

    private bool HasNetwork => PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null;

    private bool HasNetworkAuthority => !HasNetwork || PhotonNetwork.IsMasterClient;

    private static int GetViewId(Character character)
    {
        if (character == null) return -1;
        var view = character.GetComponent<PhotonView>();
        return view != null ? view.ViewID : -1;
    }

    private double GetNetworkTime()
    {
        return HasNetwork ? PhotonNetwork.Time : Time.time;
    }

    private int ChooseItemSlotIndex()
    {
        if (itemSlots.Count > 0)
        {
            return Random.Range(0, itemSlots.Count);
        }

        if (itemsRoot == null || itemsRoot.childCount == 0) return 0;
        return Random.Range(0, itemsRoot.childCount);
    }

    private void ApplyIgnitedState(double endTimestamp)
    {
        ignited = true;
        dead = false;
        igniteEndTimestamp = endTimestamp;
        timeRemaining = Mathf.Max(0f, (float)(igniteEndTimestamp - GetNetworkTime()));

        foreach (var particleSys in enableWhenLitParticles)
        {
            particleSys.Play();
        }

        if (bubbles != null)
        {
            bubbles.enabled = true;
            bubbles.__enabled = true;
        }

        audioSource?.Play();
        Plugin.FireStarted.Play(transform.position);
    }

    private void HandleFireExpired()
    {
        StopIgniteVisuals(true);
    }

    private void StopIgniteVisuals(bool markDead)
    {
        ignited = false;
        dead = markDead;
        igniteEndTimestamp = 0;
        timeRemaining = 0;

        foreach (var particleSys in enableWhenLitParticles)
        {
            particleSys.Stop();
        }

        if (bubbles != null)
        {
            bubbles.__enabled = false;
        }

        audioSource?.Stop();
    }

    private void HandleAddItemOffline(Item held, string itemName)
    {
        var rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        var slotIndex = ChooseItemSlotIndex();
        HandleAddItemConfirmed(-1, itemName, slotIndex, rotation, 3f, held);
        if (held != null)
        {
            Destroy(held.gameObject);
        }
    }

    private void HandleBrewOffline(Character interactor)
    {
        if (itemsStored.Count < maxStoredItems) return;

        StartCoroutine(DunkItemCoroutine(0.2f));
        if (brewRoutine != null)
        {
            StopCoroutine(brewRoutine);
        }

        brewRoutine = StartCoroutine(SpawnPotionAfterDelay(GetViewId(interactor)));
    }

    private void HandleAddItemConfirmed(int sourceItemViewId, string itemName, int slotIndex, Vector3 rotation, float scaleMultiplier, Item blueprint = null)
    {
        if (itemsStored.Count >= maxStoredItems)
        {
            Plugin.Log.LogWarning("Cauldron storage full while handling item addition; ignoring duplicate request.");
            return;
        }

        itemsStored.Add(itemName);
        storedItems = itemsStored.Count;

        AttachStoredItemVisual(sourceItemViewId, slotIndex, rotation, scaleMultiplier, blueprint, itemName);
    }

    private void AttachStoredItemVisual(int sourceItemViewId, int slotIndex, Vector3 rotation, float scaleMultiplier, Item blueprint, string itemName)
    {
        if (itemsRoot == null) return;

        Transform parent = itemsRoot;
        if (itemSlots.Count > 0)
        {
            slotIndex = Mathf.Clamp(slotIndex, 0, itemSlots.Count - 1);
            parent = itemSlots[slotIndex] != null ? itemSlots[slotIndex] : itemsRoot;
        }
        else if (itemsRoot.childCount > 0)
        {
            slotIndex = Mathf.Clamp(slotIndex, 0, itemsRoot.childCount - 1);
            parent = itemsRoot.GetChild(slotIndex);
        }

        Item sourceItem = blueprint != null ? blueprint : TryResolveItemFromViewId(sourceItemViewId);
        GameObject visualGo;
        if (sourceItem != null)
        {
            var dupe = Instantiate(sourceItem);
            visualGo = dupe.gameObject;
            PrepareItemVisual(dupe);
            if (!visualGo.activeSelf)
            {
                visualGo.SetActive(true);
            }
        }
        else
        {
            Plugin.Log.LogWarning($"Unable to resolve source item view {sourceItemViewId}; spawning placeholder for {itemName}.");
            visualGo = CreateFallbackVisual(parent, itemName);
        }

        var visualTransform = visualGo.transform;
        visualTransform.SetParent(parent, false);
        visualTransform.localPosition = Vector3.zero;
        visualTransform.localRotation = Quaternion.Euler(rotation);
        visualTransform.localScale *= scaleMultiplier;
    }

    private Item TryResolveItemFromViewId(int viewId)
    {
        if (viewId <= 0) return null;
        var view = PhotonView.Find(viewId);
        return view != null ? view.GetComponent<Item>() : null;
    }

    private void PrepareItemVisual(Item dupe)
    {
        if (dupe == null) return;

        var dupePhotonView = dupe.GetComponent<PhotonView>();
        if (dupePhotonView != null)
        {
            Destroy(dupePhotonView);
        }

        var dupeRigidbody = dupe.GetComponent<Rigidbody>();
        if (dupeRigidbody != null)
        {
            Destroy(dupeRigidbody);
        }

        foreach (var itemAction in dupe.GetComponents<ItemAction>())
        {
            Destroy(itemAction);
        }

        foreach (var itemComponent in dupe.GetComponents<ItemComponent>())
        {
            Destroy(itemComponent);
        }

        foreach (var collider in dupe.GetComponentsInChildren<Collider>())
        {
            Destroy(collider);
        }
    }

    private GameObject CreateFallbackVisual(Transform parent, string itemName)
    {
        var placeholder = new GameObject(string.IsNullOrWhiteSpace(itemName) ? "StoredItem" : $"StoredItem_{itemName}");
        placeholder.transform.SetParent(parent, false);
        return placeholder;
    }

    private void ClearStoredItemVisuals()
    {
        if (itemsRoot == null) return;

        if (itemSlots.Count > 0)
        {
            foreach (var slot in itemSlots)
            {
                if (slot == null) continue;
                var toDestroy = new List<GameObject>();
                foreach (Transform subchild in slot)
                {
                    toDestroy.Add(subchild.gameObject);
                }

                foreach (var go in toDestroy)
                {
                    Destroy(go);
                }
            }
            return;
        }

        var rootChildren = new List<GameObject>();
        foreach (Transform child in itemsRoot)
        {
            rootChildren.Add(child.gameObject);
        }

        foreach (var go in rootChildren)
        {
            Destroy(go);
        }
    }

    private Character ResolveCharacter(int viewId)
    {
        if (viewId <= 0) return null;
        var view = PhotonView.Find(viewId);
        return view != null ? view.GetComponent<Character>() : null;
    }

    [PunRPC]
    private void RPC_RequestIgnite(int interactorViewId, PhotonMessageInfo info)
    {
        if (!HasNetworkAuthority) return;
        if (ignited || dead) return;

        var baseTime = HasNetwork ? PhotonNetwork.Time : GetNetworkTime();
        var endTimestamp = baseTime + igniteDurationSeconds;
        ApplyIgnitedState(endTimestamp);

        if (HasNetwork)
        {
            photonView.RPC("RPC_HandleIgnited", RpcTarget.Others, endTimestamp);
        }
    }

    [PunRPC]
    private void RPC_HandleIgnited(double endTimestamp)
    {
        ApplyIgnitedState(endTimestamp);
    }

    [PunRPC]
    private void RPC_HandleFireExpired()
    {
        HandleFireExpired();
    }

    [PunRPC]
    private void RPC_RequestAddItem(int itemViewId, string itemName, PhotonMessageInfo info)
    {
        if (!HasNetworkAuthority)
        {
            return;
        }

        if (itemsStored.Count >= maxStoredItems)
        {
            photonView.RPC("RPC_AddItemRejected", info.Sender, itemViewId);
            return;
        }

        var slotIndex = ChooseItemSlotIndex();
        var rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        const float scaleMultiplier = 3f;

        if (HasNetwork)
        {
            photonView.RPC("RPC_AddItemConfirmed", RpcTarget.All, itemViewId, itemName, slotIndex, rotation.x, rotation.y, rotation.z, scaleMultiplier);

            var sourceView = PhotonView.Find(itemViewId);
            var owner = sourceView?.Owner;
            if (owner != null)
            {
                photonView.RPC("RPC_DestroyItemOnOwner", owner, itemViewId);
            }
            else if (sourceView != null)
            {
                RPC_DestroyItemOnOwner(itemViewId);
            }
        }
        else
        {
            HandleAddItemConfirmed(itemViewId, itemName, slotIndex, rotation, scaleMultiplier);
            RPC_DestroyItemOnOwner(itemViewId);
        }
    }

    [PunRPC]
    private void RPC_AddItemConfirmed(int sourceItemViewId, string itemName, int slotIndex, float rotX, float rotY, float rotZ, float scaleMultiplier)
    {
        HandleAddItemConfirmed(sourceItemViewId, itemName, slotIndex, new Vector3(rotX, rotY, rotZ), scaleMultiplier);
    }

    [PunRPC]
    private void RPC_AddItemRejected(int itemViewId)
    {
        var view = PhotonView.Find(itemViewId);
        if (view == null) return;
        var item = view.GetComponent<Item>();
        if (item != null)
        {
            Plugin.Log.LogWarning($"Add item request rejected; restoring item {item.UIData.itemName}.");
            item.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void RPC_DestroyItemOnOwner(int itemViewId)
    {
        var view = PhotonView.Find(itemViewId);
        if (view == null) return;

        var go = view.gameObject;
        if (go == null) return;

        if (HasNetwork)
        {
            PhotonNetwork.Destroy(go);
        }
        else
        {
            Destroy(go);
        }
    }

    [PunRPC]
    private void RPC_RequestBrew(int interactorViewId, PhotonMessageInfo info)
    {
        if (!HasNetworkAuthority) return;
        if (itemsStored.Count < maxStoredItems) return;

        if (HasNetwork)
        {
            photonView.RPC("RPC_DunkItems", RpcTarget.All);
        }
        else
        {
            StartCoroutine(DunkItemCoroutine(0.2f));
        }

        if (brewRoutine != null)
        {
            StopCoroutine(brewRoutine);
        }

        brewRoutine = StartCoroutine(SpawnPotionAfterDelay(interactorViewId));
    }

    private IEnumerator SpawnPotionAfterDelay(int interactorViewId)
    {
        Plugin.Log.LogWarning("Asking host to spawn potion...");
        yield return new WaitForSeconds(0.7f);
        SpawnPotion(interactorViewId);
        Plugin.Log.LogWarning("Asked!");
        brewRoutine = null;
    }

    private void SpawnPotion(int interactorViewId)
    {
        if (itemsStored.Count < maxStoredItems)
        {
            Plugin.Log.LogWarning("Potion spawn requested without enough ingredients.");
            return;
        }

        Plugin.Log.LogWarning("Got potion request...");
        var pot = PotionAPI.GetRecipe(itemsStored[0], itemsStored[1]);
        var potion = PotionAPI.CreatePotion(pot);
        potion.transform.SetParent(null);
        potion.transform.position = transform.position + Vector3.up * 2;

        var interactor = ResolveCharacter(interactorViewId);
        if (interactor != null)
        {
            potion.Interact(interactor);
        }
        else if (interactorViewId > 0)
        {
            Plugin.Log.LogWarning($"Failed to resolve interactor view {interactorViewId} for potion delivery.");
        }

        if (HasNetwork)
        {
            photonView.RPC("RPC_ResetStoredItems", RpcTarget.All);
        }
        else
        {
            RPC_ResetStoredItems();
        }

        Plugin.Log.LogWarning("Spawned potion!");
    }

    [PunRPC]
    private void RPC_ResetStoredItems()
    {
        ClearStoredItemVisuals();
        itemsStored.Clear();
        storedItems = 0;
    }

    public void CancelCast(Character interactor)
    {
    }

    public void ReleaseInteract(Character interactor)
    {
    }

    public bool holdOnFinish { get; }

    [PunRPC]
    private void RPC_DunkItems()
    {
        StartCoroutine(DunkItemCoroutine(0.2f));
    }

    private IEnumerator DunkItemCoroutine(float amnt)
    {
        Plugin.Log.LogWarning("Dunking item...");
        var i = 0f;
        while (i < 1)
        {
            yield return new WaitForEndOfFrame();
            i += Time.deltaTime;
            itemsRoot.position -= Vector3.up * (Time.deltaTime * amnt);
            itemsRoot.localScale -= Vector3.one * (Time.deltaTime * amnt * 0.01f);
        }

        if (!(amnt > 0))
        {
            Plugin.Log.LogWarning("Resetting cauldron visuals.");
            ClearStoredItemVisuals();
            yield break;
        }
        ClearStoredItemVisuals();
        Plugin.Log.LogWarning("Despawning items");
        StartCoroutine(DunkItemCoroutine(-amnt));
    }
}
