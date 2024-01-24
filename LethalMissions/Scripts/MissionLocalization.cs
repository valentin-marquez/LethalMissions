using System.Collections.Generic;
using System.Linq;
using LethalMissions.Scripts;

namespace LethalMissions.Localization
{
    public class LocalizedMission : Mission
    {
        public string LanguageCode { get; set; }

        public LocalizedMission(MissionType missionType, string missionName, string missionObjective, int reward, int? leaveTime = null, int? surviveCrewmates = null, LevelWeatherType? requiredWeather = LevelWeatherType.None, SimpleItem item = null, int? valveCount = null, string languageCode = "en")
            : base(missionType, missionName, missionObjective, reward, leaveTime, surviveCrewmates, requiredWeather, item, valveCount)
        {
            LanguageCode = languageCode;
        }
    }

    public static class MissionLocalization
    {
        private static readonly string CurrentLanguage = Plugin.Config.LanguageCode.Value;
        private static readonly Dictionary<string, Dictionary<string, string>> MissionStrings = new Dictionary<string, Dictionary<string, string>>
        {
            ["NoMissionsMessage"] = new Dictionary<string, string> { { "en", "There are no missions at the moment..." }, { "es", "No hay misiones en este momento..." } },
            ["CompletedMissionMessage"] = new Dictionary<string, string> { { "en", "Completed Missions:" }, { "es", "Misiones Completadas:" } },
            ["NoCompletedMissionsMessage"] = new Dictionary<string, string> { { "en", "No completed missions" }, { "es", "No hay misiones completadas" } },
            ["CompletedMissionsCountMessage"] = new Dictionary<string, string> {
                { "en", "You have completed {0}\nmissions.\n\nHere are your rewards:\n\n" },
                { "es", "Has completado {0}\nmisiones.\n\nAqui estan tus \nrecompensas:\n\n"}
            },
            ["Name"] = new Dictionary<string, string> { { "en", "Name: " }, { "es", "Nombre: " } },
            ["Objective"] = new Dictionary<string, string> { { "en", "Objective: " }, { "es", "Objetivo: " } },
            ["Status"] = new Dictionary<string, string> { { "en", "Status: " }, { "es", "Estado: " } },
            ["Reward"] = new Dictionary<string, string> { { "en", "Reward: " }, { "es", "Recompensa: " } },
            ["NewMissionsAvailable"] = new Dictionary<string, string> { { "en", "New missions are available!" }, { "es", "Hay nuevas misiones disponibles!" } }
        };
        public static readonly List<LocalizedMission> missions;

        static MissionLocalization()
        {
            var missionDataStore = new MissionDataStore();
            missions = new List<LocalizedMission>();

            foreach (var missionData in missionDataStore.Missions)
            {
                var localizedMission = new LocalizedMission(
                    missionData.Type,
                    missionData.Name,
                    missionData.Objective,
                    missionData.Reward,
                    missionData.LeaveTime,
                    missionData.SurviveCrewmates,
                    missionData.RequiredWeather,
                    missionData.Item,
                    missionData.ValveCount,
                    missionData.LanguageCode
                );

                missions.Add(localizedMission);
            }
        }

        public static string GetMissionString(string key, params object[] args)
        {
            string formatString = MissionStrings[key][CurrentLanguage] ?? MissionStrings[key]["en"];
            return string.Format(formatString, args);
        }

        public static List<Mission> GetLocalizedMissions()
        {
            return missions
                .Where(mission => mission.LanguageCode == CurrentLanguage)
                .Select(mission => new Mission(
                    missionType: mission.Type,
                    missionName: mission.Name,
                    missionObjective: mission.Objective,
                    reward: mission.Reward,
                    leaveTime: mission.LeaveTime,
                    surviveCrewmates: mission.SurviveCrewmates,
                    requiredWeather: mission.RequiredWeather,
                    item: mission.Item,
                    valveCount: mission.ValveCount
                ))
                .ToList();
        }
    }
}