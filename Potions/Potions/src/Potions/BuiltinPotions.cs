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
            recipe = ["Shroom!", "Shroom!"]
        };
        PotionAPI.RegisterPotion(
            numbnessPotion
        );
        
        var selflessness = new Potion
        {
            name = "Potion of Selflessness",
            drinkTime = 4.5f,
            id = "selflessness",
            effects =
            [
                new ReviveTheHomies(),
                new Die()
            ],
            liquidColour = GetSeededColor("selflessness", 0.8f),
            recipe = ["Remedy Fungus", "Shelf Fungus"]
        };
        PotionAPI.RegisterPotion(
            selflessness
        );
        
        var impatience = new Potion
        {
            name = "Potion of Impatience",
            drinkTime = 4.5f,
            id = "impatience",
            effects =
            [
                new RemoveStatus(1, CharacterAfflictions.STATUSTYPE.Cold),
                new RemoveStatus(1, CharacterAfflictions.STATUSTYPE.Hot),
                new RemoveStatus(1, CharacterAfflictions.STATUSTYPE.Poison),
                new RemoveStatus(1, CharacterAfflictions.STATUSTYPE.Drowsy)
            ],
            liquidColour = GetSeededColor("impatience", 0.2f),
            recipe = ["Shroom!", "Crispberry!"]
        };
        PotionAPI.RegisterPotion(
            impatience
        );
        
        var lifesteal = new Potion // you're honestly a dick if you use this
        {
            name = "Potion of Lifesteal",
            drinkTime = 4.5f,
            id = "lifesteal",
            effects =
            [
                new Lifesteal(60f)
            ],
            liquidColour = GetSeededColor("lifesteal", 1f),
            recipe = ["Remedy Fungus", "Coconut!"]
        };
        PotionAPI.RegisterPotion(
            lifesteal
        );
        
        var levitation = new Potion
        {
            name = "Potion of Levitation",
            drinkTime = 4.5f,
            id = "levitation",
            effects =
            [
                new Levitation(60f)
            ],
            liquidColour = GetSeededColor("levitation", .401337f),
            recipe = ["Scout Cannon", "Coconut!"]
        };
        PotionAPI.RegisterPotion(
            levitation
        );
        
        var negativity = new Potion
        {
            name = "Potion of Negativity",
            drinkTime = 4.5f,
            id = "negativity",
            effects =
            [
                new ClearAfflictions()
            ],
            liquidColour = GetSeededColor("negativity", .401337f),
            recipe = ["Shroom!", "Remedy Fungus"]
        };
        PotionAPI.RegisterPotion(
            negativity
        );
        
        var anti = new Potion
        {
            name = "Antimatter Potion",
            drinkTime = 4.5f,
            id = "antimatter",
            effects =
            [
                new AntimatterExplosion(),
                new Die()
            ],
            liquidColour = new Color(0.2f, 0.2f, 1),
            recipe = ["Crispberry!", "anti-rope!"]
        };
        PotionAPI.RegisterPotion(
            anti
        );
        
        // Antimatter Potions
        
        var literallynothinglmfao = new Potion
        {
            name = "Potion?",
            drinkTime = 4.5f,
            id = "literallynothinglmfao",
            effects =
            [
                
            ],
            liquidColour = new Color(1,1,1),
            recipe = ["Antimatter Potion", "Antimatter Potion"]
        };
        PotionAPI.RegisterPotion(
            literallynothinglmfao
        );
        
        var positivity = new Potion
        {
            name = "Potion of Positivity",
            drinkTime = 4.5f,
            id = "positivity",
            effects =
            [
                new IncreasedSpeed(60),
                new InfiniteStamina(60),
                new Levitation(60),
                new Lifesteal(60),
                new Potion_MoraleBoost(),
                new Numbness(60),
                new Poisoned(50),
                new Rebirth(60),
                new ReviveTheHomies(),
                new Temperate(60),
                new AntimatterExplosion()
            ],
            liquidColour = GetSeededColor("positivity", .401337f),
            recipe = ["Potion of Negativity", "Antimatter Potion"]
        };
        PotionAPI.RegisterPotion(
            positivity
        );
        
        var selfishness = new Potion
        {
            name = "Potion of Selfishness",
            drinkTime = 4.5f,
            id = "selfishness",
            effects =
            [
                new ReviveTheHomies(),
                
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Cold),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Hot),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Hunger),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Thorns),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Injury),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Poison),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Drowsy),
                new RemoveStatus(255, CharacterAfflictions.STATUSTYPE.Curse),
                
                new KillRandomMf()
            ],
            liquidColour = GetSeededColor("selfishness", 0.8f),
            recipe = ["Remedy Fungus", "Shelf Fungus"]
        };
        PotionAPI.RegisterPotion(
            selfishness
        );
        
        var rebirthPoison = new Potion
        {
            name = "Potion of Rebirth",
            drinkTime = 0.5f,
            id = "Rebirth",
            effects =
            [
                new Rebirth(60)
            ],
            liquidColour = new Color(1, 1, .2f),
            recipe = ["Rat Poison", "Antimatter Potion"]
        };
        PotionAPI.RegisterPotion(
            rebirthPoison
        );
    }
}
