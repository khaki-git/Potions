using Peak.Afflictions;
using Potions.APIs;
using Potions.CustomAfflictions;

namespace Potions.PotionEffects;

public class Pinata(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character, Item item)
    {
    }

    public override void Apply(Item item)
    {
        var infStamina = item.gameObject.AddComponent<Action_ApplyAffliction>();
        infStamina.OnCastFinished = true;
        var fasterAffliction = new Affliction_Pinata
        {
            totalTime = len
        };
        infStamina.affliction = fasterAffliction;
    }
}