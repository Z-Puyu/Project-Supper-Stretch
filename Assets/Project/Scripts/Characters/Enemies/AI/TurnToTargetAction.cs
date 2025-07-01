using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TurnToTarget", story: "[Agent] turns to [Target]", category: "Action", id: "48a4163ce3020b6bc19b38ee69d600f8")]
public partial class TurnToTargetAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    
    [NotNull] private Transform? SelfTransform { set; get; }
    private Quaternion TargetRotation { get; set; }

    protected override Status OnStart() {
        this.SelfTransform = this.Agent.Value.transform;
        Vector3 direction = (this.Target.Value.transform.position - this.Agent.Value.transform.position) with { y = 0 };
        if (this.SelfTransform.forward == direction.normalized) {
            return Status.Success;    
        }
        
        this.TargetRotation = Quaternion.LookRotation(direction);
        return Status.Running;
    }

    protected override Status OnUpdate() {
        Vector3 direction = (this.Target.Value.transform.position - this.Agent.Value.transform.position) with { y = 0 };
        this.TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = this.SelfTransform.rotation;
        rotation = Quaternion.Lerp(rotation, this.TargetRotation, 0.5f);
        this.SelfTransform.rotation = rotation;
        float angle = Quaternion.Angle(rotation, this.TargetRotation);
        return Mathf.Approximately(angle, 0) ? Status.Success : Status.Running;
    }
}

