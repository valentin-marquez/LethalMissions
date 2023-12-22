using HarmonyLib;
using UnityEngine;
using Unity.Netcode;
using GameNetcodeStuff;

namespace LethalMissions.Networking
{
    public class NetworkObjectManager
    {
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void OnStart(PlayerControllerB __instance)
        {
            if (__instance.NetworkManager.IsHost || __instance.NetworkManager.IsServer)
            {
                Plugin.LoggerInstance.LogInfo("Spawning network handler");
                networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
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
        static GameObject networkPrefab;
        static GameObject networkHandlerHost;
    }
}