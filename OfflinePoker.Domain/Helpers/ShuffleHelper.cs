using System.Collections.Generic;

namespace OfflinePoker.Domain.Helpers
{
    public static class ShuffleHelper
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            var list = new List<T>(items);
            var count = list.Count;
            var last = count - 1;

            for (var i = 0; i < last; ++i)
            {
                var r = Randomizer.Next(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }

            return list;
        }
    }
}
