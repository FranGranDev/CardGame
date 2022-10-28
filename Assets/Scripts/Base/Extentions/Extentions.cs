using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{ 
    public static int GetRandom(int min, int max, int except)
    {
        for(int i = 0; i < 10; i++)
        {
            int value = Random.Range(min, max);
            if (value != except)
                return value;
        }

        return Random.Range(min, max);
    }


    public static string ShortConvert(this int value)
    {
        return FormatNumber(value);
    }
    private static string FormatNumber(int num)
    {
        if (num >= 100000000)
        {
            return (num / 1000000).ToString("0.#M");
        }
        if (num >= 1000000)
        {
            return (num / 1000000).ToString("0.##M");
        }
        if (num >= 100000)
        {
            return (num / 1000).ToString("0.#k");
        }
        if (num >= 10000)
        {
            return (num / 1000).ToString("0.##k");
        }

        return num.ToString("#,0");
    }

    public static T GetRandom<T>(this List<T> list, int prevIndex = -1)
    {
        if (list == null || list.Count == 0)
            return default(T);

        if (prevIndex == -1 || list.Count == 1)
            return list[Random.Range(0, list.Count)];

        int maxIterations = 10;
        int iteration = 0;
        int index = 0;
        while (iteration < maxIterations)
        {
            index = Random.Range(0, list.Count);
            if (index == prevIndex)
            {
                iteration++;
                continue;
            }
            return list[index];
        }
        return list[index];
    }
    public static void Shuffle<T>(this System.Random random, IList<T> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = random.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static Coroutine Delayed(this MonoBehaviour behaviour, System.Action action, float delay)
    {
        return behaviour.StartCoroutine(behaviour.DelayedCour(action, delay));
    }
    private static IEnumerator DelayedCour(this MonoBehaviour behaviour, System.Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        action?.Invoke();

        yield break;
    }
    public static Coroutine Delayed(this MonoBehaviour behaviour, System.Action action, YieldInstruction instruction)
    {
        return behaviour.StartCoroutine(behaviour.DelayedCour(action, instruction));
    }
    private static IEnumerator DelayedCour(this MonoBehaviour behaviour, System.Action action, YieldInstruction instruction)
    {
        yield return instruction;

        action?.Invoke();

        yield break;
    }
}
