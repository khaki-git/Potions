using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Pinata : Affliction
{
    public override void UpdateEffect()
    {
        var c = Color.HSVToRGB(Mathf.Repeat(Time.time, 1f), 1f, 1f);
        character.refs.customization.PulseStatus(c);
    }

    public override void OnApplied()
    {
    }

    public override void OnRemoved()
    {
    }

    public override void Stack(Affliction incomingAffliction)
    {
        if (incomingAffliction is Affliction_Pinata)
        {
            totalTime += incomingAffliction.totalTime;
        }
    }

    public override AfflictionType GetAfflictionType()
    {
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(5);
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(totalTime);
    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        totalTime = serializer.ReadFloat();
    }
}