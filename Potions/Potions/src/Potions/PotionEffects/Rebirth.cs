using Potions.APIs;
using Potions.CustomAfflictions;

namespace Potions.PotionEffects;

public class Rebirth(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character, Item item)
    {
    }

    public override void Apply(Item item)
    {
        var temperate = item.gameObject.AddComponent<Action_ApplyAffliction>();
        temperate.OnCastFinished = true;
        var temperateAffliction = new Affliction_Rebirth
        {
            totalTime = len
        };
        temperate.affliction = temperateAffliction;
    }
}