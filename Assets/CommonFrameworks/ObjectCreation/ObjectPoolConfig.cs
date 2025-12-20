using SaintsField;
using UnityEngine;

namespace CommonFrameworks.ObjectCreation {
    [CreateAssetMenu(fileName = "New Object Pool Config", menuName = "Object Pooling/Object Pool Config", order = 0)]
    public sealed class ObjectPoolConfig : ScriptableObject {
        [field: SerializeField, SaintsDictionary("Prefab", "Initial Capacity")] 
        private SaintsDictionary<PoolableObject, int> Prefabs { get; set; } = new SaintsDictionary<PoolableObject, int>();

        private void OnEnable() {
            foreach ((PoolableObject prefab, int size) in this.Prefabs) {
                ObjectPools.CreatePool(prefab, size);
            }
        }
    }
}
