using System;
using System.Collections.Generic;
using System.Linq;
using PEAKLib.Core;
using PEAKLib.Items;
using PEAKLib.Items.UnityEditor;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potions.APIs;

public static class PotionAPI
{
    private static Dictionary<string, Potion> PotionRegistry = new();
        
    public static void RegisterPotion(Potion pot)
    {
        LocalizedText.mainTable[$"NAME_{pot.name.ToUpper()}"] = [pot.name.ToUpper()];
        PotionRegistry[pot.id] = pot;
    }

    internal static Potion GetRecipe(string item1, string item2)
    {
        foreach (var potion in PotionRegistry.Values)
        {
            var r0 = potion.recipe[0];
            var r1 = potion.recipe[1];

            var r0Bang = r0.EndsWith("!");
            var r1Bang = r1.EndsWith("!");

            var r0Key = r0Bang ? r0[..^1] : r0;
            var r1Key = r1Bang ? r1[..^1] : r1;

            var directMatch =
                (r0Bang ? item1.Contains(r0Key, StringComparison.OrdinalIgnoreCase) : string.Equals(r0Key, item1, StringComparison.Ordinal)) &&
                (r1Bang ? item2.Contains(r1Key, StringComparison.OrdinalIgnoreCase) : string.Equals(r1Key, item2, StringComparison.Ordinal));

            var reverseMatch =
                (r0Bang ? item2.Contains(r0Key, StringComparison.OrdinalIgnoreCase) : string.Equals(r0Key, item2, StringComparison.Ordinal)) &&
                (r1Bang ? item1.Contains(r1Key, StringComparison.OrdinalIgnoreCase) : string.Equals(r1Key, item1, StringComparison.Ordinal));

            if (directMatch || reverseMatch)
                return potion;
        }

        return PotionRegistry["vile"];
    }

    internal static Potion FromId(string id)
    {
        return PotionRegistry[id];
    }

    internal static void CreatePotionRegisterables(UnityItemContent basePotion, ModDefinition mod)
    {
        foreach (var pot in PotionRegistry)
        {
            Debug.LogWarning("CREATED POTION!!!!");
            
            var potion = Object.Instantiate(basePotion.ItemPrefab);
            Object.DontDestroyOnLoad(potion); // ty hamunii that was very cool
            var potItem = potion.GetComponent<Item>();
            potItem.UIData.itemName = pot.Value.name;
            potItem.usingTimePrimary = pot.Value.drinkTime;
            
            var actPot = potion.AddComponent<Action_Potion>();
            actPot.potion = pot.Key;
            actPot.OnCastFinished = true;

            foreach (var fx in pot.Value.effects)
            {
                fx.Apply(potItem);
            }
            
            potion.name = pot.Key;
            var registerable = new ItemContent(potItem);

            var registered = registerable.Register(mod);
            Debug.Log($"Registered {registered.Content.Name} ({registered.Content.Item.UIData.itemName})");
        }
    }

    public static Item CreatePotion(Potion potion)
    {
        Debug.Log("Trying to create potion.");
        var prefabId = "com.khakixd.potions:" + potion.id;
        var pot = PhotonNetwork.InstantiateItemRoom(prefabId, Vector3.zero, Quaternion.identity);
        Debug.Log($"{pot.name} created");
        
        return pot.GetComponent<Item>();
    }
}