﻿using System.Linq;
using Project.Scripts.Util.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Util.Singleton;

[DisallowMultipleComponent]
public class Singleton<T> : MonoBehaviour where T : Component {
    private static T? instance;
    
    public static T Instance {
        get {
            if (Singleton<T>.instance) {
                return Singleton<T>.instance;
            }

            Singleton<T>.instance = Object.FindAnyObjectByType<T>();
            if (Singleton<T>.instance) {
                return Singleton<T>.instance;
            }

            GameObject gameObject = new GameObject(typeof(T).Name + " (Auto-generated)");
            Singleton<T>.instance = gameObject.AddComponent<T>();

            return Singleton<T>.instance;
        }
    }
    
    public static bool Exists => Singleton<T>.instance;

    [field: SerializeField]
    private bool ShouldNotDestroyOnLoad { get; set; } = true;

    protected virtual void Awake() {
        if (!Application.isPlaying) {
            return;
        }

        Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
              .Where(t => t != this)
              .ForEach(Object.Destroy);
        if (this.ShouldNotDestroyOnLoad) {
            this.transform.SetParent(null);
            if (!Singleton<T>.instance) {
                Singleton<T>.instance = this as T;
                Object.DontDestroyOnLoad(this.gameObject);
            } else if (this != Singleton<T>.instance) {
                Object.Destroy(this);
            }
        } else {
            Singleton<T>.instance = this as T;
        }
    }
}
