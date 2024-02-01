using HarmonyLib;
using UnityEngine;
using LethalMissions.Scripts;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;

namespace LethalMissions.Patches
{
    public class MissionsEvents
    {
        public static int hoursPassed = 0;
        public static int lastHour = 0;

        /// <summary>
        /// Postfix method that is called after the SetClock method in the HUDManager class.
        /// Checks if a specific mission type is active and completes it if the current hour is greater than or equal to the mission leave time.
        /// </summary>
        /// <param name="timeNormalized">The normalized time value.</param>
        /// <param name="numberOfHours">The total number of hours.</param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SetClock))]
        private static void OnSetClock(float timeNormalized, float numberOfHours)
        {
            if (!Plugin.MissionManager.IsMissionActive(MissionType.OutOfTime))
            {
                return;
            }
            int totalMinutes = (int)(timeNormalized * (60f * numberOfHours)) + 360;
            int hour = totalMinutes / 60;

            if (hour != lastHour)
            {
                lastHour = hour;
                hoursPassed++;
            }

            if (hoursPassed < Plugin.MissionManager.GetMissionLeaveTime(MissionType.OutOfTime))
            {
                Plugin.MissionManager.CompleteMission(MissionType.OutOfTime);
            }
            else
            {
                Plugin.MissionManager.IncompleteMission(MissionType.OutOfTime);
            }
        }

        public static void ResetAttr()
        {
            hoursPassed = 0;
            lastHour = 0;
        }


        /// <summary>
        /// Method called after an enemy is killed.
        /// Checks if the "KillMonster" mission is active and completes it if the enemy is dead.
        /// </summary>
        /// <param name="__instance">The instance of the EnemyAI class.</param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
        public static void OnEnemyKilled(EnemyAI __instance)
        {
            if (!Plugin.MissionManager.IsMissionActive(MissionType.KillMonster))
            {
                return;
            }

            if (__instance.isEnemyDead)
            {
                Plugin.MissionManager.CompleteMission(MissionType.KillMonster);
            }
        }

        /// <summary>
        /// Postfix method called after detecting if a dead body is seen by the local player.
        /// Completes the mission of witnessing a crewmate's death if the mission is active.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DeadBodyInfo), nameof(DeadBodyInfo.DetectIfSeenByLocalPlayer))]
        private static void OnDetect()
        {
            if (!Plugin.MissionManager.IsMissionActive(MissionType.WitnessDeath))
            {
                return;
            }
            Plugin.MissionManager.CompleteMission(MissionType.WitnessDeath);
        }


        /// <summary>
        /// Callback method called after the player is killed.
        /// Completes the mission of being killed by a specific cause of death if the mission is active.
        /// </summary>
        /// <param name="causeOfDeath">The cause of death.</param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
        private static void OnKillPlayer(CauseOfDeath causeOfDeath = CauseOfDeath.Unknown)
        {
            if (!Plugin.MissionManager.IsMissionActive(MissionType.LightningRod))
            {
                return;
            }

            if (CauseOfDeath.Electrocution == causeOfDeath)
            {
                Plugin.MissionManager.CompleteMission(MissionType.LightningRod);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetItemInElevator))]
        private static void OnSetItemInElevator(bool droppedInShipRoom, bool droppedInElevator, GrabbableObject gObject)
        {
            if (!droppedInShipRoom)
            {
                return;
            }

            var itemMissionMap = new Dictionary<int, MissionType>
            {
                { 1531, MissionType.ObtainHive },
                { 3, MissionType.ObtainGenerator }
            };

            if (itemMissionMap.TryGetValue(gObject.itemProperties.itemId, out var missionType))
            {
                // Verificar si el objeto tiene el componente ItemExtra
                if (gObject.GetComponent<ItemExtra>() == null)
                {
                    Plugin.MissionManager.CompleteMission(missionType);
                }
            }
            else if (gObject is RagdollGrabbableObject && Plugin.MissionManager.IsMissionActive(MissionType.RecoverBody))
            {
                // Verificar si el objeto tiene el componente ItemExtra
                if (gObject.GetComponent<ItemExtra>() == null)
                {
                    Plugin.MissionManager.CompleteMission(MissionType.RecoverBody);
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.GrabItem))]
        private static void OnGrabItem(GrabbableObject __instance)
        {
            Mission mission = Plugin.MissionManager.GetFindScrapItem();

            if (mission != null && __instance.itemProperties.itemId == mission.Item.ItemId)
            {
                Plugin.MissionManager.CompleteMission(MissionType.FindScrap);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SteamValveHazard), nameof(SteamValveHazard.FixValve))]
        private static void OnFixValve(SteamValveHazard __instance)
        {
            if (!Plugin.MissionManager.IsMissionActive(MissionType.RepairValve))
            {
                return;
            }

            Plugin.MissionManager.ProgressValveRepair();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnEndGame()
        {

            if (Plugin.MissionManager.IsMissionActive(MissionType.SurviveCrewmates))
            {
                PlayerControllerB[] players = StartOfRound.Instance.allPlayerScripts;
                int livingPlayers = players.Count(player => !player.isPlayerDead && (player.isInHangarShipRoom || player.isInElevator));
                int requiredPlayers = Plugin.MissionManager.GetSurviveCrewmates();
                if (livingPlayers >= requiredPlayers)
                {
                    Plugin.MissionManager.CompleteMission(MissionType.SurviveCrewmates);
                }
            }
        }
    }
}