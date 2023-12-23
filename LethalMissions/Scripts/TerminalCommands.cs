using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LethalAPI.TerminalCommands.Attributes;
using LethalAPI.TerminalCommands.Models;

namespace LethalMissions.Scripts
{
    public interface ITerminalCommands
    {
        string MissionsCommand();
    }
    internal class EnglishTerminalCommands : ITerminalCommands
    {
        [TerminalCommand("Missions", true), CommandInfo("Show all Lethal Missions")]
        public string MissionsCommand()
        {
            return Plugin.MissionManager.ShowMissionOverview();
        }
    }

    internal class SpanishTerminalCommands : ITerminalCommands
    {
        [TerminalCommand("Misiones", true), CommandInfo("Muestra todas las Misiones Letales")]
        public string MissionsCommand()
        {
            return Plugin.MissionManager.ShowMissionOverview();
        }
    }
}
