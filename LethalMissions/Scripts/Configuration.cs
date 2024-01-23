using BepInEx.Configuration;
namespace LethalMissions.Scripts
{
    public class Configuration
    {
        // General
        public ConfigEntry<string> LanguageCode { get; set; }
        public ConfigEntry<int> MaxMissions { get; set; }
        public ConfigEntry<bool> NewMissionNotify { get; set; }
        public ConfigEntry<bool> PlaySoundOnly { get; set; }

        // Rewards
        public ConfigEntry<int> RecoverBodyReward { get; set; }
        public ConfigEntry<int> LightningRodReward { get; set; }
        public ConfigEntry<int> WitnessDeathReward { get; set; }
        public ConfigEntry<int> ObtainHiveReward { get; set; }
        public ConfigEntry<int> SurviveCrewmatesReward { get; set; }
        public ConfigEntry<int> OutOfTimeReward { get; set; }
        public ConfigEntry<int> KillMonsterReward { get; set; }
        public ConfigEntry<int> ObtainGeneratorReward { get; set; }
        public ConfigEntry<int> FindScrapReward { get; set; }
        public ConfigEntry<int> RepairValveReward { get; set; }

        public Configuration(ConfigFile config)
        {
            LanguageCode = config.Bind("General", "LanguageCode", "en", "The language code for translations (e.g., en for English, es for Spanish).");
            MaxMissions = config.Bind("General", "MaxMissions", 2, "The maximum number of moon missions to start.");
            NewMissionNotify = config.Bind("General", "NewMissionNotify", true, "Display a notification banner when new missions are available.");
            PlaySoundOnly = config.Bind("General", "PlaySoundOnly", false, "Play a sound when new missions are available, but do not display a notification banner.");

            RecoverBodyReward = config.Bind("Rewards", "RecoverBody", 20, "The reward for completing a Recover Body mission.");
            LightningRodReward = config.Bind("Rewards", "LightningRod", 300, "The reward for completing a Lightning Rod mission.");
            WitnessDeathReward = config.Bind("Rewards", "WitnessDeath", 40, "The reward for completing a Witness Death mission.");
            ObtainHiveReward = config.Bind("Rewards", "ObtainHive", 80, "The reward for completing an Obtain Hive mission.");
            SurviveCrewmatesReward = config.Bind("Rewards", "SurviveCrewmates", 100, "The reward for completing a Survive Crewmates mission.");
            OutOfTimeReward = config.Bind("Rewards", "OutOfTime", 30, "The reward for completing an Out of Time mission.");
            KillMonsterReward = config.Bind("Rewards", "KillMonster", 50, "The reward for completing a Kill Monster mission.");
            ObtainGeneratorReward = config.Bind("Rewards", "ObtainGenerator", 50, "The reward for completing an Obtain Generator mission.");
            FindScrapReward = config.Bind("Rewards", "FindScrap", 50, "The reward for completing a Find Scrap mission.");
            RepairValveReward = config.Bind("Rewards", "RepairValve", 80, "The reward for completing a Repair Valve mission.");

        }
    }
}
