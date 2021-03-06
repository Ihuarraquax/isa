using System;
using System.Collections.Generic;

namespace GeneticAlgorithmModule.Extensions
{
    public static class ArrayExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
