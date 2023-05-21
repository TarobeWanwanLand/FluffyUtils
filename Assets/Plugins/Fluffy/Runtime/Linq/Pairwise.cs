using System;
using System.Collections.Generic;

namespace Fluffy.Linq
{
    public readonly struct Pair<T>
    {
        public readonly T Previous;
        public readonly T Current;
        
        public Pair(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }
    }
    
    public static partial class LinqExtensions
    {
        public static IEnumerable<Pair<T>> PairWise<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using var e = source.GetEnumerator();
            
            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence cannot be empty.");
                
            var prev = e.Current;

            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence must contain at least two elements.");
                
            do
            {
                var value = e.Current;
                yield return new Pair<T>(prev, value);
                prev = value;
            } while (e.MoveNext());
        }
    }
}