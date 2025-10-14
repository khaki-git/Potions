using HarmonyLib;
using Peak.Afflictions;
using Potions.CustomAfflictions;

namespace Potions.Patches;

[HarmonyPatch(typeof(Affliction), "CreateBlankAffliction")]
public class CreateBlankAfflictionPatch
{
    private static bool Prefix(Affliction.AfflictionType afflictionType, ref Affliction __result)
    {
        switch ((int)afflictionType)
        {
            case CustomAfflictionHelper.POTIONS_ENUM_START + 0:
                __result = new Affliction_Temperate();
                return false;
            case CustomAfflictionHelper.POTIONS_ENUM_START + 1:
                __result = new Affliction_Numbness();
                return false;
            case CustomAfflictionHelper.POTIONS_ENUM_START + 2:
                __result = new Affliction_Lifesteal();
                return false;
        }
        
        return true;
    }
}