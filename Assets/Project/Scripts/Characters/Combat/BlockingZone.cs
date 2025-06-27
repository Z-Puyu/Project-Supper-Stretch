using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public class BlockingZone : MonoBehaviour {
    [field: SerializeField] private int BlockingAngle { get; set; }
    private bool IsBlocking { get; set; }
    [NotNull] private Transform? Self { get; set; }
    
    private void Awake() {
        this.Self = this.transform;
    }

    public bool HasBlocked(Vector3 damageSource) {
        float angle = Vector3.Angle(this.Self.forward, damageSource - this.Self.position);
        return angle < this.BlockingAngle;
    }
}
