using System;
using System.Collections.Generic;
using System.Linq;

namespace uwpKarate.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var target in source.OrEmptyIfNull())
            {
                action(target);
            }
        }

        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            if (source != null) return source;

            return Enumerable.Empty<T>();
        }
    }
}