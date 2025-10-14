using Peak.Afflictions;
using Potions.APIs;
using Potions.CustomAfflictions;

namespace Potions.PotionEffects;

public class Lifesteal(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character)
    {
    }

    public override void Apply(Item item)
    {
        var infStamina = item.gameObject.AddComponent<Action_ApplyAffliction>();
        infStamina.OnCastFinished = true;
        var fasterAffliction = new Affliction_Lifesteal
        {
            totalTime = len
        };
        infStamina.affliction = fasterAffliction;
    }
}