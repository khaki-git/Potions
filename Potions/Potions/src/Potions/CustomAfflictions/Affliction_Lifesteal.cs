using System.Linq;
using Peak.Afflictions;
using Potions.Patches;
using Zorro.Core.Serizalization;

namespace Potions.CustomAfflictions;

using UnityEngine;

public sealed class Affliction_Lifesteal : Affliction
{
    private const float DISTANCE = 15f;
    private const float SPEED = 0.02f;
    
    public override void UpdateEffect()
    {
        if (character.IsLocal)
        {
            var targets = Character.AllCharacters.Where(otherCharacter =>
            {
                if (otherCharacter.data.dead || otherCharacter.data.fullyPassedOut || otherCharacter.IsLocal)
                    return false;
                return !((otherCharacter.Center - character.Center).magnitude > DISTANCE);
            });

            var healed = false;

            foreach (var target in targets)
            {
                var affs = target.refs.afflictions;

                var x = 0;
                foreach (var status in character.refs.afflictions.currentStatuses)
                {
                    if (status > 0.01f)
                    {
                        var amount = Mathf.Clamp(status, 0, SPEED * Time.deltaTime);
                        target.AddStatusToThisMyselfOverRPCOkayGotIt((CharacterAfflictions.STATUSTYPE)x, amount);
                        affs.SubtractStatus((CharacterAfflictions.STATUSTYPE)x, amount);
                        target.refs.customization.PulseStatus(Color.black);
                        healed = true;
                    }
                    x++;
                }
            }

            if (healed)
            {
                character.refs.customization.PulseStatus(new Color(0.1f, 1f, 0.1f));
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
        if (incomingAffliction is Affliction_Lifesteal)
        {
            totalTime += incomingAffliction.totalTime;
        }
    }

    public override AfflictionType GetAfflictionType()
    {
        return (AfflictionType)CustomAfflictionHelper.WhatIsMyNumber(2);
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