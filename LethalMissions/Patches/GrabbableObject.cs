using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LethalMissions.Patches
{
    public class GrabbableObjectPatch
    {
        public static Dictionary<int, int> ScrapCollectedLastRound { get; internal set; } = new Dictionary<int, int>();
        public static HashSet<int> InitialItemsInShip { get; internal set; } = new HashSet<int>();


        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "EndOfGame")]
        public static void OnEndOfGame()
        {
            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var items = ship.GetComponentsInChildren<GrabbableObject>()
                .Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem");
            ScrapCollectedLastRound.Clear();

            foreach (var item in items)
            {
                int itemId = item.itemProperties.itemId;
                if (!InitialItemsInShip.Contains(itemId))
                {
                    if (ScrapCollectedLastRound.ContainsKey(itemId))
                    {
                        ScrapCollectedLastRound[itemId]++;
                    }
                    else
                    {
                        ScrapCollectedLastRound[itemId] = 1;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.OnBroughtToShip))]
        public static void OnBroughtToShipPostfix(GrabbableObject __instance)
        {
            int itemId = __instance.itemProperties.itemId;
            if (!InitialItemsInShip.Contains(itemId))
            {
                if (ScrapCollectedLastRound.ContainsKey(itemId))
                {
                    ScrapCollectedLastRound[itemId]++;
                }
                else
                {
                    ScrapCollectedLastRound[itemId] = 1;
                }
            }
        }
    }
}