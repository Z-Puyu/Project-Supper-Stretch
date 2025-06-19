using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Util.Linq;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Collect Nearby Objects with Tag", 
    story: "Collect [K] objects with tag [Tag] within range [Range] from [Anchor] into [Collection]", 
    category: "Action/Find", 
    id: "0aff30231a8c997f4bc94c6419b7e212")]
public partial class CollectNearbyObjectsWithTagAction : Action {
    [SerializeReference] public BlackboardVariable<int> K;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Anchor;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Collection;
    [SerializeReference] public BlackboardVariable<bool> RandomSelection = new BlackboardVariable<bool>(true);

    protected override Status OnStart() {
        if (!this.Anchor.Value) {
            this.LogFailure("No anchor object provided.");
            return Status.Failure;
        }

        if (this.K.Value <= 0) {
            this.LogFailure("The number of objects to collect must be greater than 0.");
            return Status.Failure;
        }

        Vector3 anchor = this.Anchor.Value.transform.position;
        GameObject[] all = GameObject.FindGameObjectsWithTag(this.Tag.Value);
        if (all.Length == 0) {
            return Status.Failure;
        }
        
        IList<GameObject> objects = this.Range > 0 
                ? all.Where(obj => Vector3.Distance(anchor, obj.transform.position) <= this.Range).ToList()
                : all;
        if (this.RandomSelection && objects.Count > 1) {
            objects.Shuffle();
        }
        
        int count = Mathf.Min(this.K.Value, objects.Count);
        this.Collection.Value.AddRange(objects.Take(count));
        return this.Collection.Value.Count > 0 ? Status.Success : Status.Failure;
    }
}