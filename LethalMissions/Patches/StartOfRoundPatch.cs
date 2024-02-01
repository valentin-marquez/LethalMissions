using HarmonyLib;
using LethalMissions.Scripts;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Reflection;
using LethalMissions.Localization;
namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch : NetworkBehaviour
    {

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void OnUpdate()
        {

            if (StartOfRound.Instance == null) return;

            var gameStates = new Dictionary<GameStateEnum, Func<bool>>
            {
                { GameStateEnum.OnMoon, () => StartOfRound.Instance.shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HangarShipLandB" },
                { GameStateEnum.InOrbit, () => StartOfRound.Instance.inShipPhase },
                { GameStateEnum.TakingOff, () => StartOfRound.Instance.shipIsLeaving }
            };

            foreach (var gameState in gameStates)
            {
                if (gameState.Value.Invoke() && Plugin.CurrentState != gameState.Key)
                {
                    ChangeGameState(gameState.Key);
                }
            }
        }

        private static void ChangeGameState(GameStateEnum state)
        {
            Plugin.CurrentState = state;
        }
        /// <summary>
        /// Method called at the end of the game.
        /// Checks if certain missions have been completed and calculates rewards accordingly.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("ShipLeave")]
        private static void OnEndGame()
        {
            if (StartOfRound.Instance.allPlayersDead || StartOfRound.Instance.currentLevel.levelID == 3)
                return;

            CalculateRewards();
        }

        /// <summary>
        /// Calculates the rewards earned at the start of a round.
        /// </summary>
        public static void CalculateRewards()
        {
            Terminal terminal = FindObjectOfType<Terminal>();
            int currentCredits = terminal.groupCredits;
            int creditsEarned = Plugin.MissionManager.GetCreditsEarned();
            int newGroupCredits = currentCredits + creditsEarned;
            terminal.groupCredits = newGroupCredits;
            DisplayMissionCompletion(creditsEarned);
        }

        /// <summary>
        /// Displays the mission completion screen with the given credits earned.
        /// </summary>
        /// <param name="creditsEarned">The number of credits earned.</param>
        public static void DisplayMissionCompletion(int creditsEarned)
        {
            List<Mission> completedMissions = Plugin.MissionManager.GetCompletedMissions();

            string text = completedMissions.Count == 0
            ? MissionLocalization.GetMissionString("NoCompletedMissionsMessage")
            : MissionLocalization.GetMissionString("CompletedMissionsCountMessage", completedMissions.Count);


            GameObject header = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/SpecialHUDGraphics/RewardsScreen/Container/Container2/Header");
            header.GetComponent<TextMeshProUGUI>().fontSize = 26;
            header.GetComponent<TextMeshProUGUI>().text = "Lethal Missions";
            HUDManager.Instance.moneyRewardsListText.fontSize = HUDManager.Instance.moneyRewardsListText.fontSize / 2;
            HUDManager.Instance.moneyRewardsListText.text = text;
            HUDManager.Instance.moneyRewardsTotalText.text = $"TOTAL: ${creditsEarned}";
            HUDManager.Instance.rewardsScrollbar.value = 1f;
            if (Plugin.Config.NumberOfMissions.Value > 8)
            {
                Type hudManagerType = typeof(HUDManager);
                FieldInfo scrollRewardTextCoroutineField = hudManagerType.GetField("scrollRewardTextCoroutine", BindingFlags.NonPublic | BindingFlags.Instance);
                HUDManager hudManagerInstance = HUDManager.Instance;
                Coroutine scrollRewardTextCoroutine = (Coroutine)scrollRewardTextCoroutineField.GetValue(hudManagerInstance);
                if (scrollRewardTextCoroutine != null)
                {
                    hudManagerInstance.StopCoroutine(scrollRewardTextCoroutine);
                }
                scrollRewardTextCoroutineField.SetValue(hudManagerInstance, hudManagerInstance.StartCoroutine(ScrollRewardsListText()));
            }
            HUDManager.Instance.StartCoroutine(ShowRewardsAndRestoreHeader(header));
        }

        /// <summary>
        /// Represents an enumerator that can be used to iterate over a collection of objects.
        /// </summary>
        /// <returns>An object that can be used to iterate over a collection of objects.</returns>
        static IEnumerator ShowRewardsAndRestoreHeader(GameObject header)
        {

            HUDManager.Instance.moneyRewardsAnimator.SetTrigger("showRewards");

            yield return new WaitForSeconds(10f);

            HUDManager.Instance.moneyRewardsListText.fontSize = HUDManager.Instance.moneyRewardsListText.fontSize * 2;
            header.GetComponent<TextMeshProUGUI>().fontSize = 36;
            header.GetComponent<TextMeshProUGUI>().text = "PAYCHECK!";
        }

        /// <summary>
        /// Represents an enumerator that supports iterating over a collection of objects.
        /// </summary>
        /// <returns>An object that can be used to iterate over a collection of objects.</returns>
        private static IEnumerator ScrollRewardsListText()
        {
            yield return new WaitForSeconds(1.5f);

            float num = 3f;
            while (num >= 0f)
            {
                num -= Time.deltaTime;
                HUDManager.Instance.rewardsScrollbar.value -= Time.deltaTime / num;
            }
            yield break;
        }

    }
}