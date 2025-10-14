using Potions.APIs;

namespace Potions.PotionEffects;

public class RemoveStatus(float amount, CharacterAfflictions.STATUSTYPE statustype): PotionEffect
{
    public override void Drink(Character character)
    {
        character.refs.afflictions.SubtractStatus(statustype, amount);
    }

    public override void Apply(Item item)
    {
    }
}