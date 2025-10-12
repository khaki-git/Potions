using Peak.Afflictions;
using Potions.APIs;

namespace Potions.PotionEffects;

public class Poisoned(float damage) : PotionEffect
{
    private float dmg = damage;

    public override void Drink(Character character)
    {
    }

    public override void Apply(Item item)
    {
        var poisonModifier = item.gameObject.AddComponent<Action_InflictPoison>();
        poisonModifier.OnCastFinished = true;
        poisonModifier.delay = 10f;
        poisonModifier.inflictionTime = 40f;
        poisonModifier.poisonPerSecond = dmg / 40f / 100f;
    }
}