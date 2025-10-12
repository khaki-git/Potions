using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PEAKLib.Core;
using Photon.Pun;
using UnityEngine;
using PEAKLib.Items;
using PEAKLib.Items.UnityEditor;
using Potions.APIs;
using Potions.Juicing;

namespace Potions;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static CauldronSpawner _spawner;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static ModDefinition modDefinition;
    internal static string cauldronId = "cauldron_campfire";

    internal static SFX_Instance FireStarted;

    private void Awake()
    {
        var harmony = new Harmony("com.khakixd.potions");
        harmony.PatchAll();
        
        modDefinition = ModDefinition.GetOrCreate(Info);
        BuiltinPotions.CreatePotions();
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
                FireStarted = peakBundle.LoadAsset<SFX_Instance>("SFXIIgnite");
                cauldron.AddComponent<PhotonView>();
                cauldron.AddComponent<Cauldron>();
                foreach (Transform child in cauldron.transform)
                {
                    PatchMaterialShader(child.gameObject);
                }
                PatchMaterialShader(cauldron);
                var referenceBubble = cauldron.transform.Find("Cylinder").Find("ReferenceBubble");
                PatchMaterialShader(referenceBubble.gameObject);
                foreach (Transform child in cauldron.transform.Find("EnableWhenLit"))
                {
                    ParticleUnfucker.ReplaceParticleMaterial(child.gameObject, child.gameObject.name);
                }
                NetworkPrefabManager.RegisterNetworkPrefab(cauldronId, cauldron);

                var bubbleSystem = referenceBubble.parent.Find("Bubbles").gameObject.AddComponent<Bubbles>();
                bubbleSystem.enabled = false;
                bubbleSystem.__enabled = false;
                bubbleSystem.referenceBubble = referenceBubble;
                var basePotion = peakBundle.LoadAsset<UnityItemContent>("GenericPotion");
                
                PotionAPI.CreatePotionRegisterables(basePotion, peakBundle.Mod);
                peakBundle.Mod.RegisterContent();
            }
        );
    } 

    private static void PatchMaterialShader(GameObject toPatch, string shaderId = "W/Peak_Standard")
    {
        var meshrenderer = toPatch.GetComponent<MeshRenderer>();
        if (meshrenderer != null)
        {
            // give it the proper material
            meshrenderer.sharedMaterial.shader = Shader.Find(shaderId);
        }
    }
}