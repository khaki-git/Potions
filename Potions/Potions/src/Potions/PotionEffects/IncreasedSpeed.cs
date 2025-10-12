using Peak.Afflictions;
using Potions.APIs;

namespace Potions.PotionEffects;

public class IncreasedSpeed(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character)
    {
    }

    public override void Apply(Item item)
    {
        var infStamina = item.gameObject.AddComponent<Action_ApplyAffliction>();
        infStamina.OnCastFinished = true;
        var fasterAffliction = new Affliction_FasterBoi
        {
            drowsyOnEnd = 0f,
            totalTime = len
        };
        infStamina.affliction = fasterAffliction;
    }
}