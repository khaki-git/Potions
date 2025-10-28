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
        if (!character.IsLocal)
        {
            return;
        }

        var selfRefs = character.refs;
        var selfAfflictions = selfRefs?.afflictions;
        if (selfAfflictions == null)
        {
            return;
        }

        var statuses = selfAfflictions.currentStatuses;
        if (statuses == null)
        {
            return;
        }

        var maxTransfer = SPEED * Time.deltaTime;
        if (maxTransfer <= 0f)
        {
            return;
        }

        var healed = false;
        var maxDistanceSqr = DISTANCE * DISTANCE;

        foreach (var target in Character.AllCharacters)
        {
            if (target == null || target == character || target.IsLocal)
            {
                continue;
            }

            var targetData = target.data;
            if (targetData == null || targetData.dead || targetData.fullyPassedOut)
            {
                continue;
            }

            var offset = target.Center - character.Center;
            if (offset.sqrMagnitude > maxDistanceSqr)
            {
                continue;
            }

            var targetRefs = target.refs;
            if (targetRefs?.afflictions == null)
            {
                continue;
            }

            var targetCustomization = targetRefs.customization;
            var statusIndex = 0;

            foreach (var status in statuses)
            {
                if (status > 0.01f)
                {
                    var amount = Mathf.Min(status, maxTransfer);
                    if (amount > 0f)
                    {
                        target.AddStatusToThisMyselfOverRPCOkayGotIt((CharacterAfflictions.STATUSTYPE)statusIndex, amount);
                        selfAfflictions.SubtractStatus((CharacterAfflictions.STATUSTYPE)statusIndex, amount);
                        targetCustomization?.PulseStatus(Color.black);
                        healed = true;
                    }
                }

                statusIndex++;
            }
        }

        if (healed)
        {
            selfRefs?.customization?.PulseStatus(new Color(0.1f, 1f, 0.1f));
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
