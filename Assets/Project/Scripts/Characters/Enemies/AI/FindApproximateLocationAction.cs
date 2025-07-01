using System;
using Project.Scripts.Util.Components;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindApproximateLocation", story: "Find some [Point] within [Distance] from [Location]", category: "Action/Enemy AI", id: "030b0d867459a157c4b96cbace5fffb8")]
public partial class FindApproximateLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Point;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<GameObject> Location;

    protected override Status OnStart() {
        return Status.Running;
    }
    
    protected override Status OnUpdate() {
        if (!this.Location.Value.transform.position.FoundRandomPoint(this.Distance, out Vector3 point)) {
            return Status.Running;
        }

        this.Point.Value = point;
        return Status.Success;
    }
}