#pragma warning disable Harmony003
using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using LethalMissions.Localization;
using LethalMissions.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LethalMissions
{
    [HarmonyPatch]
    public class MenuManager
    {
        public static QuickMenuManager QuickMenuManager => StartOfRound.Instance?.localPlayerController?.quickMenuManager;
        public static PlayerControllerB LocalPlayerController => StartOfRound.Instance?.localPlayerController;

        public static GameObject MenugameObject;
        public static GameObject MenuPanelMissions;
        public static GameObject ItemListMission;
        public static ScrollRect scrollRect;
        public static Animator MissionsMenuAnimator;
        public static bool isOpen = false;


        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager), "Start")]
        public static void Initialize(HUDManager __instance)
        {
            if (Plugin.MissionsMenuPrefab == null)
            {
                return;
            }

            MenugameObject = GameObject.Instantiate(Plugin.MissionsMenuPrefab);
            MissionsMenuAnimator = MenugameObject.GetComponent<Animator>();

            if (MissionsMenuAnimator != null)
            {
                var controller = MissionsMenuAnimator.runtimeAnimatorController;
            }

            MenugameObject.transform.SetAsLastSibling();
            MenuPanelMissions = MenugameObject.transform.GetChild(0).GetComponentInChildren<ScrollRect>().content.gameObject;
            scrollRect = MenugameObject.transform.GetChild(0).GetComponentInChildren<ScrollRect>();
        }
        public static void ToggleMissionsMenu()
        {
            if (!isOpen)
            {
                OpenMissionsMenu();
            }
            else
            {
                CloseMissionsMenu();
            }
        }


        public static void CloseMissionsMenu()
        {
            MissionsMenuAnimator.SetTrigger("close");
            isOpen = false;
        }

        public static void OpenMissionsMenu()
        {
            MissionsMenuAnimator.SetTrigger("open");
            isOpen = true;

            var currentActiveMissions = Plugin.MissionManager.GetActiveMissions();

            foreach (Transform child in MenuPanelMissions.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (var mission in currentActiveMissions)
            {
                CreateAndConfigureMissionItem(mission);
            }
        }

        /// <summary>
        /// Creates and configures a mission item based on the provided mission.
        /// </summary>
        /// <param name="mission">The mission object containing the mission details.</param>
        private static void CreateAndConfigureMissionItem(Mission mission)
        {
            var missionItem = GameObject.Instantiate(Plugin.missionItemPrefab, MenuPanelMissions.transform);
            string name = $"{mission.Name}";
            string objective = $"{mission.Objective}";
            Sprite sprite = null;

            switch (mission.Type)
            {
                case MissionType.OutOfTime:
                    name = $"{string.Format(mission.Name, mission.LeaveTime.ToString() + " PM")}";
                    objective = $"{string.Format(mission.Objective, mission.LeaveTime.ToString() + " PM")}";
                    break;
                case MissionType.SurviveCrewmates:
                    objective = $"{string.Format(mission.Objective, mission.SurviveCrewmates)}";
                    break;
                case MissionType.FindScrap:
                    objective = $"{string.Format(mission.Objective, mission.Item.ItemName)}";
                    break;
                case MissionType.RepairValve:
                    objective = $"{string.Format(mission.Objective, mission.ValveCount)}";
                    break;
            }

            missionItem.AddComponent<MissionItem>().SetMissionInfo(mission.Type, name, objective, mission.Status, sprite);
        }

        /// <summary>
        /// Determines whether the menu can be opened.
        /// </summary>
        /// <returns><c>true</c> if the menu can be opened; otherwise, <c>false</c>.</returns>
        public static bool CanOpenMenu()
        {
            if (QuickMenuManager.isMenuOpen && !isOpen)
                return false;
            if (LocalPlayerController.isPlayerDead || LocalPlayerController.inTerminalMenu || LocalPlayerController.isTypingChat || LocalPlayerController.isPlayerDead || LocalPlayerController.inSpecialInteractAnimation || LocalPlayerController.isGrabbingObjectAnimation || LocalPlayerController.inShockingMinigame || LocalPlayerController.isClimbingLadder || LocalPlayerController.isSinking || LocalPlayerController.inAnimationWithEnemy != null || StartOfRound.Instance.inShipPhase || StartOfRound.Instance.shipIsLeaving || !RoundManager.Instance.dungeonFinishedGeneratingForAllPlayers)
                return false;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "Update")]
        public static void OnUpdate()
        {
            if (StartOfRound.Instance.shipIsLeaving && isOpen)
            {
                CloseMissionsMenu();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "ScrollMouse_performed")]
        public static bool OnScrollMouse(InputAction.CallbackContext context, PlayerControllerB __instance)
        {
            if (!isOpen || __instance != LocalPlayerController || !context.performed)
                return true;


            float scrollAmount = context.ReadValue<float>();
            scrollRect.verticalNormalizedPosition += scrollAmount * 0.3f;


            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "OpenMenu_performed")]
        public static void OnQuickMenu(InputAction.CallbackContext context)
        {
            if (isOpen)
            {
                CloseMissionsMenu();
            }
        }
    }
}