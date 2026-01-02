using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonFrameworks.Utilities {
    public static class Database<T> where T : Object {
        private static Lazy<IEnumerable<T>> Cache { get; set; } = new Lazy<IEnumerable<T>>(Database<T>.Load);
#if UNITY_EDITOR
        public static IEnumerable<T> LoadedResources => Database<T>.Load();
#else
        public static IEnumerable<T> LoadedResources => Database<T>.Cache.Value;
#endif
        public static bool IsLoaded => Database<T>.Cache.IsValueCreated;
        
        private static IEnumerable<T> Load() {
            return Resources.LoadAll<T>("");
        }
        
        public static void Reload() {
            Database<T>.Cache = new Lazy<IEnumerable<T>>(Database<T>.Load);
        }
    }
}