using Photon.Pun;
using UnityEngine;

namespace Potions.Patches
{
    public static class SeekEmploymentExtensions
    {
        public static void SeekEmployment(this Character character)
        {
            var go = character.gameObject;
            var helper = go.GetComponent<SeekEmploymentBehaviour>();
            if (helper == null) helper = go.AddComponent<SeekEmploymentBehaviour>();
            helper.InvokeSeekEmployment();
        }
    }

    public sealed class SeekEmploymentBehaviour : MonoBehaviourPun
    {
        public void InvokeSeekEmployment()
        {
            photonView.RPC(nameof(RPCA_SeekEmployment), RpcTarget.All);
        }

        [PunRPC]
        private void RPCA_SeekEmployment()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}