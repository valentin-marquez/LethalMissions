using System;
using System.Collections.Generic;
using System.Threading;

namespace LethalMissions.Scripts
{

    public static class Utils
    {
        private static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;

            public static Random ThisThreadsRandom
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
}