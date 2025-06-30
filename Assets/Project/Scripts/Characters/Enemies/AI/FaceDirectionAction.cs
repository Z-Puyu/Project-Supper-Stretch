using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Face Direction", story: "[Agent] turns to face [Direction]", category: "Action", id: "ccf6f27a0270b79111be93e2a928f2cf")]
public partial class FaceDirectionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Direction;
    [NotNull] private Transform? SelfTransform { set; get; }
    private Quaternion TargetRotation { get; set; }

    protected override Status OnStart() {
        this.SelfTransform = this.Agent.Value.transform;
        if (this.SelfTransform.forward == this.Direction.Value.normalized) {
            return Status.Success;    
        }
        
        this.TargetRotation = Quaternion.LookRotation(this.Direction.Value);
        return Status.Running;
    }

    protected override Status OnUpdate() {
        this.SelfTransform.rotation = Quaternion.Slerp(this.SelfTransform.rotation, this.TargetRotation, 0.1f);
        return this.SelfTransform.rotation == this.TargetRotation ? Status.Success : Status.Running;
    }
}

