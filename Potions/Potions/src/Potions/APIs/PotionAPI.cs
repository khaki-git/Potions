using System.Collections.Generic;
using UnityEngine;

namespace Potions.APIs;

public static class PotionAPI
{
    private static Dictionary<string, Potion> PotionRegistry = new();
        
    public static void RegisterPotion(Potion pot)
    {
        PotionRegistry[pot.id] = pot;
        
    }
}