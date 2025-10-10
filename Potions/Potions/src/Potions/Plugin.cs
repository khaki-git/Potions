using BepInEx;
using BepInEx.Logging;
using PEAKLib.Core;
using Photon.Pun;
using UnityEngine;

namespace Potions;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static CauldronSpawner _spawner;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static ModDefinition modDefinition;
    internal static string cauldronId = "cauldron_campfire";

    private void Awake()
    {
        modDefinition = ModDefinition.GetOrCreate(Info);
        
        Log = Logger;

        Log.LogInfo($"Plugin {Name} is loaded!");
        
        // create the basics
        _spawner = new CauldronSpawner
        {
            log = Log
        };
        
        // init them
        _spawner.BindToScene();
        
        // other
        this.LoadBundleWithName(
            "potions.peakbundle",
            peakBundle =>
            {
                var cauldron = peakBundle.LoadAsset<GameObject>("CauldronPrefab");
                cauldron.AddComponent<PhotonView>();
                cauldron.AddComponent<Cauldron>();
                foreach (Transform child in cauldron.transform)
                {
                    PatchMaterialShader(child.gameObject);
                }
                PatchMaterialShader(cauldron);
                NetworkPrefabManager.RegisterNetworkPrefab(cauldronId, cauldron);
                peakBundle.Mod.RegisterContent();
            }
        );
    } 

    private static void PatchMaterialShader(GameObject toPatch)
    {
        var meshrenderer = toPatch.GetComponent<MeshRenderer>();
        if (meshrenderer != null)
        {
            // give it the proper material
            meshrenderer.sharedMaterial.shader = Shader.Find("W/Peak_Standard");
        }
    }
}