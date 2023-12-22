using HarmonyLib;
using LethalMissions.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using LC_API;
using UnityEngine.Rendering;
using UnityEngine;
using System.Numerics;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using GameNetcodeStuff;
using LethalMissions.DefaultData;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch : NetworkBehaviour
    {

        [HarmonyPostfix]
        [HarmonyPatch("ShipLeave")]
        private static void OnEndGame()
        {
            Plugin.LoggerInstance.LogInfo($"End Game calculating rewards...");

            if (Plugin.MissionManager.IsMissionInCurrentMissions(MissionType.SurviveCrewmates))
            {
                List<PlayerControllerB> players = FindObjectsOfType<PlayerControllerB>().ToList();
                int LivingPlayers = players.Count(player => player.isInHangarShipRoom && !player.isPlayerDead);

                Plugin.LoggerInstance.LogInfo($"Living players: {LivingPlayers}");

                if (LivingPlayers >= Plugin.MissionManager.GetSurviveCrewmates())
                {
                    Plugin.MissionManager.CompleteMission(MissionType.SurviveCrewmates);
                    Plugin.LoggerInstance.LogInfo($"Completed mission {MissionType.SurviveCrewmates}");
                }
                else
                {
                    Plugin.LoggerInstance.LogInfo($"Mission {MissionType.SurviveCrewmates} not completed");
                }
            }

            if (Plugin.MissionManager.IsMissionInCurrentMissions(MissionType.OutOfTimeLeaveBeforeCertainHour))
            {
                int leaveTime = Plugin.MissionManager.GetMissionLeaveTime(MissionType.OutOfTimeLeaveBeforeCertainHour);
                if (!HUDManagerPatch.IsPM || (HUDManagerPatch.IsPM && HUDManagerPatch.Hour < leaveTime))
                {
                    Plugin.MissionManager.CompleteMission(MissionType.OutOfTimeLeaveBeforeCertainHour);
                    Plugin.LoggerInstance.LogInfo($"Completed mission {MissionType.OutOfTimeLeaveBeforeCertainHour}");
                }
                else
                {
                    Plugin.LoggerInstance.LogInfo($"Mission {MissionType.OutOfTimeLeaveBeforeCertainHour} not completed");
                }
            }

            CalculateRewards();
            Plugin.MissionManager.RemoveActiveMissions();
        }
        public static void CalculateRewards()
        {
            Terminal terminal = FindObjectOfType<Terminal>();
            int currentCredits = terminal.groupCredits;
            int creditsEarned = Plugin.MissionManager.GetCreditsEarned();
            int newGroupCredits = currentCredits + creditsEarned;
            terminal.groupCredits = newGroupCredits;
            DisplayMissionCompletion(creditsEarned);
        }
        public static void DisplayMissionCompletion(int creditsEarned)
        {
            Plugin.LoggerInstance.LogInfo("Displaying mission completion...");

            // Obtén las misiones completadas
            List<Mission> completedMissions = Plugin.MissionManager.GetCompletedMissions();
            Plugin.LoggerInstance.LogInfo($"Number of completed missions: {completedMissions.Count}");

            // Inicializa el texto
            string text = completedMissions.Count == 0
                ? StringUtilities.GetNoCompletedMissionsMessage(Plugin.Config.LanguageCode.Value)
                : StringUtilities.GetCompletedMissionsCountMessage(Plugin.Config.LanguageCode.Value, completedMissions.Count);


            GameObject header = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/SpecialHUDGraphics/RewardsScreen/Container/Container2/Header");
            // resize header 
            header.GetComponent<TextMeshProUGUI>().fontSize = 26;
            header.GetComponent<TextMeshProUGUI>().text = "Lethal Missions";
            HUDManager.Instance.moneyRewardsListText.fontSize = HUDManager.Instance.moneyRewardsListText.fontSize / 2;
            HUDManager.Instance.moneyRewardsListText.text = text;
            HUDManager.Instance.moneyRewardsTotalText.text = $"TOTAL: ${creditsEarned}";
            HUDManager.Instance.rewardsScrollbar.value = 1f;
            if (Plugin.Config.MaxMissions.Value > 8)
            {
                // Obtén el tipo de HUDManager
                Type hudManagerType = typeof(HUDManager);

                // Obtén el campo privado scrollRewardTextCoroutine
                FieldInfo scrollRewardTextCoroutineField = hudManagerType.GetField("scrollRewardTextCoroutine", BindingFlags.NonPublic | BindingFlags.Instance);

                // Obtén la instancia actual de HUDManager
                HUDManager hudManagerInstance = HUDManager.Instance;

                // Obtén el valor actual de scrollRewardTextCoroutine
                Coroutine scrollRewardTextCoroutine = (Coroutine)scrollRewardTextCoroutineField.GetValue(hudManagerInstance);

                // Si scrollRewardTextCoroutine no es null, detén la corutina
                if (scrollRewardTextCoroutine != null)
                {
                    hudManagerInstance.StopCoroutine(scrollRewardTextCoroutine);
                    Plugin.LoggerInstance.LogInfo("Stopped existing scrollRewardTextCoroutine");
                }

                // Inicia la nueva corutina y guarda la referencia en scrollRewardTextCoroutine
                scrollRewardTextCoroutineField.SetValue(hudManagerInstance, hudManagerInstance.StartCoroutine(ScrollRewardsListText()));
                Plugin.LoggerInstance.LogInfo("Started new scrollRewardTextCoroutine");
            }
            // Start the coroutine to show rewards and restore header
            HUDManager.Instance.StartCoroutine(ShowRewardsAndRestoreHeader(header));
            Plugin.LoggerInstance.LogInfo("Started ShowRewardsAndRestoreHeader coroutine");
        }

        static IEnumerator ShowRewardsAndRestoreHeader(GameObject header)
        {

            // Trigger the animation
            HUDManager.Instance.moneyRewardsAnimator.SetTrigger("showRewards");
            Plugin.LoggerInstance.LogInfo("Triggered showRewards animation");

            yield return new WaitForSeconds(10f); // Increased wait time to 10 seconds

            // Now restore
            HUDManager.Instance.moneyRewardsListText.fontSize = HUDManager.Instance.moneyRewardsListText.fontSize * 2;
            header.GetComponent<TextMeshProUGUI>().fontSize = 36;
            header.GetComponent<TextMeshProUGUI>().text = "PAYCHECK!";
            Plugin.LoggerInstance.LogInfo("Restored header text and font size");
        }

        private static IEnumerator ScrollRewardsListText()
        {
            yield return new WaitForSeconds(1.5f);
            Plugin.LoggerInstance.LogInfo("Waited 1.5 seconds");

            float num = 3f;
            while (num >= 0f)
            {
                num -= Time.deltaTime;
                HUDManager.Instance.rewardsScrollbar.value -= Time.deltaTime / num;
                Plugin.LoggerInstance.LogInfo($"Updated rewardsScrollbar value to {HUDManager.Instance.rewardsScrollbar.value}");
            }
            Plugin.LoggerInstance.LogInfo("Finished scrolling rewards list text");
            yield break;
        }
    }
}