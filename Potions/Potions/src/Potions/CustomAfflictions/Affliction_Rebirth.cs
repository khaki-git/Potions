using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Rebirth : Affliction
{
    public override void UpdateEffect()
    {
        character.refs.customization.PulseStatus(new Color(0.7f, 0.7f, 0.3f));
    }

    public override void OnApplied()
    {
    }

    public override void OnRemoved()
    {
    }

    public override void Stack(Affliction incomingAffliction)
    {
        if (incomingAffliction is Affliction_Rebirth)
        {
            totalTime += incomingAffliction.totalTime;
        }
    }

    public override AfflictionType GetAfflictionType()
    {
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(4);
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