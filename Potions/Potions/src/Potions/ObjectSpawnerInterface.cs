using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potions
{
    internal class SpawnRequest
    {
        public string Obj;
        public Vector3 Pos;
        public Quaternion Rot;

        public SpawnRequest(string objectName, Vector3 position, Quaternion rotation)
        {
            Obj = objectName;
            Pos = position;
            Rot = rotation;
        }
    }

    internal sealed class ObjectSpawnerInterface : MonoBehaviour, IOnEventCallback
    {
        public static ObjectSpawnerInterface singleton;

        const byte EvSpawnRequest = 51;
        const byte EvSpawnResponse = 52;

        readonly Dictionary<int, Action<PhotonView>> _callbacks = new();

        void Awake()
        {
            if (singleton != null && singleton != this)
            {
                Destroy(gameObject);
                return;
            }
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void SpawnObject(SpawnRequest request, Action<PhotonView> callback)
        {
            if (!PhotonNetwork.InRoom) throw new InvalidOperationException("Not in a Photon room.");
            if (string.IsNullOrWhiteSpace(request.Obj)) throw new ArgumentException("Prefab path required.", nameof(request));

            var requestId = GenerateRequestId();
            _callbacks[requestId] = callback;

            if (PhotonNetwork.IsMasterClient)
            {
                var pv = DoMasterSpawn(request.Obj, request.Pos, request.Rot);
                TryInvoke(requestId, pv);
            }
            else
            {
                var content = new object[]
                {
                    request.Obj,
                    request.Pos.x, request.Pos.y, request.Pos.z,
                    request.Rot.x, request.Rot.y, request.Rot.z, request.Rot.w,
                    requestId,
                    PhotonNetwork.LocalPlayer.ActorNumber
                };

                var options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                PhotonNetwork.RaiseEvent(EvSpawnRequest, content, options, SendOptions.SendReliable);
            }
        }

        public void SpawnAndInvokeRPC(string prefabPath, Vector3 position, Quaternion rotation, string rpcMethodName, RpcTarget target = RpcTarget.AllBuffered, params object[] rpcParams)
        {
            SpawnObject(new SpawnRequest(prefabPath, position, rotation), pv =>
            {
                if (!pv) return;
                pv.RPC(rpcMethodName, target, rpcParams ?? Array.Empty<object>());
            });
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == EvSpawnRequest)
            {
                if (!PhotonNetwork.IsMasterClient) return;

                var data = photonEvent.CustomData as object[];
                if (data == null || data.Length < 10) return;

                var obj = data[0] as string;
                if (string.IsNullOrEmpty(obj)) return;

                var px = (float)data[1];
                var py = (float)data[2];
                var pz = (float)data[3];
                var rx = (float)data[4];
                var ry = (float)data[5];
                var rz = (float)data[6];
                var rw = (float)data[7];
                var reqId = (int)data[8];
                var targetActor = (int)data[9];

                var pos = new Vector3(px, py, pz);
                var rot = new Quaternion(rx, ry, rz, rw);

                var pv = DoMasterSpawn(obj, pos, rot);

                var resp = new object[] { pv.ViewID, reqId };
                var options = new RaiseEventOptions { TargetActors = new[] { targetActor } };
                PhotonNetwork.RaiseEvent(EvSpawnResponse, resp, options, SendOptions.SendReliable);
            }
            else if (photonEvent.Code == EvSpawnResponse)
            {
                var data = photonEvent.CustomData as object[];
                if (data == null || data.Length < 2) return;

                var viewId = (int)data[0];
                var reqId = (int)data[1];

                var pv = PhotonView.Find(viewId);
                TryInvoke(reqId, pv);
            }
        }

        PhotonView DoMasterSpawn(string objectName, Vector3 pos, Quaternion rot)
        {
            var go = PhotonNetwork.Instantiate(objectName, pos, rot);
            var pv = go.GetComponent<PhotonView>();
            if (!pv) throw new InvalidOperationException($"Spawned prefab '{objectName}' has no PhotonView on root.");
            return pv;
        }

        void TryInvoke(int requestId, PhotonView pv)
        {
            if (_callbacks.TryGetValue(requestId, out var cb))
            {
                _callbacks.Remove(requestId);
                cb?.Invoke(pv);
            }
        }

        int GenerateRequestId()
        {
            int id;
            do id = Random.Range(int.MinValue, int.MaxValue);
            while (_callbacks.ContainsKey(id));
            return id;
        }

        public static void EnsureSingleton()
        {
            if (singleton && singleton.gameObject) return;
            var go = new GameObject(nameof(ObjectSpawnerInterface));
            go.AddComponent<ObjectSpawnerInterface>();
        }
    }
}
