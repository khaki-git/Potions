using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Potions.Patches
{
    public static class AddStatusToMePatch
    {
        public static void AddStatusToThisMyselfOverRPCOkayGotIt(this Character self, CharacterAfflictions.STATUSTYPE statusType, float amount)
        {
            var statusHelper = self.GetComponent<StatusAdderHelper>();
            if (statusHelper == null)
            {
                statusHelper = self.gameObject.AddComponent<StatusAdderHelper>();
            }
            statusHelper.AddStatus(statusType, amount);
        }

        [RequireComponent(typeof(PhotonView))]
        public sealed class StatusAdderHelper : MonoBehaviourPun
        {
            public void AddStatus(CharacterAfflictions.STATUSTYPE statusType, float amount)
            {
                if (photonView == null || photonView.Owner == null)
                {
                    return;
                }
                photonView.RPC(nameof(RPCA_AddToStatusOverNetwork), photonView.Owner, statusType, amount);
            }

            [PunRPC]
            public void RPCA_AddToStatusOverNetwork(CharacterAfflictions.STATUSTYPE statusType, float amount)
            {
                var afflictions = GetComponent<CharacterAfflictions>();
                if (afflictions == null)
                {
                    return;
                }
                if (PhotonNetwork.LocalPlayer != null && photonView != null && photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    afflictions.AddStatus(statusType, amount);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    internal static class EnsureStatusAdderHelperPatch
    {
        private static void Postfix(Character __instance)
        {
            if (!__instance.TryGetComponent<AddStatusToMePatch.StatusAdderHelper>(out _))
            {
                __instance.gameObject.AddComponent<AddStatusToMePatch.StatusAdderHelper>();
            }
        }
    }
}
