using Potions.APIs;
using Potions.CustomAfflictions;

namespace Potions.PotionEffects;

public class Temperate(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character)
    {
    }

    public override void Apply(Item item)
    {
        var temperate = item.gameObject.AddComponent<Action_ApplyAffliction>();
        temperate.OnCastFinished = true;
        var temperateAffliction = new Affliction_Temperate(0.1f, len);
        temperate.affliction = temperateAffliction;
    }
}