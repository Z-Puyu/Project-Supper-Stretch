using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor;

public class ObjectCache<T> where T : ScriptableObject {
    private static ObjectCache<T>? instance;

    public static ObjectCache<T> Instance {
        get {
            if (ObjectCache<T>.instance is not null) {
                return ObjectCache<T>.instance;
            }

            ObjectCache<T>.instance = new ObjectCache<T>();
            ObjectCache<T>.instance.RefreshCache();
            return ObjectCache<T>.instance;
        }
    }

    private List<T> CachedObjects { get; init; } = [];
    
    public IReadOnlyList<T> Objects => this.CachedObjects.AsReadOnly();
    public int Count => this.CachedObjects.Count;
    
    public event Action<T> OnObjectAdded = delegate { };
    public event Action<T> OnObjectRemoved = delegate { };
    public event Action OnCacheRefreshed = delegate { };

#if UNITY_EDITOR
    private ObjectCache() {
        // Subscribe to asset database changes
        EditorApplication.projectChanged += this.RefreshCache;
        AssemblyReloadEvents.beforeAssemblyReload += this.OnBeforeAssemblyReload;
    }

    private void RefreshCache() {
        HashSet<T> previousObjects = new HashSet<T>(this.CachedObjects);
        this.CachedObjects.Clear();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);

            if (!obj) {
                continue;
            }

            this.CachedObjects.Add(obj);
        }

        // Detect added objects
        IEnumerable<T> newObjects = this.CachedObjects.Where(obj => !previousObjects.Contains(obj));
        foreach (T? obj in newObjects) {
            this.OnObjectAdded.Invoke(obj);
        }

        // Detect removed objects
        IEnumerable<T> removedObjects = previousObjects.Where(obj => !this.CachedObjects.Contains(obj));
        foreach (T? obj in removedObjects) {
            this.OnObjectRemoved.Invoke(obj);
        }

        this.OnCacheRefreshed.Invoke();
        Debug.Log($"ScriptableObject cache refreshed. Found {this.CachedObjects.Count} instances of {typeof(T).Name}");
    }
    
    private void OnBeforeAssemblyReload() {
        EditorApplication.projectChanged -= this.RefreshCache;
    }
#endif
}
