using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
public class GoalPoint : MonoBehaviour {
    public static event UnityAction OnReached = delegate { };
    [field: SerializeField, Tag] private List<string> ActivatorTags { get; set; } = [];
    
    private void OnTriggerEnter(Collider other) {
        if (this.ActivatorTags.Count > 0 && !this.ActivatorTags.Any(other.CompareTag)) {
            return;
        }

        GoalPoint.OnReached.Invoke();       
        Object.Destroy(this.gameObject);
    }
}
