using Potions.APIs;
using Potions.CustomAfflictions;

namespace Potions.PotionEffects;

public class Numbness(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character)
    {
    }

    public override void Apply(Item item)
    {
        var temperate = item.gameObject.AddComponent<Action_ApplyAffliction>();
        temperate.OnCastFinished = true;
        var temperateAffliction = new Affliction_Numbness
        {
            totalTime = len
        };
        temperate.affliction = temperateAffliction;
    }
}