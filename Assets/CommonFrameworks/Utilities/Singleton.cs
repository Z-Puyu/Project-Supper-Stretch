using UnityEngine;

namespace CommonFrameworks.Utilities {
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private enum PersistenceLevel { Scene, Game }

        private static T instance;
        
        public static T Instance {
            get {
                if (!Singleton<T>.instance) {
                    Singleton<T>.instance = new GameObject($"{typeof(T).Name} (auto-generated)").AddComponent<T>();
                }   
                
                return Singleton<T>.instance;
            }

            private set => Singleton<T>.instance = value;
        }

        [field: SerializeField] private PersistenceLevel LevelOfPersistence { get; set; } = PersistenceLevel.Scene;
        
        protected virtual void Awake() {
            if (Singleton<T>.Instance) {
                Object.Destroy(this.gameObject);
            } else {
                Singleton<T>.Instance = this as T;
                if (this.LevelOfPersistence == PersistenceLevel.Game) {
                    Object.DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }
}
