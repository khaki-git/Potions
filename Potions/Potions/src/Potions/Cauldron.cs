using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Potions.APIs;
using Potions.Juicing;
using Sirenix.Utilities;
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
    private Transform itemsRoot;
    private List<string> itemsStored = [];
    private Bubbles bubbles;
    private float timeRemaining = 0;

    private void Awake()
    {
        bubbles = transform.Find("Cylinder").Find("Bubbles").GetComponent<Bubbles>();
        itemsRoot = transform.Find("Items");
        photonView = GetComponent<PhotonView>();
        foreach (Transform particleSysTf in transform.Find("EnableWhenLit"))
        {
            var particleSys = particleSysTf.GetComponent<ParticleSystem>();
            particleSys.Stop();
        }
    }

    void Update()
    {
        if (!ignited) return;
        timeRemaining -= Time.deltaTime;
        if (!(timeRemaining <= 0)) return;
        ignited = false;
        dead = true;
                
        foreach (Transform particleSysTf in transform.Find("EnableWhenLit"))
        {
            var particleSys = particleSysTf.GetComponent<ParticleSystem>();
            particleSys.Stop();
        }

        bubbles.__enabled = false;
        GetComponent<AudioSource>().Stop();
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
            // brew the potion
            photonView.RPC("RPC_DunkItems", RpcTarget.All);
            StartCoroutine(KindlyAskHostToSpawnPotion());
            return;
        }
        switch (ignited)
        {
            case true:
                Plugin.Log.LogWarning("Adding item...");
                var held = interactor.data.currentItem;
                
                // dupe factory
                held.gameObject.SetActive(false);
                var dupe = Instantiate(held);
                PhotonNetwork.AllocateViewID(dupe.GetComponent<PhotonView>());
                held.gameObject.SetActive(true);
                dupe.gameObject.SetActive(true);
                // done! new dupe!
                
                Destroy(dupe.GetComponent<Item>()); // get rid of all the gross weird bits
                Destroy(dupe.GetComponent<Rigidbody>());
                
                // delete all the actions
                foreach (var itemAction in dupe.GetComponents<ItemAction>())
                {
                    Destroy(itemAction);
                }
                foreach (var itemAction in dupe.GetComponents<ItemComponent>())
                {
                    Destroy(itemAction);
                }
                
                // parent the dupe
                
                var idx = Random.Range(0, itemsRoot.childCount);
                var itemSlot = itemsRoot.GetChild(idx);

                dupe.transform.SetParent(itemSlot, false);
                dupe.transform.localPosition = Vector3.zero;
                dupe.transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                dupe.transform.localScale *= 3f;
                
                // nuke it
                interactor.player.EmptySlot(interactor.refs.items.currentSelectedSlot);
                PhotonNetwork.Destroy(held.gameObject);

                storedItems++;
                itemsStored.Add(held.UIData.itemName);
                break;
            case false:
                Plugin.FireStarted.Play(transform.position);
                bubbles.enabled = true;
                bubbles.__enabled = true;
                ignited = true;
                foreach (Transform particleSysTf in transform.Find("EnableWhenLit"))
                {
                    var particleSys = particleSysTf.GetComponent<ParticleSystem>();
                    particleSys.Play();
                }
                GetComponent<AudioSource>().Play();

                timeRemaining = 180f;
                break;
        }
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

    [PunRPC]
    private void RPC_RequestPotion(Character interactor)
    {
        Plugin.Log.LogWarning("Got potion request...");
        var pot = PotionAPI.GetRecipe(itemsStored[0], itemsStored[1]);
        var potion = PotionAPI.CreatePotion(pot);
        potion.transform.SetParent(null);
        potion.transform.position = transform.position + Vector3.up * 2;
        potion.Interact(interactor);
        Plugin.Log.LogWarning("Spawned potion!");
    }
    
    private IEnumerator KindlyAskHostToSpawnPotion()
    {
        Plugin.Log.LogWarning("Asking host to spawn potion...");
        yield return new WaitForSeconds(0.7f);
        photonView.RPC("RPC_RequestPotion", RpcTarget.MasterClient, Character.localCharacter);
        Plugin.Log.LogWarning("Asked!");
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
            Plugin.Log.LogWarning("Reset everything.");
            itemsStored = [];
            storedItems = 0;
            yield break;
        }
        foreach (Transform child in itemsRoot)
        {
            foreach (Transform subchild in child)
            {
                if (subchild == child || subchild == itemsRoot) continue;
                Destroy(subchild.gameObject);
            }
        }
        Plugin.Log.LogWarning("Despawning items");
        StartCoroutine(DunkItemCoroutine(-amnt));
    }
}