using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Temperate : Affliction
{
    private float statusPerSecond;

    public Affliction_Temperate()
    {
        if (statusPerSecond < 0.01f)
        {
            statusPerSecond = 0.1f;
        }
    }

    public Affliction_Temperate(float statusPerSecond, float duration)
    {
        this.statusPerSecond = statusPerSecond;
        totalTime = duration;
    }

    public override void UpdateEffect()
    {
        if (character.IsLocal)
        {
            character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, statusPerSecond * Time.deltaTime);
            character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, statusPerSecond * Time.deltaTime);
        }
        character.refs.customization.PulseStatus(Color.gray);
    }

    public override void OnApplied()
    {
    }

    public override void OnRemoved()
    {
    }

    public override void Stack(Affliction incomingAffliction)
    {
        if (incomingAffliction is Affliction_Temperate)
        {
            totalTime += incomingAffliction.totalTime;
        }
    }

    public override AfflictionType GetAfflictionType()
    {
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(0);
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(statusPerSecond);
        serializer.WriteFloat(totalTime);
    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        statusPerSecond = serializer.ReadFloat();
        totalTime = serializer.ReadFloat();
    }
}
