namespace Potions.APIs;

public abstract class PotionEffect
{
    public abstract void Drink(Character character, Item item);
    public abstract void Apply(Item item);
}