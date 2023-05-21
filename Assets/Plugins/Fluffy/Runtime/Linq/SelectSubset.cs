using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Fluffy.Linq
{
    public static partial class LinqExtensions
    {
        /// <summary>
        /// Returns an array of random elements of the specified size from the original array.
        /// </summary>
        /// <param name="source">The source sequence from which random elements are selected and returned.</param>
        /// <param name="count">Number of elements to select.</param>
        /// <typeparam name="T">Type of source sequence element.</typeparam>
        /// <returns>A specified number of non-duplicate random elements order from the original sequence.</returns>
        /// <exception cref="ArgumentNullException">The source parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// count parameter is less than or equal to 0 or less than the number of elements in the original sequence.
        /// </exception>
        public static IEnumerable<T> RandomSubset<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            return RandomSubsetImpl(source, count);
        }

        private static IEnumerable<T> RandomSubsetImpl<T>(IEnumerable<T> source, int count)
        {
            var array = source as T[] ?? source.ToArray();
            var length = array.Length;
            
            if (length < count)
                throw new ArgumentException("Sequence must contain at least as many elements as requested.");

            var swapIndex = length - 1;
            
            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, length - i);
                yield return array[index];
                (array[index], array[swapIndex]) = (array[swapIndex], array[index]);
                swapIndex--;
            }
        }
    }
}