using HarmonyLib;
using LethalMissions.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using LC_API;
using UnityEngine.Rendering;
using UnityEngine;
using System.Numerics;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using GameNetcodeStuff;
using LethalMissions.DefaultData;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    public class RoundManagerPatch : NetworkBehaviour
    {

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void onAwake()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Plugin.MissionManager.RemoveActiveMissions();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(RoundManager.GenerateNewLevelClientRpc))]
        static void OnStartGame()
        {
            Plugin.LoggerInstance.LogInfo("Start Game - starting Game");

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Plugin.LoggerInstance.LogInfo("Host or server -  Generating missions");
                Plugin.MissionManager.GenerateMissions(Plugin.Config.MaxMissions.Value);
            }
        }

        // private static void LookForRagdolls()
        // {
        //     GameObject ship = GameObject.Find("/Environment/HangarShip");

        //     var ragdolls = ship.GetComponentsInChildren<RagdollGrabbableObject>().ToList();

        //     ragdolls.Do(ragdoll => Plugin.LoggerInstance.LogInfo($"{ragdoll.name} - {ragdoll.isInShipRoom}"));
        // }

    }

}