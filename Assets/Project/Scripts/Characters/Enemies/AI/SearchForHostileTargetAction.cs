using System;
using Project.Scripts.Interaction.ObjectDetection;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SearchForHostileTarget", 
    story: "[Agent] searches for a hostile [Target]", 
    category: "Action/Enemy AI", 
    id: "d49fee93c09e8ffb504f1592345580ff")]
public partial class SearchForHostileTargetAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject?> Target;
    
    private void SpotHostileTarget(Collider target) {
        this.Target.Value = target.gameObject;
        Debug.Log($"Spotted hostile target {this.Target.Value}!");
    }

    protected override Status OnStart() {
        Sensor sensor = this.Agent.Value.GetComponentInChildren<Sensor>();
        sensor.OnDetection += this.SpotHostileTarget;
        sensor.OnLostSight += this.ResetTarget;
        return Status.Running;
    }

    private void ResetTarget(Collider target) {
        if (this.Target.Value == target.gameObject) {
            this.Target.Value = null;
        }
    }

    protected override Status OnUpdate() {
        return this.Target.Value ? Status.Success : Status.Running;
    }
}