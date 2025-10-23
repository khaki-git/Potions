using PEAKLib.Core;
using Photon.Pun;
using Potions.APIs;
using Potions.Patches;
using UnityEngine;

namespace Potions.PotionEffects;

public class Ducky: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        for (var i = 0; i < Random.Range(10, 20); i++)
        {
            var d = NetworkPrefabManager.SpawnNetworkPrefab("Rubberducky", character.Center, Quaternion.identity);
            d.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * Random.Range(15,50), ForceMode.Impulse);
        }
        character.DieInstantlyWithoutSkeleton();
    }

    public override void Apply(Item item)
    {
    }
}