using System;
using System.Reflection;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Peak.Afflictions;
using Potions.CustomAfflictions;

namespace Potions.Patches;

[HarmonyPatch]
public class PinataDeath
{
    static readonly Affliction.AfflictionType AffType =
        (Affliction.AfflictionType)CustomAfflictionHelper.POTIONS_ENUM_START + 5;

    static MethodBase TargetMethod()
    {
        var t = typeof(global::Character);
        return AccessTools.Method(t, "RPCA_Die", new[] { typeof(Vector3) });
    }

    static void Prefix(global::Character __instance, Vector3 itemSpawnPoint)
    {
        Debug.Log("bro died lmfao");

        if (!__instance.IsLocal) return;

        if (__instance.refs.afflictions.HasAfflictionType(AffType, out _))
        {
            Debug.Log("Spawning in Candy");
            ObjectSpawnerInterface.singleton.SpawnObject(
                new SpawnRequest("0_items/Lollipop", __instance.Center, Quaternion.identity),
                view =>
                {
                    view.GetComponent<Rigidbody>()
                        .AddForce(UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(15, 50), ForceMode.Impulse);
                });

            for (var i = 0; i < 4; i++)
            {
                ObjectSpawnerInterface.singleton.SpawnObject(
                    new SpawnRequest("0_Items/com.khakixd.potions:Candy", __instance.Center, Quaternion.identity),
                    view =>
                    {
                        view.GetComponent<Rigidbody>()
                            .AddForce(UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(15, 50), ForceMode.Impulse);
                    });
            }
        }
    }
}