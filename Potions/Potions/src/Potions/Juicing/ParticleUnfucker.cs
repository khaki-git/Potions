using UnityEngine;

namespace Potions.Juicing;

internal static class ParticleUnfucker
{
    internal static void ReplaceParticleMaterial(GameObject targetObject, string shaderName = "SmokeParticleSimple")
    {
        if (targetObject == null || string.IsNullOrEmpty(shaderName))
        {
            Debug.LogWarning("wtf");
            return;
        }


        var newShader = Shader.Find(shaderName);


        var renderers = targetObject.GetComponentsInChildren<ParticleSystemRenderer>();


        foreach (var psRenderer in renderers)
        {
            if (psRenderer.sharedMaterial != null)
            {
                var updatedMat = new Material(psRenderer.sharedMaterial);


                updatedMat.shader = newShader;


                psRenderer.material = updatedMat;
            }
        }
    }
}