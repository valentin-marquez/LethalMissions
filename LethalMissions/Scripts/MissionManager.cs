#pragma warning disable CS8632
using GameNetcodeStuff;
using LethalMissions.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using LethalMissions.Scripts;
using LethalMissions.Localization;

namespace LethalMissions.Scripts
{
    public enum MissionStatus
    {
        Incomplete,
        Complete
    }

    public enum MissionType
    {
        RecoverBody,
        LightningRod,
        WitnessDeath,
        ObtainHive,
        SurviveCrewmates,
        OutOfTime,
        KillMonster,
        ObtainGenerator,
        FindScrap,
        RepairValve,

    }
    public class Mission
    {
        public string Name { get; set; }
        public string Objective { get; set; }
        public MissionStatus Status { get; set; }
        public MissionType Type { get; set; }
        public int Reward { get; set; }
        public int Weight { get; set; }
        public int? LeaveTime { get; set; }
        public int? SurviveCrewmates { get; set; }
        public LevelWeatherType? RequiredWeather { get; set; }
        public SimpleItem? Item { get; set; }
        public int? ValveCount { get; set; }

        public Mission(MissionType missionType, string missionName, string missionObjective, int reward, int? leaveTime = null, int? surviveCrewmates = null, LevelWeatherType? requiredWeather = LevelWeatherType.None, SimpleItem item = null, int? valveCount = null)
        {
            Type = missionType;
            Name = missionName;
            Objective = missionObjective;
            Status = MissionStatus.Incomplete;
            Reward = reward;
            Weight = 1;
            LeaveTime = leaveTime;
            SurviveCrewmates = surviveCrewmates;
            RequiredWeather = requiredWeather;
            Item = item;
            ValveCount = valveCount;
        }

    }
    public class MissionManager : MonoBehaviour
    {
        private List<Mission> Allmissions = new List<Mission>();
        private List<Mission> Currentmissions = new List<Mission>();
        private readonly MissionGenerator missionGenerator;

        public MissionManager()
        {
            Allmissions = MissionLocalization.GetLocalizedMissions();
            this.missionGenerator = new MissionGenerator();
        }

        public bool AreMissionsEqual(List<Mission> missions, List<Mission> missions1)
        {

            if (missions == null || missions1 == null)
            {
                return false;
            }

            if (missions.Count != missions1.Count)
            {
                return false;
            }

            for (int i = 0; i < missions.Count; i++)
            {
                if (missions[i].Type != missions1[i].Type ||
                    missions[i].Status != missions1[i].Status ||
                    missions[i].Name != missions1[i].Name ||
                    missions[i].Objective != missions1[i].Objective ||
                    missions[i].Reward != missions1[i].Reward)
                {
                    return false;
                }
            }

            return true;
        }

        public void GenerateMissions(int missionCount)
        {
            Allmissions = MissionLocalization.GetLocalizedMissions();
            if (Allmissions == null)
            {
                Plugin.LogError("No missions available");
                return;
            }

            var allMissionsClone = new List<Mission>(Allmissions);
            if (allMissionsClone == null)
            {
                Plugin.LogError("Failed to clone missions");
                return;
            }

            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {

                if (Plugin.Config.RandomMode.Value)
                {
                    Currentmissions = missionGenerator.GenerateRandomMissions(allMissionsClone);
                }
                else
                {
                    missionCount = Math.Min(missionCount, allMissionsClone.Count);
                    if (missionCount < 0)
                    {
                        Plugin.LogError("Mission count cannot be negative");
                        return;
                    }
                    Currentmissions = missionGenerator.GenerateMissions(missionCount, allMissionsClone);
                }

                SyncMissionsServer();
            }
        }

        public string ShowMissionOverview()
        {
            StringBuilder missionOverview = new StringBuilder();
            missionOverview.AppendLine("Lethal Missions\n");

            if (Currentmissions.Count == 0)
            {
                missionOverview.AppendLine(MissionLocalization.GetMissionString("NoMissionsMessage"));
            }
            else
            {
                foreach (var mission in Currentmissions)
                {
                    if (mission.Type == MissionType.OutOfTime)
                    {
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Name")}{string.Format(mission.Name, mission.LeaveTime.ToString() + " PM")}");
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Objective")}{string.Format(mission.Objective, mission.LeaveTime.ToString() + " PM")}");
                    }
                    else if (mission.Type == MissionType.SurviveCrewmates)
                    {
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Name")}{mission.Name}");
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Objective")}{string.Format(mission.Objective, mission.SurviveCrewmates)}");
                    }
                    else if (mission.Type == MissionType.FindScrap)
                    {
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Name")}{mission.Name}");
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Objective")}{string.Format(mission.Objective, mission.Item.ItemName)}");
                    }
                    else if (mission.Type == MissionType.RepairValve)
                    {
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Name")}{mission.Name}");
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Objective")}{string.Format(mission.Objective, mission.ValveCount)}");
                    }
                    else
                    {
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Name")}{mission.Name}");
                        missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Objective")}{mission.Objective}");
                    }

                    missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Status")}{mission.Status}");
                    missionOverview.AppendLine($"{MissionLocalization.GetMissionString("Reward")}{mission.Reward}");
                    missionOverview.AppendLine("-----------------------------");
                }
            }

            return missionOverview.ToString();
        }
        public void RemoveActiveMissions()
        {
            Currentmissions.Clear();
        }
        public List<Mission> GetActiveMissions()
        {
            return Currentmissions;
        }


        public void CompleteMission(MissionType missionType)
        {
            var mission = Currentmissions.FirstOrDefault(m => m.Type == missionType);

            if (mission != null && mission.Status != MissionStatus.Complete)
            {
                mission.Status = MissionStatus.Complete;
                SyncMissionsServer();
            }
        }

        public void IncompleteMission(MissionType missionType)
        {
            var mission = Currentmissions.FirstOrDefault(m => m.Type == missionType);

            if (mission != null && mission.Status != MissionStatus.Incomplete)
            {
                mission.Status = MissionStatus.Incomplete;
                SyncMissionsServer();
            }
        }

        private void SyncMissionsServer()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                if (Currentmissions != null && Currentmissions.Count > 0)
                {
                    string serializedMissions = JsonConvert.SerializeObject(Currentmissions);

                    if (NetworkHandler.Instance != null)
                    {
                        NetworkHandler.Instance.SyncMissionsServerRpc(serializedMissions);
                    }
                    else
                    {
                        Plugin.LogError("SyncMissionsServer - NetworkHandler.Instance is null");
                    }
                }
                else
                {
                    Plugin.LogError("Currentmissions is null or empty, unable to sync missions.");
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
                    Plugin.LogError("RequestMissionsClient - NetworkHandler.Instance is null");
                }
            }
        }

        internal void SyncMissions(string serializedMissions)
        {
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
                    Plugin.LogError("NetworkHandler.Instance is null");
                }
            }
        }

        public bool IsMissionActive(MissionType type)
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
        public Mission GetFindScrapItem()
        {
            return Currentmissions.FirstOrDefault(m => m.Type == MissionType.FindScrap);
        }

        public void ProgressValveRepair()
        {
            var mission = Currentmissions.FirstOrDefault(m => m.Type == MissionType.RepairValve);
            if (mission != null && mission.ValveCount > 0)
            {
                mission.ValveCount--;
                if (mission.ValveCount == 0)
                {
                    CompleteMission(MissionType.RepairValve);
                }
                SyncMissionsServer();
            }
        }
    }
}