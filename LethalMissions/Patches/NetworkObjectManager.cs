using HarmonyLib;
using UnityEngine;
using Unity.Netcode;
using GameNetcodeStuff;
using LethalMissions.Networking;

namespace LethalMissions.Patches
{
    public class NetworkObjectManager
    {
        static GameObject networkPrefab;
        static GameObject networkHandlerHost;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        public static void OnStart()
        {
            if (networkPrefab != null) return;

            networkPrefab = (GameObject)Assets.MainAssetBundle.LoadAsset("LethalMissionsNetworkHandler");
            networkPrefab.AddComponent<NetworkHandler>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
            Plugin.LoggerInstance.LogInfo("LethalMissions:  NetworkHandler prefab added to NetworkManager");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                networkHandlerHost = Object.Instantiate(networkPrefab);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn(true);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        static void DestroyNetworkHandler()
        {
            try
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    Plugin.LoggerInstance.LogInfo("LethalMissions:  Destroying NetworkHandler on Host");
                    Object.Destroy(networkHandlerHost);
                    networkHandlerHost = null;
                }
            }
            catch
            {
                Plugin.LoggerInstance.LogError("LethalMissions:  Failed to destroy NetworkHandler on Host");
            }
        }
    }
}