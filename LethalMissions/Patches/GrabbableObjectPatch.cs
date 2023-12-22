using HarmonyLib;
using LethalMissions.Scripts;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GrabbableObject.Update))]
        private static void OnGrabbableObjectTriggerEnter(GrabbableObject __instance)
        {
            if (__instance.itemProperties.itemId == 1531 && __instance.isInShipRoom)
            {
                Plugin.MissionManager.CompleteMission(MissionType.ObtainHoneycomb);
            }
            else if (__instance.itemProperties.itemId == 3 && __instance.isInShipRoom)
            {
                Plugin.MissionManager.CompleteMission(MissionType.ObtainGenerator);
            }
        }
    }
}