using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Util.Components;

public static class ScriptUtil {
    public static T[] GetAllInstances<T>() where T : ScriptableObject {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}"); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i += 1) {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }
}
