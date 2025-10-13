# Potions
Adds potions to PEAK which can brewed using two ingredients at a Cauldron. This mod also contains a small API for developers to add their own potions.

### Recipes

| Name                   | Recipe                                 | Drink Time | Effects                                                                                     |
| ---------------------- | -------------------------------------- | ---------- | ------------------------------------------------------------------------------------------- |
| Vile Potion            | Default Potion                         | 4.5s       | 110% Poison total beginning 10s after consuming                                             |
| Rat Poison             | Vile Potion + Green Crispberry         | 0.5s       | Instantly kills the user                                                                    |
| Potion of Happiness    | Any Berrynana Peel + Yellow Crispberry | 4.5s       | Provides a Morale Boost to everyone (which gives a whole bar of reserve stamina)            |
| Erratic Potion         | Big Lollipop + Energy Drink            | 4.5s       | Provides the effects of the Big Lollipop and the Energy Drink at the same time for 20s      |
| Temperate Potion       | Any Clusterberry + Cluster Shroom      | 4.5s       | Provides the Temperate affliction for 120s which removes 10% of your cold & hot each second |
| Numbness Potion        | Any Mushroom + Any Mushroom            | 4.5f       | Removes all injury for 120s and then gives it back after                                    |
| Potion of Selflessness | Remedy Fungus + Shelf Fungus           | 4.5f       | Respawns all fallen teammates on the user and then kills the user                           |


### For developers: Making custom potion recipes
To create a custom potion: Create an instance of the Potions.APIs.Potion class and register it using Potions.APIs.PotionAPI.RegisterPotion
```csharp
using Potions.APIs;
using BepInEx;

namespace MyPotionPack;

[BepInAutoPlugin]
public partial class Plugin: : BaseUnityPlugin
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
