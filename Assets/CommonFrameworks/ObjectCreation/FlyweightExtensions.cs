using UnityEngine;

namespace CommonFrameworks.ObjectCreation {
    public static class FlyweightExtensions {
        public static void ReturnToPool<T>(this T component) where T : Component {
            FlyweightFactory.Recycle(component);
        }
        
        public static void ReturnToPool(this GameObject obj) {
            FlyweightFactory.Recycle(obj);
        }
    }
}