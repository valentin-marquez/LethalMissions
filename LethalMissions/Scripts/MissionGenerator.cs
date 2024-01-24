using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace LethalMissions.Scripts
{
    public class MissionGenerator
    {
        private readonly List<Mission> allMissions;
        private readonly System.Random random;


        public MissionGenerator(List<Mission> missions)
        {
            this.allMissions = missions;
            this.random = new System.Random();
        }

        public List<Mission> GenerateMissions(int n)
        {

            if (!Plugin.Config.RandomMode.Value)
            {
                ValidateNumberOfMissions(n);
            }
            var availableMissions = GetAvailableMissions();
            if (Plugin.Config.RandomMode.Value)
            {
                n = random.Next(1, availableMissions.Count + 1);
            }
            var generatedMissions = GenerateUniqueMissions(n, availableMissions);
            SetMissionSpecificProperties(generatedMissions);

            return generatedMissions;
        }

        private void ValidateNumberOfMissions(int n)
        {
            if (n > allMissions.Count)
            {
                n = allMissions.Count;
                Plugin.LoggerInstance.LogWarning($"Requested number of missions is greater than available missions. Generating all available missions.");
            }

            if (n < 1)
            {
                Plugin.LoggerInstance.LogWarning("No missions to generate.");
            }
        }

        private List<Mission> GetAvailableMissions()
        {
            var availableMissions = new List<Mission>(allMissions);
            var currentWeather = RoundManager.Instance.currentLevel.currentWeather;
            var isHiveInLevel = CheckItemInLevel(1531);
            var isApparatusInLevel = CheckItemInLevel(3);
            var areThereValves = AreThereValves();

            // Increase weight for certain missions if conditions are met
            foreach (var mission in availableMissions)
            {
                if (mission.Type == MissionType.ObtainHive && isHiveInLevel)
                {
                    mission.Weight *= 3;
                }
                if (mission.Type == MissionType.ObtainGenerator && isApparatusInLevel)
                {
                    mission.Weight *= 2;
                }
                if (mission.Type == MissionType.RepairValve && areThereValves)
                {
                    mission.Weight *= 2;
                }
            }

            availableMissions.RemoveAll(mission => mission.Type == MissionType.OutOfTime); // Handle OutOfTime mission separately
            availableMissions.RemoveAll(mission => mission.RequiredWeather.HasValue && mission.RequiredWeather.Value != currentWeather);
            availableMissions.RemoveAll(mission => mission.Type == MissionType.ObtainHive && !isHiveInLevel); // Remove ObtainHive mission if there is no hive in the level
            availableMissions.RemoveAll(mission => mission.Type == MissionType.ObtainGenerator && !isApparatusInLevel); // Remove ObtainApparatus mission if there is no apparatus in the level
            availableMissions.RemoveAll(mission => mission.Type == MissionType.RepairValve && !areThereValves); // Remove RepairValve mission if there are no valves in the level
            availableMissions.RemoveAll(mission => mission.Type == MissionType.RecoverBody); // this mission only added when a crewmate dies

            return availableMissions;
        }

        private List<Mission> GenerateUniqueMissions(int n, List<Mission> availableMissions)
        {
            availableMissions.Shuffle();
            var generatedMissions = new List<Mission>();

            for (int i = 0; i < n; i++)
            {
                var mission = ChooseMissionWithWeight(availableMissions);
                generatedMissions.Add(mission);
                availableMissions.Remove(mission);
            }

            return generatedMissions;
        }

        private Mission ChooseMissionWithWeight(List<Mission> availableMissions)
        {
            var totalWeight = availableMissions.Sum(mission => mission.Weight);
            var randomValue = random.Next(totalWeight);
            var currentWeight = 0;

            foreach (var mission in availableMissions)
            {
                currentWeight += mission.Weight;
                if (randomValue < currentWeight)
                {
                    return mission;
                }
            }

            return availableMissions.Last();
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
                else if (mission.Type == MissionType.RepairValve)
                {
                    SteamValveHazard[] allValves = UnityEngine.Object.FindObjectsOfType<SteamValveHazard>();
                    int N = allValves.Length == 1 ? 1 : random.Next(1, allValves.Length + 1);
                    for (int i = 0; i < N; i++)
                    {
                        allValves[i].valveCrackTime = 0.001f;
                        allValves[i].valveBurstTime = 0.01f;
                        allValves[i].triggerScript.interactable = true;
                    }

                    mission.ValveCount = N;
                }
            }
        }

        private bool AreThereValves()
        {
            var allValves = UnityEngine.Object.FindObjectsOfType<SteamValveHazard>();
            return allValves.Any();
        }
        private bool CheckItemInLevel(int itemId)
        {
            return UnityEngine.Object.FindObjectsByType<GrabbableObject>(UnityEngine.FindObjectsSortMode.None).FirstOrDefault(grabbable => grabbable.itemProperties.itemId == itemId);
        }

        private int GenerateRandomLeaveTime()
        {
            return random.Next(6, 11);
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