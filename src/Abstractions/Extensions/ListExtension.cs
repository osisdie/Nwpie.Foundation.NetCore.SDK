using System;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class ListExtension
    {
        public static void Shuffle<T>(T[] array)
        {
            var n = array.Length;
            for (var i = 0; i < n; i++)
            {
                var r = i + (int)(Randomizer.NextDouble() * (n - i));
                var t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        public static readonly Random Randomizer = new Random();
    }
}
