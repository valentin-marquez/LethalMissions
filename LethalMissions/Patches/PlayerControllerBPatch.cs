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


        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
        private static void OnConnectClientToPlayerObject()
        {
            if (!NetworkManager.Singleton.IsHost || !NetworkManager.Singleton.IsServer)
            {
                Plugin.LoggerInstance.LogWarning($"OnConnectClientToPlayerObject called.");
                Plugin.MissionManager.RequestMissions();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerControllerB.SetItemInElevator))]
        private static void OnSetItemInElevator(bool droppedInShipRoom, bool droppedInElevator, GrabbableObject gObject)
        {
            if (gObject.itemProperties.itemId == 1531 && droppedInShipRoom)
            {
                Plugin.MissionManager.CompleteMission(MissionType.ObtainHoneycomb);
            }
            else if (gObject.itemProperties.itemId == 3 && droppedInShipRoom)
            {
                Plugin.MissionManager.CompleteMission(MissionType.ObtainGenerator);
            }
            else if (gObject is RagdollGrabbableObject && droppedInShipRoom)
            {
                Plugin.MissionManager.CompleteMission(MissionType.RecoverCrewmateBody);
            }
        }
    }
}