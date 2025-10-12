using System.Collections.Generic;
using Potions.APIs;
using Potions.PotionEffects;
using UnityEngine;

namespace Potions;

public static class BuiltinPotions
{
    private static Color GetSeededColor(string seed, float saturation)
    {
        var hash = seed.GetHashCode();
        var rng = new System.Random(hash);
        var hue = (float)rng.NextDouble();
        const float value = 1f;
        saturation = Mathf.Clamp01(saturation);
    
        return Color.HSVToRGB(hue, saturation, value);
    }
    
    internal static void CreatePotions()
    {
        var vilePotion = new Potion
        {
            name = "Vile Potion",
            drinkTime = 4.5f,
            id = "vile",
            effects =
            [
                new Poisoned(110)
            ],
            liquidColour = new Color(0.1f, 0.9f, 0.4f),
            recipe = ["X", "Y"] // default
        };
        PotionAPI.RegisterPotion(
                vilePotion
            );
        
        var ratPoison = new Potion
        {
            name = "Rat Poison",
            drinkTime = 0.5f,
            id = "rat_poison",
            effects =
            [
                new Die()
            ],
            liquidColour = new Color(0, 0.9f, 0),
            recipe = ["Vile Potion", "Green Crispberry"]
        };
        PotionAPI.RegisterPotion(
            ratPoison
        );
        
        var moralePotion = new Potion
        {
            name = "Potion of Happiness",
            drinkTime = 4.5f,
            id = "happiness",
            effects =
            [
                new Potion_MoraleBoost()
            ],
            liquidColour = new Color(1.000f, 0.922f, 0.102f),
            recipe = ["Berrynana Peel", "Yellow Crispberry"]
        };
        PotionAPI.RegisterPotion(
            moralePotion
        );
        
        var erraticPotion = new Potion
        {
            name = "Erratic Potion",
            drinkTime = 4.5f,
            id = "erratic",
            effects =
            [
                new InfiniteStamina(20),
                new IncreasedSpeed(20),
            ],
            liquidColour = new Color(1.000f, 0.149f, 0.863f, 1.000f),
            recipe = ["Big Lollipop", "Energy Drink"]
        };
        PotionAPI.RegisterPotion(
            erraticPotion
        );
        
        var temperatePotion = new Potion
        {
            name = "Temperate Potion",
            drinkTime = 4.5f,
            id = "temperate",
            effects =
            [
                new Temperate(120f)
            ],
            liquidColour = new Color(0.4f, 0.4f, 0.4f),
            recipe = ["Clusterberry!", "Cluster Shroom"]
        };
        PotionAPI.RegisterPotion(
            temperatePotion
        );
        
        var numbnessPotion = new Potion
        {
            name = "Numbness Potion",
            drinkTime = 4.5f,
            id = "numbness",
            effects =
            [
                new Numbness(120f)
            ],
            liquidColour = GetSeededColor("numbness", 0.4f),
            recipe = ["Clusterberry!", "Cluster Shroom"]
        };
        PotionAPI.RegisterPotion(
            numbnessPotion
        );
    }
}
