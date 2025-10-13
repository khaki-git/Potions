using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Photon.Pun;
using Potions.APIs;
using Vector3 = UnityEngine.Vector3;

namespace Potions.PotionEffects;

public class ReviveTheHomies: PotionEffect
{
    public override void Drink(Character character)
    {
        var list = Character.AllCharacters.Where(allCharacter => allCharacter.data.dead || allCharacter.data.fullyPassedOut).ToList();

        foreach (var item in list)
        {
            item.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, character.transform.position + Vector3.up  * 3, false);
        }
    }

    public override void Apply(Item item)
    {
    }
}