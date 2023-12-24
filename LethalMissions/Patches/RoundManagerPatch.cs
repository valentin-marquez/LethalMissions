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
            LogStartGame();
            if (ShouldGenerateMissions())
            {
                GenerateMissions();
                if (StartOfRound.Instance.currentLevel.levelID != 3)
                {
                    HUDManager.Instance?.DisplayTip("LethalMissions", StringUtilities.GetNewMissionsAvailableMessage(Plugin.Config.LanguageCode.Value), true);
                }
            }
            else if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                Plugin.MissionManager.RequestMissions();
            }
        }

        static void LogStartGame()
        {
            Plugin.LoggerInstance.LogInfo("Start Game - starting Game");
        }

        static bool ShouldGenerateMissions()
        {
            var startOfRound = StartOfRound.Instance;
            if (startOfRound == null || startOfRound.currentLevel == null)
            {
                return false;
            }

            return startOfRound.currentLevel.levelID != 3 && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer);
        }

        static void GenerateMissions()
        {
            Plugin.LoggerInstance.LogInfo("Host or server -  Generating missions");
            Plugin.MissionManager.GenerateMissions(Plugin.Config.MaxMissions.Value);
        }
    }
}