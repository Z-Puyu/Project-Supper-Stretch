using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Util.Singleton;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T? instance;
    
    public static T Instance {
        get {
            if (Singleton<T>.instance != null) {
                return Singleton<T>.instance;
            }

            Singleton<T>.instance = Object.FindAnyObjectByType<T>();
            if (Singleton<T>.instance != null) {
                return Singleton<T>.instance;
            }

            GameObject gameObject = new GameObject(typeof(T).Name + " (Auto-generated)");
            Singleton<T>.instance = gameObject.AddComponent<T>();

            return Singleton<T>.instance;
        }
    }
    
    public static bool Exists => Singleton<T>.instance != null;

    [field: SerializeField]
    private bool ShouldNotDestroyOnLoad { get; set; } = true;

    private void Awake() {
        if (!Application.isPlaying) {
            return;
        }

        if (this.ShouldNotDestroyOnLoad) {
            this.transform.SetParent(null);
            if (Singleton<T>.instance == null) {
                Singleton<T>.instance = this as T;
                Object.DontDestroyOnLoad(this.gameObject);
            } else if (this != Singleton<T>.instance) {
                Object.Destroy(this.gameObject);
            }
        } else {
            Singleton<T>.instance = this as T;
        }
    }
}
