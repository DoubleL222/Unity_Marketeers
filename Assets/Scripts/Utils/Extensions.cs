using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Extensions  {

        public static void DoActionOnGameObjectAndChildren(this GameObject o, System.Action<GameObject> f)
        {
            f(o);

            int numChildren = o.transform.childCount;

            for (int i = 0; i < numChildren; ++i)
            {
                o.transform.GetChild(i).gameObject.DoActionOnGameObjectAndChildren(f);
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}