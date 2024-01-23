using System.Collections.Generic;
using LethalMissions.Scripts;

namespace LethalMissions.Localization
{
    public class MissionDataStore
    {
        public List<LocalizedMission> Missions { get; set; }

        public MissionDataStore()
        {
            Missions = new List<LocalizedMission>
            {
            // English||en
            new(
                MissionType.OutOfTime,
                "Escape before {0}",
                "Don't stay on the moon after {0}! It's a dangerous place!",
                reward: Plugin.Config.OutOfTimeReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.LightningRod,
                "Lightning rod",
                "Become the human lightning rod, stop a lightning with style",
                reward: Plugin.Config.LightningRodReward.Value,
                languageCode: "en",
                requiredWeather: LevelWeatherType.Stormy
            ),
            new (
                MissionType.WitnessDeath,
                "Witness a Celestial Event",
                "Witness the dramatic farewell of a fellow crewmate",
                reward: Plugin.Config.WitnessDeathReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.RecoverBody,
                "Retrieve Your Pal's Corpse",
                "Because being a hero also means playing space undertaker",
                reward: Plugin.Config.RecoverBodyReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.ObtainHive,
                "Get a beehive!",
                "Because even in space, we all need some honey and laughter",
                reward: Plugin.Config.ObtainHiveReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.SurviveCrewmates,
                "Preserve the Group's Life",
                "Make sure at least {0} crewmates survive the space odyssey!",
                reward: Plugin.Config.SurviveCrewmatesReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.KillMonster,
                "Defeat the Monster",
                "Embark on a daring mission to eliminate a menacing creature",
                reward: Plugin.Config.KillMonsterReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.ObtainGenerator,
                "Obtaining the Apparatus",
                "Take the Generator from the factory to the ship.",
                reward: Plugin.Config.ObtainGeneratorReward.Value,
                languageCode: "en"
            ),
            new (
                MissionType.FindScrap,
                "Find Scrap",
                "Find the next scrap: {0}",
                reward: Plugin.Config.FindScrapReward.Value,
                languageCode: "en"
            ),
            new(
                MissionType.RepairValve,
                "Repair the Valves",
                "As broken as your heart, show your skills and fix at least {0} of them.",
                reward: Plugin.Config.RepairValveReward.Value,
                languageCode: "en"
            ),

            // Spanish||es
            new (
                MissionType.OutOfTime,
                "Escapa antes de las {0}",
                "¡No te quedes en la luna después de las {0}! ¡Es un lugar peligroso!",
                reward: Plugin.Config.OutOfTimeReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.LightningRod,
                "Atrayendo rayos",
                "Conviértete en el pararrayos humano, detén un rayo con estilo",
                reward: Plugin.Config.LightningRodReward.Value,
                languageCode: "es",
                requiredWeather: LevelWeatherType.Stormy
            ),
            new (
                MissionType.WitnessDeath,
                "Atestigua un suceso celestial",
                "Observa el dramático adiós de un compañero de la nave",
                reward: Plugin.Config.WitnessDeathReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.RecoverBody,
                "Recupera el cuerpo de un amigo",
                "Porque ser un héroe incluye ser también sepulturero espacial",
                reward: Plugin.Config.RecoverBodyReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.ObtainHive,
                "Consigue una colmena",
                "Porque incluso en el espacio, todos necesitamos un poco de miel y risas",
                reward: Plugin.Config.ObtainHiveReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.SurviveCrewmates,
                "Preserva la Vida del Grupo",
                "¡Asegurate de que al menos {0} tripulantes sobrevivan a la odisea espacial!",
                reward: Plugin.Config.SurviveCrewmatesReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.KillMonster,
                "Elimina al Monstruo",
                "Embarcate en una misión audaz para eliminar a una amenazante criatura",
                reward: Plugin.Config.KillMonsterReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.ObtainGenerator,
                "Obteniendo el Aparato",
                "Lleva el generador de la fábrica a la nave.",
                reward: Plugin.Config.ObtainGeneratorReward.Value,
                languageCode: "es"
            ),
            new (MissionType.FindScrap,
                "Encuentra chatarra",
                "Encuentra el siguiente objeto: {0}",
                reward: Plugin.Config.FindScrapReward.Value,
                languageCode: "es"
            ),
            new (
                MissionType.RepairValve,
                "Repara las Válvulas",
                "Tan rota como tu corazón, demuestra tus habilidades y arregla al menos {0} de ellas.",
                reward: Plugin.Config.RepairValveReward.Value,
                languageCode: "es"
            )
        };
        }
    }
}