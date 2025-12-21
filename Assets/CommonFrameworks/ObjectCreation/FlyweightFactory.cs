using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CommonFrameworks.ObjectCreation {
    public static class FlyweightFactory {
        private static Dictionary<PoolableObject, IObjectPool<Flyweight>> Pools { get; } =
            new Dictionary<PoolableObject, IObjectPool<Flyweight>>();

        private static IObjectPool<Flyweight> GetPool(PoolableObject obj) {
            return FlyweightFactory.Pools.TryGetValue(obj, out IObjectPool<Flyweight> pool) ? pool : obj.CreatePool();
        }

        public static T Pull<T>(PoolableObject prefab) where T : Component {
            return FlyweightFactory.GetPool(prefab).Get().As<T>();
        }
        
        public static GameObject Pull(PoolableObject prefab) {
            return FlyweightFactory.GetPool(prefab).Get().gameObject;
        }
        
        public static void Recycle(Flyweight flyweight) {
            FlyweightFactory.GetPool(flyweight.SourceObject).Release(flyweight);
        }

        public static void Recycle(GameObject obj) {
            if (obj.TryGetComponent(out Flyweight flyweight)) {
                FlyweightFactory.Recycle(flyweight);
            } else {
#if DEBUG
                Debug.LogError($"{obj.name} is not a flyweight and cannot be recycled by the factory.");
#endif
            }
        }
        
        public static void Recycle<T>(T instance) where T : Component {
            FlyweightFactory.Recycle(instance.gameObject);
        }
        
        public static void Clear() {
            FlyweightFactory.Pools.Clear();
        }
    }
}
