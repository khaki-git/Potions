using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Numbness : Affliction
{
    private float buildup;

    public override void UpdateEffect()
    {
        if (character.IsLocal)
        {
            var currentBuildup = character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Injury);
            character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, currentBuildup);
            buildup += currentBuildup;
        }
        character.refs.customization.PulseStatus(new Color(0.7f, 0.2f, 0.2f));
    }

    public override void OnApplied()
    {
    }

    public override void OnRemoved()
    {
        character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, buildup);
    }

    public override void Stack(Affliction incomingAffliction)
    {
        if (incomingAffliction is Affliction_Numbness)
        {
            totalTime += incomingAffliction.totalTime;
        }
    }

    public override AfflictionType GetAfflictionType()
    {
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(1);
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(buildup);
        serializer.WriteFloat(totalTime);
    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        buildup = serializer.ReadFloat();
        totalTime = serializer.ReadFloat();
    }
}