using Project.Scripts.Interaction.ObjectDetection;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait for Target to Escape", story: "[Agent] waits for [Target] to escape from [Sensor]", category: "Action/Enemy AI", id: "cac9f5114a376e3b14426ce3e87d2113")]
public partial class WaitForTargetToEscapeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject?> Target;
    [SerializeReference] public BlackboardVariable<Sensor> Sensor;

    protected override Status OnStart() {
        this.Sensor.Value.OnTargetLost += this.OnLostTarget;
        return Status.Running;
    }

    private void OnLostTarget(Collider target) {
        if (this.Target.Value == target.gameObject) {
            this.Target.Value = null;
        }
    }

    protected override Status OnUpdate() {
        return !this.Target.Value ? Status.Success : Status.Running;
    }

    protected override void OnEnd() {
        this.Sensor.Value.OnTargetLost -= this.OnLostTarget;
        base.OnEnd();   
    }
}

