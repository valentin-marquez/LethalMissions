using System;
using System.Collections.Generic;
using System.Linq;
using LethalMissions.Networking;
using LethalMissions.Patches;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LethalMissions.Scripts
{
    public class MissionGenerator
    {
        private readonly System.Random random;

        public MissionGenerator()
        {
            this.random = new System.Random();
        }

        public List<Mission> GenerateRandomMissions(int missionCount, List<Mission> allMissionsClone)
        {
            missionCount = ValidateMissionCount(missionCount, allMissionsClone);
            var generatedMissions = ChooseRandomMissions(missionCount, allMissionsClone);
            SetMissionSpecificProperties(generatedMissions);

            foreach (var mission in generatedMissions)
            {
                Plugin.LogInfo($"Generated Mission: {mission.Name}, Type: {mission.Type}, Objective: {mission.Objective}");
            }

            return generatedMissions;
        }

        private int ValidateMissionCount(int n, List<Mission> availableMissions)
        {
            if (n > availableMissions.Count || Plugin.Config.RandomMode.Value)
            {
                return availableMissions.Count;
            }
            return n;
        }

        private List<Mission> ChooseRandomMissions(int n, List<Mission> availableMissions)
        {
            var selectedMissions = new List<Mission>();

            for (int i = 0; i < n; i++)
            {
                if (!availableMissions.Any())
                {
                    Plugin.LogError("No available missions to choose from.");
                    break;
                }

                var randomIndex = random.Next(availableMissions.Count);
                var mission = availableMissions[randomIndex];

                if (mission.Type == MissionType.RepairValve && !MapHasValves())
                {
                    Plugin.LogInfo("Skipped RepairValve mission as there are no valves on the map.");
                    availableMissions.RemoveAt(randomIndex);
                    continue;
                }
                else if (mission.Type == MissionType.LightningRod && StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.Stormy)
                {
                    Plugin.LogInfo("Skipped LightningRod mission as the weather is not stormy.");
                    availableMissions.RemoveAt(randomIndex);
                    continue;
                }
                else if (mission.Type == MissionType.ObtainGenerator && !MapHasApparatus())
                {
                    Plugin.LogInfo("Skipped ObtainGenerator mission as there is no apparatus on the map.");
                    availableMissions.RemoveAt(randomIndex);
                    continue;
                }

                selectedMissions.Add(mission);
                availableMissions.RemoveAt(randomIndex);
            }

            return selectedMissions;
        }

        private void SetMissionSpecificProperties(List<Mission> generatedMissions)
        {
            foreach (var mission in generatedMissions)
            {
                if (mission.Type == MissionType.OutOfTime)
                {
                    mission.LeaveTime = GenerateRandomLeaveTime();
                }
                else if (mission.Type == MissionType.SurviveCrewmates)
                {
                    mission.SurviveCrewmates = GenerateRandomSurviveCrewmates();
                }
                else if (mission.Type == MissionType.FindScrap)
                {
                    mission.Item = ObtainRandomScrap();
                }
                else if (mission.Type == MissionType.ObtainHive)
                {
                    SetObtainHiveMissionProperties(mission);
                }
                else if (mission.Type == MissionType.RepairValve)
                {
                    SetRepairValveMissionProperties(mission);
                }
            }
        }

        private void SetObtainHiveMissionProperties(Mission mission)
        {
            GameObject shipObj = StartOfRound.Instance.shipAnimator.gameObject;

            if (EnemyTypeInitializer.OutsideEnemies.TryGetValue(typeof(RedLocustBees), out EnemyType enemyType))
            {
                GameObject[] outsideAINodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
                Vector3 spawnLocation = outsideAINodes[Random.Range(0, outsideAINodes.Length)].transform.position;
                GameObject enemy = GameObject.Instantiate(enemyType.enemyPrefab, spawnLocation, Quaternion.identity);
                Plugin.LogInfo($"Enemy spawned at {spawnLocation}");
                RedLocustBees enemyAI = enemy.GetComponent<RedLocustBees>();

                enemyAI.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
            }
        }

        private void SetRepairValveMissionProperties(Mission mission)
        {
            SteamValveHazard[] allValves = UnityEngine.Object.FindObjectsOfType<SteamValveHazard>();

            Array.Sort(allValves, (valve1, valve2) => valve1.name.CompareTo(valve2.name));

            int N = allValves.Length == 1 ? 1 : random.Next(1, allValves.Length + 1);
            mission.ValveCount = N;
        }

        private int GenerateRandomLeaveTime()
        {
            return random.Next(12, 17);
        }

        private bool MapHasValves()
        {
            return UnityEngine.Object.FindObjectsOfType<SteamValveHazard>().Length > 0;
        }

        private bool MapHasApparatus()
        {
            return UnityEngine.Object.FindObjectsOfType<GrabbableObject>().Any(grabbable => grabbable.itemProperties.itemId == 3);
        }

        private int GenerateRandomSurviveCrewmates()
        {
            int players = NetworkManager.Singleton.ConnectedClientsIds.Count;

            if (players == 1)
            {
                return 1;
            }
            return (int)(players * 0.6);
        }

        private SimpleItem ObtainRandomScrap()
        {
            GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>().Where(grabbable => grabbable.itemProperties.isScrap && grabbable.isInFactory && !grabbable.isInElevator && grabbable.itemProperties.itemId != 3).ToArray();
            var randomItem = array[random.Next(array.Length)].itemProperties;
            SimpleItem ScrapSelected = new(randomItem.itemId, randomItem.itemName);
            return ScrapSelected;
        }
    }
}
