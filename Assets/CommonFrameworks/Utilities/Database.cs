using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonFrameworks.CommonUtilities {
    public static class Database<T> where T : Object {
        private static Lazy<IEnumerable<T>> Cache { get; } = new Lazy<IEnumerable<T>>(Database<T>.Load);
        public static IEnumerable<T> LoadedResources => Database<T>.Cache.Value;
        public static bool IsLoaded => Database<T>.Cache.IsValueCreated;
        
        private static IEnumerable<T> Load() {
            return Resources.LoadAll<T>("");
        }
    }
}
