using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Potions;

public class Cauldron: MonoBehaviour, IInteractibleConstant
{
    public bool ignited;
    private PhotonView photonView;
    private int storedItems;
    private const int maxStoredItems = 2;
    private Transform itemsRoot;
    private List<string> itemsStored = [];

    private void Awake()
    {
        itemsRoot = transform.Find("Items");
        photonView = GetComponent<PhotonView>();
    }

    public bool IsInteractible(Character interactor)
    {
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
            // TODO: spawn potion
            
            // destroy stored items and make a potion
            foreach (var child in itemsRoot.GetComponentsInChildren<Transform>())
            {
                foreach (var subchild in child.GetComponentsInChildren<Transform>())
                {
                    Destroy(subchild.gameObject);
                }
            }

            // reset the mf
            itemsStored = [];
            return;
        }
        switch (ignited)
        {
            case true:
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
                itemsStored.Add(held.name);
                break;
            case false:
                ignited = true;
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
}