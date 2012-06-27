using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.Internal
{
    internal static class Enumerable
    {
        static internal IEnumerable<T> ToEnumerable<T>(this IEnumerable<T> source)
        {
            return source != null ? source.AsEnumerable() : null;
        }

        static private IEnumerable<T> CreateEnumerable<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
                yield return item;
        }
    }
}
