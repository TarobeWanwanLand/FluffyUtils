using System;
using System.Collections.Generic;
using System.Linq;

namespace Fluffy.Linq
{
    public static partial class LinqExtensions
    {
        public static T WeightedRandom<T>(this IEnumerable<(T element, int weight)> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var list = source as List<(T element, int weight)> ?? source.ToList();
            
            if (list.Count == 0)
                throw new InvalidOperationException("Sequence cannot be empty.");
            
            var totalWeight = 0;
            foreach (var (_, weight) in list)
            {
                if (weight < 0)
                    throw new ArgumentOutOfRangeException(nameof(source), "Weight cannot be negative.");
                totalWeight += weight;
            }
            
            if (totalWeight == 0)
                throw new InvalidOperationException("Total weight cannot be zero.");

            var threshold = _random.Next(totalWeight);
            var cumulative = 0;
            
            foreach (var (element, weight) in list)
            {
                cumulative += weight;
                if (cumulative > threshold)
                    return element;
            }
            
            throw new InvalidOperationException("This should never happen.");
        }

        public static IEnumerable<T> WeightedRandom<T>(this IEnumerable<(T element, int weight)> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            var list = source as List<(T element, int weight)> ?? source.ToList();
            var length = list.Count;
            
            if (length < count)
                throw new InvalidOperationException("Sequence must contain at least as many elements as requested.");

            var totalWeight = 0;
            foreach (var (_, weight) in list)
            {
                if (weight < 0)
                    throw new ArgumentOutOfRangeException(nameof(source), "Weight cannot be negative.");
                totalWeight += weight;
            }
            
            if (totalWeight == 0)
                throw new InvalidOperationException("Total weight cannot be zero.");
            
            for (var i = 0; i < count; i++)
            {
                var threshold = _random.Next(totalWeight);
                var cumulative = 0;

                for (var j = 0; j < length; j++)
                {
                    var weight = list[j].weight;
                    
                    cumulative += weight;
                    if (cumulative > threshold)
                    {
                        yield return list[j].element;
                        list.RemoveAt(j);
                        
                        length--;
                        totalWeight -= weight;
                        
                        break;
                    }
                }
            }
        }
    } 
}