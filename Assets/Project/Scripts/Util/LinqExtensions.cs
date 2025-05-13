using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.Util;

public static class LinqExtensions {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (T t in source) {
            action(t);
        }
    }

    public static bool DifferentFrom<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return source.Except(other).Any();
    }
    
    public static bool SameAs<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return !source.DifferentFrom(other);
    }
    
    public static bool DisjointWith<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return !source.Intersect(other).Any();
    }
    
    public static bool Intersects<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return source.Intersect(other).Any();
    }

    public static bool Contains<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return other.All(source.Contains);
    }
    
    public static bool ContainedIn<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        return source.All(other.Contains);
    }
}
