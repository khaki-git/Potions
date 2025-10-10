using System.Collections.Generic;

namespace Potions.APIs;

public class Potion
{
    public string name;
    public string id;
    public List<PotionEffect> effects;
    public float drinkTime = 1f;
}