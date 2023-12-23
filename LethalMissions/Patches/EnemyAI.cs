using HarmonyLib;

namespace LethalMissions.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    public class EnemyAIPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyAI.KillEnemy))]
        public static void OnEnemyKilled(EnemyAI __instance)
        {
            if (__instance.isEnemyDead)
            {
                Plugin.MissionManager.CompleteMission(Scripts.MissionType.KillMonster);
            }
        }
    }
}