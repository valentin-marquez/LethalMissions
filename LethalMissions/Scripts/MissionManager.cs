using GameNetcodeStuff;
using LethalMissions.DefaultData;
using LethalMissions.Networking;
using LethalMissions.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LethalMissions.Scripts
{
    public enum MissionStatus
    {
        Incomplete,
        Complete
    }

    public enum MissionType
    {
        RecoverCrewmateBody,
        LightningRod,
        WitnessCrewmateDeath,
        ObtainHoneycomb,
        SurviveCrewmates,
        UseRandomTeleporter,
        OutOfTimeLeaveBeforeCertainHour,
        KillMonster,
        ObtainGenerator,
        NoMissions
    }

    public class Mission(string missionName, string missionObjective, MissionType missionType, int reward, int? leaveTime = null, int? surviveCrewmates = null)
    {
        public string Name { get; set; } = missionName;
        public string Objective { get; set; } = missionObjective;
        public MissionStatus Status { get; set; } = MissionStatus.Incomplete;
        public MissionType Type { get; set; } = missionType;
        public int Reward { get; set; } = reward;
        public int? LeaveTime { get; set; } = leaveTime;
        public int? SurviveCrewmates { get; set; } = surviveCrewmates;
    }


    public class MissionManager : MonoBehaviour
    {
        private List<Mission> Allmissions = new List<Mission>();
        private List<Mission> Currentmissions = new List<Mission>();

        public MissionManager()
        {
            Allmissions = DefaultMissions.Missions
                           .Where(m => m.LanguageCode == Plugin.Config.LanguageCode.Value)
                           .Select(m => new Mission(missionName: m.MissionName, missionObjective: m.MissionObjective, missionType: m.MissionType, reward: m.MissionReward))
                           .ToList();

            Plugin.LoggerInstance.LogInfo($"Loaded {Allmissions.Count} missions for language {Plugin.Config.LanguageCode.Value}");
        }
        public static int GenerateRandomLeaveTime()
        {
            return new System.Random().Next(6, 11);
        }
        public static int GenerateRandomSurviveCrewmates()
        {
            int players = FindObjectsOfType<PlayerControllerB>().Count(player => player.isInHangarShipRoom && !player.isPlayerDead);

            Plugin.LoggerInstance.LogInfo($"JUGADORES VIVOS: {players}");
            if (players == 1)
            {
                return 1;
            }
            return (int)(players * 0.75);
        }

        public void GenerateMissions(int n)
        {
            Plugin.LoggerInstance.LogInfo($"Generating {n} missions...");

            if (n > Allmissions.Count)
            {
                n = Allmissions.Count;
                Plugin.LoggerInstance.LogInfo($"Requested number of missions is greater than available missions. Generating all available missions.");
            }

            if (n < 1)
            {
                Plugin.LoggerInstance.LogInfo("No missions to generate.");
                return;
            }

            Currentmissions.Clear();
            Plugin.LoggerInstance.LogInfo("Cleared current missions.");

            // Shuffle Allmissions
            Allmissions.Shuffle();

            Plugin.LoggerInstance.LogInfo("Shuffled all missions.");

            for (int i = 0; i < n; i++)
            {
                var mission = Allmissions[i];
                if (mission.Type == MissionType.OutOfTimeLeaveBeforeCertainHour)
                {
                    mission.LeaveTime = GenerateRandomLeaveTime();
                    Plugin.LoggerInstance.LogInfo($"Generated leave time for mission {mission.Name}: {mission.LeaveTime}");
                }
                else if (mission.Type == MissionType.SurviveCrewmates)
                {
                    mission.SurviveCrewmates = GenerateRandomSurviveCrewmates();
                    Plugin.LoggerInstance.LogInfo($"Generated survive crewmates for mission {mission.Name}: {mission.SurviveCrewmates}");
                }

                Currentmissions.Add(mission);
            }

            SyncMissionsServer();
            Plugin.LoggerInstance.LogInfo("Synced missions.");
        }
        public string ShowMissionOverview()
        {
            StringBuilder missionOverview = new();
            string languageCode = Plugin.Config.LanguageCode.Value;

            missionOverview.AppendLine("LethalMissions\n");

            if (Currentmissions.Count == 0)
            {
                missionOverview.AppendLine(StringUtilities.GetNoMissionsMessage(languageCode));
            }
            else
            {
                foreach (var mission in Currentmissions)
                {

                    // Format the objective if the mission type is OutOfTimeLeaveBeforeCertainHour
                    if (mission.Type == MissionType.OutOfTimeLeaveBeforeCertainHour)
                    {
                        missionOverview.AppendLine($"{StringUtilities.GetName(languageCode)}{string.Format(mission.Name, mission.LeaveTime.ToString() + " PM")}");
                        missionOverview.AppendLine($"{StringUtilities.GetObjective(languageCode)}{string.Format(mission.Objective, mission.LeaveTime.ToString() + " PM")}");
                    }
                    else if (mission.Type == MissionType.SurviveCrewmates)
                    {
                        missionOverview.AppendLine($"{StringUtilities.GetName(languageCode)}{mission.Name}");
                        missionOverview.AppendLine($"{StringUtilities.GetObjective(languageCode)}{string.Format(mission.Objective, mission.SurviveCrewmates)}");
                    }
                    else
                    {
                        missionOverview.AppendLine($"{StringUtilities.GetName(languageCode)}{mission.Name}");
                        missionOverview.AppendLine($"{StringUtilities.GetObjective(languageCode)}{mission.Objective}");
                    }

                    missionOverview.AppendLine($"{StringUtilities.GetStatus(languageCode)}{mission.Status}");
                    missionOverview.AppendLine($"{StringUtilities.GetReward(languageCode)}{mission.Reward}");
                    missionOverview.AppendLine("-----------------------------");
                }
            }

            return missionOverview.ToString();
        }
        public void RemoveActiveMissions()
        {
            Currentmissions.Clear();
            SyncMissionsServer();
            Plugin.LoggerInstance.LogInfo("Cleared all active missions");
        }
        public List<Mission> GetActiveMissions()
        {
            return Currentmissions;
        }
        public void CompleteMission(MissionType missionType)
        {
            var mission = Currentmissions.FirstOrDefault(m => m.Type == missionType);

            if (mission != null)
            {
                mission.Status = MissionStatus.Complete;
                Plugin.LoggerInstance.LogInfo($"Marked mission of type {missionType} as complete");
                SyncMissionsServer();
            }
        }
        public void IncompleteMission(MissionType missionType)
        {
            var mission = Currentmissions.FirstOrDefault(m => m.Type == missionType);

            if (mission != null)
            {
                mission.Status = MissionStatus.Incomplete;
                SyncMissionsServer();
                Plugin.LoggerInstance.LogInfo($"Marked mission of type {missionType} as incomplete");
            }
        }
        private void SyncMissionsServer()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                if (Currentmissions != null)
                {
                    string serializedMissions = JsonConvert.SerializeObject(Currentmissions);
                    Plugin.LoggerInstance.LogInfo($"Syncing missions Host: {serializedMissions}");

                    if (NetworkHandler.Instance != null)
                    {
                        NetworkHandler.Instance.SyncMissionsServerRpc(serializedMissions);
                    }
                    else
                    {
                        Plugin.LoggerInstance.LogError("SyncMissionsServer - NetworkHandler.Instance is null");
                    }
                }
                else
                {
                    Plugin.LoggerInstance.LogError("Currentmissions is null");
                }
            }
        }

        public void RequestMissionsClient()
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                if (NetworkHandler.Instance != null)
                {
                    NetworkHandler.Instance.RequestMissionsServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    Plugin.LoggerInstance.LogError("RequestMissionsClient - NetworkHandler.Instance is null");
                }
            }
        }

        internal void SyncMissions(string serializedMissions)
        {
            Plugin.LoggerInstance.LogInfo($"Syncing missions Client: {serializedMissions}");
            List<Mission> deserializedMissions = JsonConvert.DeserializeObject<List<Mission>>(serializedMissions);

            Currentmissions.Clear();
            Currentmissions.AddRange(deserializedMissions);
        }

        public void RequestMissions()
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                if (NetworkHandler.Instance != null)
                {
                    NetworkHandler.Instance.RequestMissionsServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    Plugin.LoggerInstance.LogError("NetworkHandler.Instance is null");
                }
            }
        }

        public bool IsMissionInCurrentMissions(MissionType type)
        {
            return Currentmissions.Any(m => m.Type == type);
        }

        public int GetMissionLeaveTime(MissionType type)
        {
            return (int)Currentmissions.FirstOrDefault(m => m.Type == type).LeaveTime.Value;
        }

        public int GetCreditsEarned()
        {
            return Currentmissions.Where(m => m.Status == MissionStatus.Complete).Sum(m => m.Reward);
        }

        public List<Mission> GetCompletedMissions()
        {
            return Currentmissions.Where(m => m.Status == MissionStatus.Complete).ToList();
        }

        public int GetSurviveCrewmates()
        {
            return Currentmissions.FirstOrDefault(m => m.Type == MissionType.SurviveCrewmates).SurviveCrewmates.Value;
        }
    }
}