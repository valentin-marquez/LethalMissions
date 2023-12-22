
using HarmonyLib;

namespace LethalMissions.Patches
{

    [HarmonyPatch(typeof(EnemyAI))]
    public class EnemyAIPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.Update))]
        public static void OnEnemyAIUpdate(EnemyAI __instance)
        {
            if (__instance.isEnemyDead)
            {
                Plugin.MissionManager.CompleteMission(Scripts.MissionType.KillMonster);
            }
        }
    }
}