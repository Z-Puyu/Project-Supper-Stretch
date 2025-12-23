using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonFrameworks.Utilities;

[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private enum PersistenceLevel { Scene, Game }

    private static T instance;
        
    public static T Instance {
        get {
            if (Singleton<T>.instance) {
                return Singleton<T>.instance;
            }

            Singleton<T>.instance = Object.FindAnyObjectByType<T>();
            if (Singleton<T>.instance) {
                return Singleton<T>.instance;
            }
                
            return Singleton<T>.instance = new GameObject($"{typeof(T).Name} (auto-generated)").AddComponent<T>();
        }

        private set => Singleton<T>.instance = value;
    }

    [field: SerializeField] private PersistenceLevel LevelOfPersistence { get; set; } = PersistenceLevel.Scene;
        
    protected virtual void Awake() {
        if (!Application.isPlaying) {
            return;
        }
            
        if (Singleton<T>.instance) {
            Object.Destroy(this.gameObject);
        } else {
            Singleton<T>.Instance = this as T;
        }
    }

    protected virtual void Start() {
        if (this.LevelOfPersistence == PersistenceLevel.Game) {
            this.transform.SetParent(null);
            Object.DontDestroyOnLoad(this.gameObject);
        }
    }
}