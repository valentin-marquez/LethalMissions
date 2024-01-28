using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LethalMissions.Patches
{
    public class GrabbableObjectPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
        public static void OnStartGameItems()
        {
            if (StartOfRound.Instance.currentLevel.levelID == 3)
            {
                return;
            }

            GameObject ship = GameObject.Find("/Environment/HangarShip");
            GrabbableObject[] items = ship.GetComponentsInChildren<GrabbableObject>();

            GrabbableObject itemToExclude1 = items.FirstOrDefault(obj => obj.name == "ClipboardManual");
            GrabbableObject itemToExclude2 = items.FirstOrDefault(obj => obj.name == "StickyNoteItem");

            foreach (GrabbableObject item in items)
            {
                if (item != itemToExclude1 && item != itemToExclude2)
                {
                    item.gameObject.AddComponent<ItemExtra>();
                }
            }
        }
    }
}