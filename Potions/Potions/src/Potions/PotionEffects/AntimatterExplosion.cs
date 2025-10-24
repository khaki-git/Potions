using Photon.Pun;
using Potions.APIs;
using UnityEngine;

namespace Potions.PotionEffects;

public class AntimatterExplosion: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        ObjectSpawnerInterface.singleton.SpawnObject(
            new SpawnRequest("0_items/Dynamite", character.Center, Quaternion.identity),
            view =>
            {
                view.RPC("RPC_Explode", RpcTarget.All);
            });
    }

    public override void Apply(Item item)
    {
    }
}