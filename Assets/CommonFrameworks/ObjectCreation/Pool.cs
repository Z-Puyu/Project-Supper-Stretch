using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CommonFrameworks.ObjectCreation { 
    internal sealed class Pool {
        private ObjectPool<PoolableObject> InternalPool { get; }

        internal Pool(PoolableObject prefab, int size) {
            this.InternalPool = new ObjectPool<PoolableObject>(
                createFunc: () => PoolableObject.CreateFrom(prefab),
                actionOnGet: obj => obj.Activate(onReturn: this.Release),
                actionOnDestroy: obj => obj.Destroy(),
                actionOnRelease: obj => obj.Deactivate(),
                defaultCapacity: size
            );
        }
        
        public T Pull<T>() where T : Component {
            return this.InternalPool.Get().As<T>();
        }
        
        public GameObject Pull() {
            return this.InternalPool.Get().gameObject;
        }

        private void Release(PoolableObject obj) {
            this.InternalPool.Release(obj);
        }
    }
}
