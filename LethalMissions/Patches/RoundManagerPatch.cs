using HarmonyLib;
using Unity.Netcode;
using LethalMissions.Localization;
using LethalMissions.Scripts;

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
        [HarmonyPatch(nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
        private static void OnFinishGeneratingNewLevel()
        {
            if (StartOfRound.Instance.currentLevelID != 3)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    Plugin.LogInfo("Host or server -  Generating missions");
                    Plugin.MissionManager.GenerateMissions(Plugin.Config.MaxMissions.Value);
                }
                else
                {
                    Plugin.LogInfo("Client - Requesting missions");
                    Plugin.MissionManager.RequestMissions();
                }

                switch (Plugin.Config.MissionsNotification.Value)
                {
                    case NotificationOption.SoundOnly:
                        RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, HUDManager.Instance.tipsSFX, randomize: true);
                        break;
                    case NotificationOption.SoundAndBanner:
                        RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, HUDManager.Instance.tipsSFX, randomize: true);
                        HUDManager.Instance.DisplayTip("LethalMissions", MissionLocalization.GetMissionString("NewMissionsAvailable"), true);
                        break;
                    case NotificationOption.None:
                    default:
                        break;
                }
            }
        }
    }
}