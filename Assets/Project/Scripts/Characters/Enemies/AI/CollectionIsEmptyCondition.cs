using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CollectionIsEmpty", story: "[Collection] is empty", category: "Conditions", id: "39527214a84b0f86d73488d1ccd9a7c0")]
public partial class CollectionIsEmptyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject?>?> Collection;

    public override bool IsTrue() {
        return this.Collection.Value is null || this.Collection.Value.Count(obj => obj) == 0;
    }
}
