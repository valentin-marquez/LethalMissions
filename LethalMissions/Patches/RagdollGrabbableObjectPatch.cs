using HarmonyLib;
using LethalMissions.Scripts;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(RagdollGrabbableObject))]
    internal class RagdollGrabbableObjectPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RagdollGrabbableObject.Update))]
        private static void OnRagdollGrabbableObjectTriggerEnter(RagdollGrabbableObject __instance)
        {
            if (__instance.isInShipRoom)
            {
                Plugin.LoggerInstance.LogInfo($"RagdollGrabbableObject {__instance.name} is in ship room, {__instance.itemProperties.itemId}");
                Plugin.MissionManager.CompleteMission(MissionType.RecoverCrewmateBody);
            }
        }
    }
}