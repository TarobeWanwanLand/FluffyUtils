using System;
using System.Collections.Generic;
using System.Linq;

namespace Fluffy.Linq
{
    public static partial class LinqExtensions
    {
        private static Random _random = new();
        
        public static T Random<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var list = source as List<T> ?? source.ToList();
            var len = list.Count;
            
            if (len == 0)
                throw new InvalidOperationException("Sequence cannot be empty.");
            
            var index = _random.Next(len);
            return list[index];
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            var list = source as List<T> ?? source.ToList();
            var len = list.Count;
            
            if (len < count)
                throw new InvalidOperationException("Sequence must contain at least as many elements as requested.");

            for (var i = 0; i < count; i++)
            {
                var index = _random.Next(len - i);
                yield return list[index];
                list.RemoveAt(index);
            }
        }
    }
}