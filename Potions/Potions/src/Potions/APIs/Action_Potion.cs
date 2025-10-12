using System;
using UnityEngine;

namespace Potions.APIs;

public class Action_Potion: ItemAction
{
    public string potion;
    
    public override void RunAction()
    {
        var realPotion = PotionAPI.FromId(potion);
        Debug.Log("Ran action!");
        foreach (var pe in realPotion.effects)
        {
            Debug.Log("Drank!");
            pe.Drink(GetComponent<Item>()._holderCharacter);
        }
    }

    private void Start()
    {
        var realPotion = PotionAPI.FromId(potion);
        var fluid = transform.Find("Potion").Find("Container").Find("Container").gameObject;
        var renderer = fluid.GetComponent<Renderer>();
        var mat = renderer.material;
        var fluidColor = realPotion.liquidColour;

        var shader = Shader.Find("W/Peak_Standard");
        if (shader != null) mat.shader = shader;

        var tintId = Shader.PropertyToID("Tint");
        if (!mat.HasProperty(tintId)) tintId = Shader.PropertyToID("_Tint");
        if (mat.HasProperty(tintId)) mat.SetColor(tintId, fluidColor);

        var baseId = Shader.PropertyToID("BaseColor");
        if (!mat.HasProperty(baseId)) baseId = Shader.PropertyToID("_BaseColor");
        if (mat.HasProperty(baseId))
        {
            var currentBase = mat.GetColor(baseId);
            var target = new Color(fluidColor.r, fluidColor.g, fluidColor.b, currentBase.a);
            mat.SetColor(baseId, Color.Lerp(currentBase, target, 0.5f));
        }
    }
}