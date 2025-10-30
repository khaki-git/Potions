using HarmonyLib;
using Peak.Afflictions;
using Photon.Pun;
using Potions.CustomAfflictions;
using Vector3 = UnityEngine.Vector3;

namespace Potions.Patches;

[HarmonyPatch(typeof(Character), "HandlePassedOut")]
public class CharacterPatch
{
    private static bool Prefix(Character __instance)
    {
        if (!(__instance.data.deathTimer > 0.9f) ||
            !__instance.refs.afflictions.HasAfflictionType(
                (Affliction.AfflictionType)CustomAfflictionHelper.POTIONS_ENUM_START + 4, out _)) return true;
        __instance.view.RPC("RPCA_ReviveAtPosition", RpcTarget.All, __instance.Center + Vector3.up * 5, false);
        return false;

    }
}