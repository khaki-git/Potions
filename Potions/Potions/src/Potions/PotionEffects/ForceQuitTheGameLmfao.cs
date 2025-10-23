using Potions.APIs;
using Potions.Patches;

namespace Potions.PotionEffects;

public class ForceQuitTheGameLmfao : PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        character.SeekEmployment();
    }

    public override void Apply(Item item)
    {
    }
}