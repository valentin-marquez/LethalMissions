using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using LethalMissions.Scripts;
using BepInEx.Configuration;
using System.IO;


namespace LethalMissions
{


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi", MinimumDependencyVersion: "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        private const string ConfigFileName = "LethalMissions.cfg";

        public static ManualLogSource LoggerInstance { get; private set; }
        public static new Configuration Config { get; private set; }
        public static MissionManager MissionManager { get; private set; }


        private void Awake()
        {
            LoggerInstance = Logger;

            LoggerInstance.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            ConfigFile configFile = new(Path.Combine(Paths.ConfigPath, ConfigFileName), true);
            Config = new Configuration(configFile);
            LoggerInstance.LogInfo($"Loading missions for language: {Config.LanguageCode.Value}");
            MissionManager = new MissionManager();


            LoggerInstance.LogInfo("Installing patches...");
            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches.DeadBodyInfoPatch));
            harmony.PatchAll(typeof(Patches.EnemyAIPatch));
            harmony.PatchAll(typeof(Patches.HUDManagerPatch));
            harmony.PatchAll(typeof(Patches.PlayerControllerBPatch));
            harmony.PatchAll(typeof(Patches.RoundManagerPatch));
            harmony.PatchAll(typeof(Patches.StartOfRoundPatch));
            harmony.PatchAll(typeof(Patches.NetworkObjectManager));

            LoggerInstance.LogInfo("Loading assets...");
            Assets.PopulateAssets("LethalMissions.asset");

            LoggerInstance.LogInfo("Registering commands...");
            DetermineCommandLibrary();

            LoggerInstance.LogInfo("Patching with netcode...");
            NetcodeWeaver();

            LoggerInstance.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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

            if (CheckDependency("atomic.terminalapi"))
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
        /// Checks if a plugin with the specified GUID is loaded.
        /// </summary>
        /// <param name="PLUGIN_GUID">The GUID of the plugin to check.</param>
        /// <returns>True if the plugin is loaded, false otherwise.</returns>
        public bool CheckDependency(string PLUGIN_GUID)
        {
            LoggerInstance.LogInfo($"Checking dependency {PLUGIN_GUID}");
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID) && BepInEx.Bootstrap.Chainloader.PluginInfos[PLUGIN_GUID].Instance != null;
        }

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
