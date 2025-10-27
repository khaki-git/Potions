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
            item.Invoke("DieInstantly", 0.02f);
        }
    }

    public override void Apply(Item item)
    {
    }
}