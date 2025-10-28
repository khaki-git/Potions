using Potions.APIs;

namespace Potions.PotionEffects;

public class ClearAfflictions: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        try
        {
            foreach (var aff in character.refs.afflictions.afflictionList)
            {
                try
                {
                    character.refs.afflictions.RemoveAffliction(aff.GetAfflictionType());
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch
        {
            // ignored
        }
    }

    public override void Apply(Item item)
    {
    }
}