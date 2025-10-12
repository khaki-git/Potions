using Potions.APIs;

namespace Potions.PotionEffects;

public class Die: PotionEffect
{
    public override void Drink(Character character)
    {
        character.Invoke("DieInstantly", 0.02f);
    }

    public override void Apply(Item item)
    {
    }
}