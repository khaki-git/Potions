using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Levitation : Affliction
{
    public override void UpdateEffect()
    {
        if (character.IsLocal)
        {
            for (var i = 0; i < character.refs.ragdoll.partList.Count; i++)
            {
                if (!character.data.isGrounded)
                {
                    character.refs.ragdoll.partList[i].Gravity(character.refs.movement.GetGravityForce() * character.data.currentRagdollControll * -1);
                }
            }
        }
    }

    public override void OnApplied()
    {
    }

    public override void OnRemoved()
    {
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
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(3);
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