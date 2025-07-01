using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Characters;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterMovement {
    [NotNull]
    private NavMeshAgent? Navigator { get; set; }
    
    private void Awake() {
        this.Navigator = this.GetComponent<NavMeshAgent>();
    }

    private void Start() {
        this.Navigator.speed *= this.Speed;
    }

    public override void StopImmediately() {
        this.Navigator.ResetPath();
    }

    public override void MoveTowards(Vector3 location) {
        this.Navigator.SetDestination(location);
    }
}
