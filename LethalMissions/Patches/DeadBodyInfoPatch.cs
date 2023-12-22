using HarmonyLib;
using LethalMissions.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(DeadBodyInfo))]
    internal class DeadBodyInfoPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DeadBodyInfo.DetectIfSeenByLocalPlayer))]
        private static void OnDetect()
        {
            Plugin.MissionManager.CompleteMission(MissionType.WitnessCrewmateDeath);
        }
    }
}
