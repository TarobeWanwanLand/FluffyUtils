using System;

namespace Fluffy.Random
{
    /// <summary>
    /// <see cref="System.Random"/> that is shared between threads.
    /// </summary>
    public static class ShareRandom
    {
        [ThreadStatic]
        private static System.Random _random = new();

        public static int NextInt() => _random.Next();
        public static int NextInt(int maxValue) => _random.Next(maxValue);
        public static int NextInt(int minValue, int maxValue) => _random.Next(minValue, maxValue);
        
        public static float NextFloat() => (float)_random.NextDouble();
        public static float NextFloat(float maxValue) => (float)_random.NextDouble() * maxValue;
        public static float NextFloat(float minValue, float maxValue) =>
            (float)_random.NextDouble() * (maxValue - minValue) + minValue;
    }
}