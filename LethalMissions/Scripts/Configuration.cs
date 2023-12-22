using BepInEx;
using BepInEx.Configuration;
using LethalMissions.DefaultData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LethalMissions.Scripts
{
    public class Configuration
    {
        public ConfigEntry<string> LanguageCode { get; set; }
        public ConfigEntry<int> MaxMissions { get; set; }
        public string TranslationFolderPath { get; set; }

        public Configuration(ConfigFile config)
        {
            LanguageCode = config.Bind("General", "LanguageCode", "en", "The language code for translations (e.g., en for English, es for Spanish).");
            MaxMissions = config.Bind("General", "MaxMissions", 2, "The maximum number of moon missions to start.");
        }
    }
}
