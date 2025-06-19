using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Project.Scripts.Util.Linq;

public static class LinqExtensions {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (T t in source) {
            action(t);
        }
    }

    /// <summary>
    /// Slice a list into a new list.
    /// </summary>
    /// <param name="source">The list.</param>
    /// <param name="start">The inclusive start index.</param>
    /// <param name="end">The exclusive end index.</param>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <returns>A new list.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="start"/> is negative.</exception>
    public static IEnumerable<T> Slice<T>(this IList<T> source, int start, int end) {
        if (start < 0) {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (end < start) {
            return [];
        }

        return end >= source.Count ? source.Skip(start) : source.Skip(start).Take(end - start);
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

    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i -= 1) {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static void Shuffle<T>(this IEnumerable<T> list, out IEnumerable<T> shuffled) {
        IList<T> shuffledList = [..list];
        for (int i = shuffledList.Count - 1; i > 0; i -= 1) {
            int j = Random.Range(0, i + 1);
            (shuffledList[i], shuffledList[j]) = (shuffledList[j], shuffledList[i]);
        }

        shuffled = shuffledList;
    }
}
