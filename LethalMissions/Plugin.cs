using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using LethalMissions.Scripts;
using BepInEx.Configuration;
using System.IO;
using LethalMissions.Patches;


namespace LethalMissions
{


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi", MinimumDependencyVersion: "1.5.0")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", MinimumDependencyVersion: "0.4.4")]
    public class Plugin : BaseUnityPlugin
    {
        private const string ConfigFileName = "LethalMissions.cfg";

        public static ManualLogSource LoggerInstance { get; private set; }
        public static new Configuration Config { get; private set; }
        public static MissionManager MissionManager { get; private set; }
        public static MenuManager MissionMenuManager { get; private set; }
        public static GameObject MissionsMenuPrefab;
        public static GameObject missionItemPrefab;

        private void Awake()
        {
            LoggerInstance = Logger;

            LoggerInstance.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            ConfigFile configFile = new(Path.Combine(Paths.ConfigPath, ConfigFileName), true);
            Config = new Configuration(configFile);
            LoggerInstance.LogInfo($"Loading missions for language: {Config.LanguageCode.Value}");
            MissionManager = new MissionManager();
            MissionMenuManager = new MenuManager();
            Keybinds.Initialize();



            LoggerInstance.LogInfo("Installing patches...");
            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(MissionsEvents));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(NetworkObjectManager));
            harmony.PatchAll(typeof(Keybinds));
            harmony.PatchAll(typeof(MenuManager));

            LoggerInstance.LogInfo("Loading assets...");
            Assets.PopulateAssets("LethalMissions.asset");
            LoadMissionsMenuAsset();
            LoadMissionItemAsset();

            LoggerInstance.LogInfo("Registering commands...");
            DetermineCommandLibrary();

            LoggerInstance.LogInfo("Patching with netcode...");
            NetcodeWeaver();

            LoggerInstance.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }


        public static void LoadMissionsMenuAsset()
        {
            try
            {
                MissionsMenuPrefab = Assets.MainAssetBundle.LoadAsset<GameObject>("LethalMissionsMenu");
                Plugin.LoggerInstance.LogInfo("MissionsMenu asset loaded");
            }
            catch
            {
                Plugin.LoggerInstance.LogError("Failed to load MissionsMenu asset");
            }
        }

        public static void LoadMissionItemAsset()
        {
            try
            {
                missionItemPrefab = Assets.MainAssetBundle.LoadAsset<GameObject>("MissionItem");
                Plugin.LoggerInstance.LogInfo("MissionItem asset loaded");
            }
            catch
            {
                Plugin.LoggerInstance.LogError("Failed to load MissionItem asset");
            }
        }

        public void DetermineCommandLibrary()
        {
            string language = Config.LanguageCode.Value;
            string command = (language == "es") ? "misiones" : "missions";

            if (language != "en" && language != "es")
            {
                Logger.LogWarning("Language not supported, using english");
                command = "missions";
            }

            if (IsModLoaded("atomic.terminalapi"))
            {
                LoggerInstance.LogWarning("Using atomic.terminalapi for commands");

                TerminalApi.TerminalApi.AddCommand(command, new TerminalApi.Classes.CommandInfo()
                {
                    DisplayTextSupplier = () => { return MissionManager.ShowMissionOverview(); },
                    Description = (language == "es") ? "Muestra todas las Misiones Letales" : "Show all Lethal Missions",
                    Category = "Other"
                }, null, true);

            }
            else
            {
                LoggerInstance.LogFatal("No command library found, please install atomic.terminalapi");
            }
        }


        /// <summary>
        /// Checks if a mod with the specified GUID is loaded.
        /// </summary>
        /// <param name="modGUID">The GUID of the mod to check.</param>
        /// <returns>True if the mod is loaded, false otherwise.</returns>
        public static bool IsModLoaded(string guid) => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);

        /// <summary>
        /// Patching the RuntimeInitializeOnLoadMethodAttribute with netcodeweaver
        /// </summary>
        private void NetcodeWeaver()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
