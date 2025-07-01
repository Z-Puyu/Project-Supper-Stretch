using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Cache Current Transform", story: "[Agent] caches current [Position] and [Forward] direction", category: "Action", id: "9f190181ea521028241ad882fcd07f18")]
public partial class CacheCurrentTransformAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<Vector3> Forward;
    
    protected override Status OnStart() {
        this.Position.Value = this.Agent.Value.transform.position;
        this.Forward.Value = this.Agent.Value.transform.forward;
        return Status.Success;
    }
}

