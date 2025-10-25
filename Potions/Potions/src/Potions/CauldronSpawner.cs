using BepInEx.Logging;
using PEAKLib.Core;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potions;

public class CauldronSpawner
{
    public ManualLogSource log;
    private const float Liftup = 5f;
    private const float Radius = 2f;
    private const float NudgeUp = 0.62f;
    
    public void BindToScene()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (!PhotonNetwork.IsMasterClient) return; // only let the sigma alpha wolf daddy spawn in shit
            if (!scene.name.StartsWith("Level_") && !scene.name.Contains("WilIsland")) return; // don't load stuff in the airport
            
            log.LogInfo("Started spawning cauldrons...");
            var campfires = GameObjectFinder.FindByName("Campfire", includeInactive: false);
            foreach (var campfire in campfires)
            {
                CreateCauldronForCampfire(campfire);
            }
        };
    }

    private static Quaternion FromHitNormal(RaycastHit hit, Vector3 upHint)
    {
        return FromNormal(hit.normal, upHint);
    }    
    
    private static Quaternion FromNormal(Vector3 normal, Vector3 upHint)
    {
        normal = normal.sqrMagnitude > 0f ? normal.normalized : Vector3.forward;
        var up = upHint.sqrMagnitude > 0f ? upHint.normalized : Vector3.up;

        if (!(Mathf.Abs(Vector3.Dot(normal, up)) > 0.999f)) return Quaternion.LookRotation(normal, up);
        var arbitrary = Mathf.Abs(normal.x) < 0.9f ? Vector3.right : Vector3.forward;
        up = Vector3.Cross(normal, arbitrary).normalized;

        return Quaternion.LookRotation(normal, up);
    }
    
    private void CreateCauldronForCampfire(GameObject campfire)
    {
        var campfireTf = campfire.transform;
        var campfireComponent = campfire.GetComponent<Campfire>();
        if (campfireComponent == null) return;
        
        // cauldron spawning mechanics
        var cauldronSpawnerHead = campfireTf.position + Vector3.up * Liftup;
        var radians = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        var cauldronHead = cauldronSpawnerHead + new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * Radius;

        if (Physics.Raycast(cauldronHead, Vector3.down, out var hit, Mathf.Infinity))
        { 
            var cauldronRot = FromHitNormal(hit, Vector3.up);
            var cauldron = NetworkPrefabManager.SpawnNetworkPrefab(Plugin.cauldronId, hit.point + Vector3.up * NudgeUp, cauldronRot);
        }
        else
        {
            log.LogInfo("AAAAAAA SOMEONE HELP ME IM DYING PLEASE I BEG YOU" +
                        "\nokay but actually what the fuck even happened");
        }
    }
}