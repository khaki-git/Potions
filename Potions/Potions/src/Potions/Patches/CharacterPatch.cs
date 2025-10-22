using HarmonyLib;
using Peak.Afflictions;
using Potions.CustomAfflictions;
using Vector3 = UnityEngine.Vector3;

namespace Potions.Patches;

[HarmonyPatch(typeof(Character), "HandlePassedOut")]
public class CharacterPatch
{
    private static bool Prefix(Character __instance)
    {
        Affliction ignored;
        if (!(__instance.data.deathTimer > 0.9f) ||
            !__instance.refs.afflictions.HasAfflictionType(
                (Affliction.AfflictionType)CustomAfflictionHelper.POTIONS_ENUM_START + 4, out ignored)) return true;
        __instance.RPCA_ReviveAtPosition(__instance.Center + Vector3.up * 5, false);
        return false;

    }
}