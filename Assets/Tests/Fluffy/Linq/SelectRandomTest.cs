using System;
using System.Collections.Generic;
using System.Linq;
using Fluffy.Linq;
using NUnit.Framework;
using Random = System.Random;

public class SelectRandom
{
    [Test]
    public void SelectRandomCount()
    {
        var sequence = Enumerable.Range(0, 100);
        var random = new Random();

        for (var i = 0; i < 10000; i++)
        {
            var count = random.Next(1, 100);
            var results = sequence.RandomSubset(count);

            Assert.AreEqual(count, results.Count());
            Assert.IsTrue(results.All(x => sequence.Contains(x)));
            Assert.IsTrue(results.Distinct().Count() == count);
        }
    }
    
    [Test]
    public void SelectRandomThrowsAtNullSequence()
    {
        Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).RandomSubset(10));
    }

    [Test]
    public void SelectRandomThrowsAtZeroCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 100).RandomSubset(0));
    }

    [Test]
    public void SelectRandomThrowsAtNegativeCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 100).RandomSubset(-10));
    }
    
    
}