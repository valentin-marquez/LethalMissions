using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalMissions.DefaultData
{
    public static class DefaultMissions
    {
        public static List<DefaultMissionData> Missions { get; private set; }

        static DefaultMissions()
        {
            Missions = new List<DefaultMissionData>
        {
            // Missions in english
            new(Scripts.MissionType.OutOfTimeLeaveBeforeCertainHour, "Escape before {0}", "Don't stay on the moon after {0}! It's a dangerous place!", "en", 30),
            new(Scripts.MissionType.LightningRod, "Attracting Lightning", "Become the human lightning rod, stop a lightning with style", "en", 100),
            new(Scripts.MissionType.WitnessCrewmateDeath, "Witness a Celestial Event", "Witness the dramatic farewell of a fellow crewmate", "en", 40),
            new(Scripts.MissionType.RecoverCrewmateBody, "Retrieve Your Pal's Corpse", "Because being a hero also means playing space undertaker", "en", 20),
            new(Scripts.MissionType.ObtainHoneycomb, "Get a beehive!", "Because even in space, we all need some honey and laughter", "en", 80),
            new(Scripts.MissionType.KillMonster, "Defeat the Monster", "Embark on a daring mission to eliminate a menacing creature", "en", 50),

            // Misiones en español
            new(Scripts.MissionType.OutOfTimeLeaveBeforeCertainHour, "Escapa antes de las {0}", "¡No te quedes en la luna después de las {0}! ¡Es un lugar peligroso!", "es", 30),
            new(Scripts.MissionType.LightningRod, "Atrayendo rayos", "Conviértete en el pararrayos humano, detén un rayo con estilo", "es", 100),
            new(Scripts.MissionType.WitnessCrewmateDeath, "Atestigua un suceso celestial", "Observa el dramático adiós de un compañero de la nave", "es", 40),
            new(Scripts.MissionType.RecoverCrewmateBody, "Recupera el cuerpo de un amigo", "Porque ser un héroe incluye ser también sepulturero espacial", "es", 20),
            new(Scripts.MissionType.ObtainHoneycomb, "Consigue una colmena", "Porque incluso en el espacio, todos necesitamos un poco de miel y risas", "es", 80),
            new(Scripts.MissionType.SurviveCrewmates, "Preserva la Vida del Grupo", "¡Asegúrate de que al menos {0} tripulantes sobrevivan a la odisea espacial!", "es", 100),
            new(Scripts.MissionType.KillMonster, "Elimina al Monstruo", "Embarcate en una misión audaz para eliminar a una amenazante criatura", "es", 50),


        };
        }
    }

    public class DefaultMissionData
    {
        public Scripts.MissionType MissionType { get; }
        public string MissionName { get; }
        public string MissionObjective { get; }
        public int MissionReward { get; }
        public string LanguageCode { get; }

        public DefaultMissionData(Scripts.MissionType missionType, string missionName, string missionObjective, string languageCode, int missionReward)
        {
            MissionType = missionType;
            MissionName = missionName;
            MissionObjective = missionObjective;
            LanguageCode = languageCode;
            MissionReward = missionReward;
        }
    }

    public static class StringUtilities
    {
        public static string NoMissionsMessageEnglish { get; private set; }
        public static string NoMissionsMessageSpanish { get; private set; }
        public static string CompletedMissionMessageEnglish { get; private set; }
        public static string CompletedMissionMessageSpanish { get; private set; }
        public static string NoCompletedMissionsMessageEnglish { get; private set; } = "No completed missions";
        public static string NoCompletedMissionsMessageSpanish { get; private set; } = "No hay misiones completadas";
        public static string CompletedMissionsCountMessageEnglish { get; private set; } = "You have completed {0} missions.\n\nHere are your rewards:\n\n";
        public static string CompletedMissionsCountMessageSpanish { get; private set; } = "Has completado {0} misiones.\n\nAqui estan tus recompensas:\n\n";
        public static string NameEnglish { get; private set; } = "Name: ";
        public static string NameSpanish { get; private set; } = "Nombre: ";
        public static string ObjectiveEnglish { get; private set; } = "Objective: ";
        public static string ObjectiveSpanish { get; private set; } = "Objetivo: ";
        public static string StatusEnglish { get; private set; } = "Status: ";
        public static string StatusSpanish { get; private set; } = "Estado: ";
        public static string RewardEnglish { get; private set; } = "Reward: ";
        public static string RewardSpanish { get; private set; } = "Recompensa: ";

        public static string GetName(string languageCode)
        {
            return languageCode switch
            {
                "en" => NameEnglish,
                "es" => NameSpanish,
                _ => NameEnglish,
            };
        }

        public static string GetObjective(string languageCode)
        {
            return languageCode switch
            {
                "en" => ObjectiveEnglish,
                "es" => ObjectiveSpanish,
                _ => ObjectiveEnglish,
            };
        }

        public static string GetStatus(string languageCode)
        {
            return languageCode switch
            {
                "en" => StatusEnglish,
                "es" => StatusSpanish,
                _ => StatusEnglish,
            };
        }

        public static string GetReward(string languageCode)
        {
            return languageCode switch
            {
                "en" => RewardEnglish,
                "es" => RewardSpanish,
                _ => RewardEnglish,
            };
        }

        static StringUtilities()
        {
            NoMissionsMessageEnglish = "There are no missions at the moment, the mission system only works within the range of the factories, sorry :']\n\n";
            NoMissionsMessageSpanish = "No hay misiones en este momento, el sistema de misiones solo funciona dentro del rango de las fábricas, lo siento :']\n\n";
            CompletedMissionMessageEnglish = "Misiones Completadas:\n";
            CompletedMissionMessageSpanish = "Misiones Completadas:\n";
        }

        public static string GetNoMissionsMessage(string languageCode)
        {
            return languageCode switch
            {
                "en" => StringUtilities.NoMissionsMessageEnglish,
                "es" => StringUtilities.NoMissionsMessageSpanish,
                _ => StringUtilities.NoMissionsMessageEnglish,
            };
        }
        public static string GetCompletedMissionsCountMessage(string languageCode, int count)
        {
            return languageCode switch
            {
                "en" => string.Format(StringUtilities.CompletedMissionsCountMessageEnglish, count),
                "es" => string.Format(StringUtilities.CompletedMissionsCountMessageSpanish, count),
                _ => string.Format(StringUtilities.CompletedMissionsCountMessageEnglish, count),
            };
        }

        public static string GetCompletedMissionMessage(string languageCode)
        {
            return languageCode switch
            {
                "en" => StringUtilities.CompletedMissionMessageEnglish,
                "es" => StringUtilities.CompletedMissionMessageSpanish,
                _ => StringUtilities.CompletedMissionMessageEnglish,
            };
        }

        public static string GetNoCompletedMissionsMessage(string languageCode)
        {
            return languageCode switch
            {
                "en" => StringUtilities.NoCompletedMissionsMessageEnglish,
                "es" => StringUtilities.NoCompletedMissionsMessageSpanish,
                _ => StringUtilities.NoCompletedMissionsMessageEnglish,
            };
        }
    }
}
