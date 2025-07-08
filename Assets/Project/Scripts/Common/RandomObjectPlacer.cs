using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Common;

[DisallowMultipleComponent]
public class RandomObjectPlacer : MonoBehaviour {
    [field: SerializeField] private List<GameObject> Objects { get; set; } = [];
    [field: SerializeField] private bool RandomiseRotation { get; set; } = true;
    
    private void Start() {
        GameObject prefab = this.Objects[Random.Range(0, this.Objects.Count)];
        GameObject instance = Object.Instantiate(prefab, this.transform);
        instance.transform.localPosition = Vector3.zero;
        if (this.RandomiseRotation) {
            instance.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        } else {
            instance.transform.rotation = Quaternion.identity;
        }
    }
}
