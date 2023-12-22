using LethalAPI.TerminalCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LethalAPI.TerminalCommands.Models;

namespace LethalMissions.Scripts
{
    internal class TerminalCommands
    {
        [TerminalCommand("Missions", true), CommandInfo("Show all Lethal Missions")]
        public string MissionsCommand()
        {
            return Plugin.MissionManager.ShowMissionOverview();
        }
    }
}
