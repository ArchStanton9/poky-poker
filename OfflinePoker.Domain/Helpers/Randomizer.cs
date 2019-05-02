using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OfflinePoker.Domain.Helpers
{
    public static class Randomizer
    {
        private static readonly Lazy<ConcurrentBag<Random>> randomsPool
            = new Lazy<ConcurrentBag<Random>>(() =>
            {
                var randoms = Enumerable.Range(0, 10)
                    .Select(i => new Random(Environment.TickCount - i));

                return new ConcurrentBag<Random>(randoms);
            });


        public static int Next()
        {
            while (true)
            {
                if (randomsPool.Value.TryPeek(out var random))
                {
                    var result = random.Next();
                    randomsPool.Value.Add(random);
                    return result;
                }
            }
        }

        public static int Next(int min, int max)
        {
            while (true)
            {
                if (randomsPool.Value.TryPeek(out var random))
                {
                    var result = random.Next(min, max);
                    randomsPool.Value.Add(random);
                    return result;
                }
            }
        }
    }
}
