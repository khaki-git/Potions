using Photon.Pun;
using Potions.APIs;
using UnityEngine;

namespace Potions.PotionEffects;

public class SpawnPrefab(string prefab, bool separate, int amount = 1): PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        for (var i = 0; i < amount; i++)
        {
            var loc = character.Center;
            if (separate)
            {
                // offset the ~~tumbleweed~~ prefab
                loc += new Vector3(Random.Range(-35, 35), 0f, Random.Range(-35, 35));
            }
            PhotonNetwork.Instantiate(prefab, loc, Quaternion.identity);
        }
    }

    public override void Apply(Item item)
    {
    }
}