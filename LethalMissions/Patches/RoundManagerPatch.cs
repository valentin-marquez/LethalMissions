using HarmonyLib;
using Unity.Netcode;
using LethalMissions.DefaultData;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    public class RoundManagerPatch : NetworkBehaviour
    {
        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void OnAwake()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Plugin.MissionManager.RemoveActiveMissions();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(RoundManager.GenerateNewLevelClientRpc))]
        static void OnStartGame()
        {
            Plugin.LoggerInstance.LogInfo("Start Game - starting Game");

            if (StartOfRound.Instance.currentLevelID != 3)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    Plugin.LoggerInstance.LogInfo("Host or server -  Generating missions");
                    Plugin.MissionManager.GenerateMissions(Plugin.Config.MaxMissions.Value);
                }
                else
                {
                    Plugin.LoggerInstance.LogInfo("Client - Requesting missions");
                    Plugin.MissionManager.RequestMissions();
                }

                HUDManager.Instance.DisplayTip("LethalMissions", StringUtilities.GetNewMissionsAvailableMessage(Plugin.Config.LanguageCode.Value), true);
            }
        }
    }
}