using System;
using System.Collections.Generic;

namespace Project.Scripts.Util;

public static class LinqExtensions {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (T t in source) {
            action(t);
        }
    }
}
