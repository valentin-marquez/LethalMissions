using HarmonyLib;
using Unity.Netcode;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    public class GameNetworkManagerPatch : NetworkBehaviour
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnClientDisconnect")]
        static void OnClientDisconnect()
        {
            Plugin.MissionManager.RemoveActiveMissions();
        }

    }
}