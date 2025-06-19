using System;
using Unity.Behavior;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NullityCheck", story: "[Object] [Predicate] null", category: "Conditions", 
    id: "9d12405dd151052814ed98c94127767c")]
public partial class NullityCheckCondition : Condition {
    public enum Verb { Is, Not }
    
    [SerializeReference] public BlackboardVariable<GameObject>? Object;
    [SerializeReference] public BlackboardVariable<Verb> Predicate;
    
    public override bool IsTrue() {
        return this.Object is null || !this.Object.Value ? this.Predicate == Verb.Is : this.Predicate == Verb.Not;
    }
}