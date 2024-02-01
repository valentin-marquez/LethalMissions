using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LethalMissions.Patches
{
    public class EnemyTypeInitializer
    {
        public static Dictionary<Type, EnemyType> OutsideEnemies = new Dictionary<Type, EnemyType>();
        public static Dictionary<Type, EnemyType> InsideEnemies = new Dictionary<Type, EnemyType>();
        public static HashSet<Item> Items = new HashSet<Item>();

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        static void AwakePostfix()
        {
            foreach (EnemyType enemyType in Resources.FindObjectsOfTypeAll<EnemyType>())
            {
                Type enemyAIType = enemyType.enemyPrefab.GetComponent<EnemyAI>().GetType();

                if (enemyType.isOutsideEnemy)
                {
                    if (!OutsideEnemies.ContainsKey(enemyAIType))
                    {
                        OutsideEnemies.Add(enemyAIType, enemyType);
                    }
                }
                else
                {
                    if (!InsideEnemies.ContainsKey(enemyAIType))
                    {
                        InsideEnemies.Add(enemyAIType, enemyType);
                    }
                }
            }

            foreach (Item item in Resources.FindObjectsOfTypeAll<Item>())
            {
                Items.Add(item);
            }
        }
    }
}