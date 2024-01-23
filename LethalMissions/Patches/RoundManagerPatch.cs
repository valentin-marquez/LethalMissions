using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using LethalMissions.Localization;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    public class RoundManagerPatch : NetworkBehaviour
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
        private static void OnFinishGeneratingNewLevel()
        {

            if (StartOfRound.Instance.currentLevelID != 3)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    Plugin.MissionManager.GenerateMissions(Plugin.Config.MaxMissions.Value);
                }

                if (Plugin.Config.NewMissionNotify.Value || Plugin.Config.PlaySoundOnly.Value)
                {
                    if (Plugin.Config.PlaySoundOnly.Value)
                    {
                        RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, HUDManager.Instance.tipsSFX, randomize: true);
                    }
                    else
                    {
                        HUDManager.Instance.DisplayTip("LethalMissions", MissionLocalization.GetMissionString("NewMissionsAvailableMessage"), true);
                    }
                }
            }
        }
    }
}