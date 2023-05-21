using System;
using System.Collections.Generic;

namespace Fluffy.Linq
{
    public static partial class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int count, int skip)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (skip <= 0)
                throw new ArgumentOutOfRangeException(nameof(skip));

            using var e = source.GetEnumerator();
            
            var queue = new Queue<T>(count);

            do
            {
                var value = e.Current;
                queue.Enqueue(value);

                if (queue.Count == count)
                {
                    yield return queue;

                    for (var i = 0; i < skip; i++)
                    {
                        queue.Dequeue();
                    }
                }
            } while (e.MoveNext());
        }
    }
}