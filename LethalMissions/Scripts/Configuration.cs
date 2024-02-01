using BepInEx.Configuration;
namespace LethalMissions.Scripts
{
    public enum NotificationOption
    {
        None,
        SoundOnly,
        SoundAndBanner,
        BannerOnly
    }
    public class Configuration
    {
        // General
        public ConfigEntry<string> LanguageCode { get; set; }
        public ConfigEntry<int> NumberOfMissions { get; set; }
        public ConfigEntry<NotificationOption> MissionsNotification { get; set; }
        public ConfigEntry<bool> RandomMode { get; set; }

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
            // general
            LanguageCode = config.Bind("General", "LanguageCode", "en", "The language code for translations (e.g., en for English, es for Spanish).");
            NumberOfMissions = config.Bind("General", "NumberOfMissions", 3, "The maximum number of missions to start per map. Recommended is 3-4. More than this may cause errors. Total missions are 10 but 3 are generated based on map conditions.");
            MissionsNotification = config.Bind("General", "NotificationOption", NotificationOption.SoundAndBanner, "The option for new mission notifications. Options: None, SoundOnly, SoundAndBanner, BannerOnly.");
            RandomMode = config.Bind("General", "RandomMode", false, "Generate random number of missions.");

            // rewards
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
