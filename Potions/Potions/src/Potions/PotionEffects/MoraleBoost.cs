using Potions.APIs;

namespace Potions.PotionEffects;

public class Potion_MoraleBoost: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        MoraleBoost.SpawnMoraleBoost(character.Center, 3000, 255, 255, sendToAll: true);
    }

    public override void Apply(Item item)
    {
    }
}