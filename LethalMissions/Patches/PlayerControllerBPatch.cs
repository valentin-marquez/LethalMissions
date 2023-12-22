using GameNetcodeStuff;
using HarmonyLib;
using LethalMissions.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;


namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : NetworkBehaviour
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
        private static void OnKillPlayer(Vector3 bodyVelocity, bool spawnBody = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0)
        {
            Plugin.LoggerInstance.LogWarning($"OnKillPlayerServerRpc called. spawnbody: {spawnBody}, bodyvelocity: {bodyVelocity}, causeofdeath: {causeOfDeath}, deathanimation: {deathAnimation}");

            if (CauseOfDeath.Electrocution == causeOfDeath)
            {
                Plugin.MissionManager.CompleteMission(MissionType.LightningRod);
            }
        }
    }
}