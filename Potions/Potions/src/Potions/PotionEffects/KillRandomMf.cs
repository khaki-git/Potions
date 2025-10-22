using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Photon.Pun;
using Potions.APIs;
using Vector3 = UnityEngine.Vector3;

namespace Potions.PotionEffects;

public class KillRandomMf: PotionEffect
{
    public override void Drink(Character character, Item _item)
    {
        var list = Character.AllCharacters.Where(allCharacter => !allCharacter.data.dead && !allCharacter.data.fullyPassedOut && allCharacter != character).ToList();

        foreach (var item in list)
        {
            item.photonView.RPC("RPCA_Die", RpcTarget.All, character.Center + Vector3.up * 3, false);
        }
    }

    public override void Apply(Item item)
    {
    }
}