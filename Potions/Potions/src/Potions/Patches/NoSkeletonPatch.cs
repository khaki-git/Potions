using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Potions.Patches;

public static class CharacterNoSkeletonPatch
{
    private static readonly HashSet<int> SkipNextSkeleton = new HashSet<int>();

    public static void DieInstantlyWithoutSkeleton(this Character self)
    {
        if (self == null) return;
        var pv = self.GetComponent<PhotonView>();
        if (pv == null) return;
        SkipNextSkeleton.Add(pv.ViewID);
        pv.RPC("RPCA_Die", RpcTarget.All, self.Center);
    }

    private static bool ConsumeSkip(Character c)
    {
        if (c == null) return false;
        var pv = c.GetComponent<PhotonView>();
        if (pv == null) return false;
        return SkipNextSkeleton.Remove(pv.ViewID);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Skelleton), "SpawnSkelly", new[] { typeof(Character) })]
    private static bool Skelly_SpawnSkelly_Prefix(Skelleton __instance, Character c)
    {
        if (!ConsumeSkip(c)) return true;
        if (__instance != null) Object.Destroy(__instance.gameObject);
        return false;
    }
}
