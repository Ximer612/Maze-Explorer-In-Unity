using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0,i + 1);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }
}
