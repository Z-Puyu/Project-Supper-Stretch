using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Flags {
    public static class FlagsExtension {
        public static bool HasAny<T>(this IFlag<T> flag, IEnumerable<T> test) {
            return test.Any(flag.Has);
        }

        public static bool HasAll<T>(this IFlag<T> flag, IEnumerable<T> test) {
            return test.All(flag.Has);
        }

        public static bool HasNone<T>(this IFlag<T> flag, IEnumerable<T> test) {
            return !test.Any(flag.Has);
        }
        
        public static bool HasAnyOrEmpty<T>(this IFlag<T> flag, IEnumerable<T> test) {
            T[] array = test.ToArray();
            return array.Length == 0 || flag.HasAny(array);
        }
    }
}
