﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using LethalMissions.Scripts;
using BepInEx.Configuration;
using System.IO;
using LethalAPI.TerminalCommands.Models;

namespace LethalMissions
{


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string ConfigFileName = "LethalMissions.cfg";

        public static ManualLogSource LoggerInstance { get; private set; }
        public static new Configuration Config { get; private set; }
        private TerminalModRegistry commands;
        public static MissionManager MissionManager { get; private set; }

        private void Awake()
        {
            LoggerInstance = Logger;

            LoggerInstance.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            // Configuración
            ConfigFile configFile = new(Path.Combine(Paths.ConfigPath, ConfigFileName), true);
            Config = new Configuration(configFile);
            //load language
            LoggerInstance.LogInfo($"Loading missions for language: {Config.LanguageCode.Value}");

            // load missions
            MissionManager = new MissionManager();


            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches.DeadBodyInfoPatch));
            harmony.PatchAll(typeof(Patches.EnemyAIPatch));
            harmony.PatchAll(typeof(Patches.HUDManagerPatch));
            harmony.PatchAll(typeof(Patches.PlayerControllerBPatch));
            harmony.PatchAll(typeof(Patches.RoundManagerPatch));
            harmony.PatchAll(typeof(Patches.StartOfRoundPatch));
            harmony.PatchAll(typeof(Networking.NetworkObjectManager));

            Assets.PopulateAssets("LethalMissions.asset");
            // create a missions command for show in the terminal 
            if (Config.LanguageCode.Value == "en")
            {
                commands = TerminalRegistry.RegisterFrom(new EnglishTerminalCommands());
            }
            else if (Config.LanguageCode.Value == "es")
            {
                commands = TerminalRegistry.RegisterFrom(new SpanishTerminalCommands());
            }

            NetcodeWeaver();
        }
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
