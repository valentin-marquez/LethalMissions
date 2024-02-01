using System;
using System.Collections.Generic;
using System.Threading;
using LethalMissions.Localization;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LethalMissions.Scripts
{

    public static class Utils
    {
        private static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static System.Random Local;

            public static System.Random ThisThreadsRandom
            {
                get { return Local ??= new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)); }
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(--n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void NotifyMissions()
        {
            switch (Plugin.Config.MissionsNotification.Value)
            {
                case NotificationOption.SoundOnly:
                    RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, HUDManager.Instance.tipsSFX, randomize: true);
                    break;
                case NotificationOption.SoundAndBanner:
                    RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, HUDManager.Instance.tipsSFX, randomize: true);
                    HUDManager.Instance.DisplayTip("LethalMissions", MissionLocalization.GetMissionString("NewMissionsAvailable"), true);
                    break;
                case NotificationOption.BannerOnly:
                    HUDManager.Instance.DisplayTip("LethalMissions", MissionLocalization.GetMissionString("NewMissionsAvailable"), true);
                    break;
                case NotificationOption.None:
                default:
                    break;
            }
        }

        public static string NumberToHour(int n, bool isPM)
        {
            int hour;

            if (n < 1 || n > 19)
            {
                hour = 0; // return 0 if the number is not in the range of 1 to 19
            }
            else if (isPM && n >= 6)
            {
                hour = n - 6; // subtract 6 from the number if it's PM and the number is greater than or equal to 6
            }
            else if (!isPM && n == 19)
            {
                hour = 12; // return 12 if it's AM and the number is 19
            }
            else
            {
                hour = n + 5; // add 5 to the number in all other cases
            }

            return hour.ToString();
        }
    }
    public class SimpleItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        public SimpleItem(int itemId, string itemName)
        {
            ItemId = itemId;
            ItemName = itemName;
        }
    }

    public enum GameStateEnum
    {
        InOrbit,
        TakingOff,
        OnMoon,
    }

}