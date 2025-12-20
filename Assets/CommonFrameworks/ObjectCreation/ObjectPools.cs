using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFrameworks.ObjectCreation {
    public static class ObjectPools {
        private static Dictionary<Guid, Pool> Pools { get; } = new Dictionary<Guid, Pool>();

        internal static Pool CreatePool(PoolableObject prefab, int size) {
            if (ObjectPools.Pools.TryGetValue(prefab.Id, out Pool pool)) {
                return pool;
            }
            
            pool = new Pool(prefab, size);
            ObjectPools.Pools.Add(prefab.Id, pool);
            return pool;
        }

        private static Pool GetPool(PoolableObject prefab) {
            return ObjectPools.Pools.TryGetValue(prefab.Id, out Pool pool) ? pool : ObjectPools.CreatePool(prefab, 100);
        }

        public static T Pull<T>(PoolableObject prefab) where T : Component {
            return ObjectPools.GetPool(prefab).Pull<T>();
        }
        
        public static GameObject Pull(PoolableObject prefab) {
            return ObjectPools.GetPool(prefab).Pull();
        }
    }
}
