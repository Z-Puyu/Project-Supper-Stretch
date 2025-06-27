using System.Collections.Generic;
using Project.Scripts.Util.Pooling;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies;

public class RandomSpawner<T> : MonoBehaviour where T : MonoBehaviour, IPoolable<T> {
    [field: SerializeField, MinMaxSlider(0, 10)] 
    private Vector2Int SpawnAmount { get; set; } = new Vector2Int(0, 3);
    
    [field: SerializeField] private List<T> Prefabs { get; set; } = [];
    private List<Pool<T>> Pools { get; set; } = [];

    private void Awake() {
        foreach (T prefab in this.Prefabs) {
            this.Pools.Add(Pool<T>.Builder.Of(prefab).Build());
        }
    }
}
