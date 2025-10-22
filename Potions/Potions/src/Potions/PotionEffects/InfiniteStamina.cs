using Potions.APIs;

namespace Potions.PotionEffects;

public class InfiniteStamina(float length) : PotionEffect
{
    private float len = length;

    public override void Drink(Character character, Item item)
    {
    }

    public override void Apply(Item item)
    {
        var infStamina = item.gameObject.AddComponent<Action_ApplyInfiniteStamina>();
        infStamina.OnCastFinished = true;
        infStamina.drowsyAmount = 0f;
        infStamina.buffTime = len;
    }
}