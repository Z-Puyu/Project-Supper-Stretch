using UnityEngine;

namespace Project.Scripts.Util.Singleton;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T> {
    private static T? instance;
    
    public static T Instance { 
        get {
            if (SingletonScriptableObject<T>.instance) {
                return SingletonScriptableObject<T>.instance;
            }

            T[] resources = Resources.LoadAll<T>("");
            if (resources is null || resources.Length == 0) {
                Debug.LogError($"No asset of type {typeof(T)} found! A default instance will be created.");
                SingletonScriptableObject<T>.instance = ScriptableObject.CreateInstance<T>();
            } else {
                SingletonScriptableObject<T>.instance = resources[0];
                if (resources.Length > 1) {
                    Debug.LogWarning($"Multiple assets of type {typeof(T)} found! Only the first one is used.");
                }
            }
            
            return SingletonScriptableObject<T>.instance;
        }
    }
}
