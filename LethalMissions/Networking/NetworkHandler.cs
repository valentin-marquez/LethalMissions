using System;
using Unity.Netcode;
using UnityEngine;

namespace LethalMissions.Networking
{
    public class NetworkHandler : NetworkBehaviour
    {
        public static NetworkHandler Instance { get; private set; }

        // Field to store the current serialized missions
        private string currentSerializedMissions;

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            }

            Instance = this;
            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncMissionsServerRpc(string serializedMissions)
        {
            Debug.Log($"LethalMissions - SyncMissionsServerRpc: {serializedMissions}");
            currentSerializedMissions = serializedMissions; // Store the current serialized missions
            SyncMissionsClientRpc(serializedMissions);
        }

        [ClientRpc]
        public void SyncMissionsClientRpc(string serializedMissions)
        {
            Debug.Log($"LethalMissions - SyncMissionsClientRpc: {serializedMissions}");
            currentSerializedMissions = serializedMissions; // Store the current serialized missions
            Plugin.MissionManager.SyncMissions(serializedMissions); // Pass the serialized missions to the MissionManager
        }
    }
}