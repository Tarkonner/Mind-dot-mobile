using System;
using System.Diagnostics;
using System.Linq;

public class ShuffleElements
{
    static Random random = new Random();

    // Generic Fisher-Yates Shuffle
    public static T[] Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            Swap(ref array[i], ref array[j]);
        }

        return array;
    }

    public static T[] ShuffleWithReturn<T>(T[] target)
    {
        T[] result = new T[target.Length];
        for (int i = 0; i < target.Length; i++)
            result[i] = target[i];

        for (int i = target.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            Swap(ref result[i], ref result[j]);
        }

        return result;
    }

    // Helper method to swap two elements in an array
    static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    /// <summary>
    /// Will not place the same X number in the first 2 slots
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="howManyElements"></param>
    public static void ReshuffleWithWight<T>(T[] target, int howManyElements)
    {
        //Known error
        if (howManyElements >= target.Length)
        {
            Debug.WriteLine("More element to remenber when target to shuffle");
            return;
        }
        if(target.Length <= 1)
        {
            Debug.WriteLine($"The array is {target.Length} long. Can't shuffle");
            return;
        }

        T lastElement = target[target.Length - 1];        

        for (int i = target.Length - 1; i > 0; i--)
        {
            int j;

            if (i >= target.Length - howManyElements - 1)
                j = random.Next(howManyElements, i + 1);
            else
                j= random.Next(0, i + 1);

            Swap(ref target[i], ref target[j]);
        }
    }
}
