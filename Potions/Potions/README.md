# Potions
Adds potions to PEAK which can brewed using two ingredients at a Cauldron. This mod also contains a small API for developers to add their own potions.

### Recipes


| Name                   | Recipe                                    | Drink Time | Effects                                                                                                                                                                                                        |
| ---------------------- | ----------------------------------------- | ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Vile Potion            | Default Potion                            | 4.5s       | 110% Poison total beginning 10s after consuming                                                                                                                                                                |
| Rat Poison             | Vile Potion + Green Crispberry            | 0.5s       | Instantly kills the user                                                                                                                                                                                       |
| Potion of Happiness    | Any Berrynana Peel + Yellow Crispberry    | 4.5s       | Provides a Morale Boost to everyone (a whole bar of reserve stamina)                                                                                                                                           |
| Erratic Potion         | Big Lollipop + Energy Drink               | 4.5s       | Speed boost and infinite stamina for 20s (effects of Big Lollipop + Energy Drink)                                                                                                                              |
| Temperate Potion       | Any Clusterberry + Cluster Shroom         | 4.5s       | Temperate for 120s, removing 10% Cold and 10% Hot each second                                                                                                                                                  |
| Numbness Potion        | Any Mushroom + Any Mushroom               | 4.5s       | Removes all Injury for 120s, then returns the Injury afterwards                                                                                                                                                |
| Potion of Selflessness | Remedy Fungus + Shelf Fungus              | 4.5s       | Respawns all fallen teammates on the user, then kills the user                                                                                                                                                 |
| Potion of Impatience   | Any Shroom + Any Crispberry               | 4.5s       | Removes all of your Cold, Hot, Poison and Drowsy                                                                                                                                                               |
| Potion of Lifesteal    | Remedy Fungus + Any Coconut               | 4.5s       | Lifesteal for 60s                                                                                                                                                                                              |
| Potion of Levitation   | Scout Cannon + Any Coconut                | 4.5s       | Levitation for 60s                                                                                                                                                                                             |
| Potion of Negativity   | Any Shroom + Remedy Fungus                | 4.5s       | Clears all afflictions                                                                                                                                                                                         |
| Duck in a Bottle       | Napberry + Any Kingberry                  | 4.5s       | Kills the user and spawns a bunch of Rubber Duckies                                                                                                                                                            |
| Potion of Employment   | Ancient Idol + Potion of Happiness        | 4.5s       | Closes the users game                                                                                                                                                                                          |
| Sandstorm in a Bottle  | Any Prickleberry + Rope Spool             | 4.5s       | Summons a Tornado                                                                                                                                                                                              |
| Antimatter Potion      | Any Crispberry + Any Anti-Rope            | 4.5s       | Triggers an antimatter explosion and kills the user                                                                                                                                                            |
| Potion?                | Antimatter Potion + Antimatter Potion     | 4.5s       | Literally nothing                                                                                                                                                                                              |
| Potion of Positivity   | Potion of Negativity + Antimatter Potion  | 4.5s       | Massive mixed bag for 60s: Speed, Infinite Stamina, Levitation, Lifesteal, Morale Boost, Numbness, Temperate; also applies 50% Poison, triggers an antimatter explosion, grants Rebirth, and revives teammates |
| Potion of Selfishness  | Remedy Fungus + Shelf Fungus              | 4.5s       | Kills all other teammates, clears virtually all statuses (Cold/Hot/Hunger/Thorns/Injury/Poison/Drowsy/Curse)                                                                                                   |
| Potion of Rebirth      | Rat Poison + Antimatter Potion            | 0.5s       | Rebirth for 60s (revive on death)                                                                                                                                                                              |
| Tumbleweed in a Bottle | Sandstorm in a Bottle + Antimatter Potion | 4.5s       | Spawns 5 Tumbleweeds                                                                                                                                                                                           |



### For developers: Making custom potion recipes
To create a custom potion: Create an instance of the Potions.APIs.Potion class and register it using Potions.APIs.PotionAPI.RegisterPotion
```csharp
using Potions.APIs;
using BepInEx;

namespace MyPotionPack;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
  private void Awake()
  {
    var myPotion = new Potion
    {
        name = "Speedy Potion",
        drinkTime = 4.5f, // default drink time for most built-in potions
        id = "speedy",
        effects = [
            new IncreasedSpeed(120f),
        ],
        liquidColour = new Color(1, 0, 0), // red
        recipe = ["Energy Drink", "Red Crispberry"] // the UIData.name of the item
    }
    PotionAPI.RegisterPotion(myPotion);
  }
}
```
To make a potion effect, just inherit the PotionEffect class
```csharp
using Peak.Afflictions;  
using Potions.APIs;  
  
namespace Potions.PotionEffects;  
  
public class IncreasedSpeed(float length) : PotionEffect  
{  
    private float len = length;  
  
    public override void Drink(Character character)  
    {    }  
    public override void Apply(Item item)  
    {        var infStamina = item.gameObject.AddComponent<Action_ApplyAffliction>();  
        infStamina.OnCastFinished = true;  
        var fasterAffliction = new Affliction_FasterBoi  
        {  
            drowsyOnEnd = 0f,  
            totalTime = len  
        };  
        infStamina.affliction = fasterAffliction;  
    }  
}
```
