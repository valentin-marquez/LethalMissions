using HarmonyLib;
using LethalMissions.Scripts;
using Unity.Netcode;
using UnityEngine;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        public static int Hour { get; private set; }
        public static bool IsPM { get; private set; } = false;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HUDManager.SetClock))]
        private static void OnSetClock(float timeNormalized, float numberOfHours)
        {
            int totalMinutes = (int)(timeNormalized * (60f * numberOfHours)) + 360;
            int hour = (int)Mathf.Floor(totalMinutes / 60);

            if (hour >= 12)
            {
                hour %= 12;
                IsPM = true;
            }
            else
            {
                IsPM = false;
            }
            Plugin.LoggerInstance.LogInfo($"Current Time: {hour}, Es PM: {IsPM}");
        }
    }

}