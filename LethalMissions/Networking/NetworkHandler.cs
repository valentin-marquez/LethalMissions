using System;
using LethalMissions.Scripts;
using Unity.Netcode;
using UnityEngine;

namespace LethalMissions.Networking
{
    /// <summary>
    /// Handles network synchronization of missions in the LethalMissions game.
    /// </summary>
    public class NetworkHandler : NetworkBehaviour
    {
        /// <summary>
        /// Gets the instance of the NetworkHandler.
        /// </summary>
        public static NetworkHandler Instance { get; private set; }

        private string currentSerializedMissions;

        /// <summary>
        /// Called when the NetworkHandler is spawned on the network.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Synchronizes missions on the server.
        /// </summary>
        /// <param name="serializedMissions">The serialized missions to synchronize.</param>
        [ServerRpc(RequireOwnership = false)]
        public void SyncMissionsServerRpc(string serializedMissions)
        {
            currentSerializedMissions = serializedMissions;
            SyncMissionsClientRpc(serializedMissions);
        }

        /// <summary>
        /// Synchronizes missions on the clients.
        /// </summary>
        /// <param name="serializedMissions">The serialized missions to synchronize.</param>
        [ClientRpc]
        public void SyncMissionsClientRpc(string serializedMissions)
        {

            currentSerializedMissions = serializedMissions;
            Plugin.MissionManager.SyncMissions(serializedMissions);
        }

        /// <summary>
        /// Requests missions from the server.
        /// </summary>
        /// <param name="clientId">The client ID of the player requesting the missions.</param>
        [ServerRpc(RequireOwnership = false)]
        public void RequestMissionsServerRpc(ulong clientId)
        {
            if (string.IsNullOrEmpty(currentSerializedMissions))
            {
                Debug.LogError("currentSerializedMissions is null or empty");
                return;
            }
            SendMissionsClientRpc(currentSerializedMissions, clientId);
        }

        /// <summary>
        /// Sends missions to a specific client.
        /// </summary>
        /// <param name="serializedMissions">The serialized missions to send.</param>
        /// <param name="targetClientId">The client ID of the target client.</param>
        [ClientRpc]
        public void SendMissionsClientRpc(string serializedMissions, ulong targetClientId)
        {
            if (NetworkManager.Singleton.LocalClientId == targetClientId)
            {
                Plugin.MissionManager.SyncMissions(serializedMissions); // Pass the serialized missions to the MissionManager
            }
        }

    }
}