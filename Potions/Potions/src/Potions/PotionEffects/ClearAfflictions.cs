using Potions.APIs;

namespace Potions.PotionEffects;

public class ClearAfflictions: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        foreach (var aff in character.refs.afflictions.afflictionList)
        {
            character.refs.afflictions.RemoveAffliction(aff.GetAfflictionType());
        }
    }

    public override void Apply(Item item)
    {
    }
}