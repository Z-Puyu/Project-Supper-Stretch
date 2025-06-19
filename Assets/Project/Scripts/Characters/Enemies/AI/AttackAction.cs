using System;
using Project.Scripts.Characters.CharacterControl.Combat;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] attacks [Target]", category: "Action/Enemy AI", id: "64f1defd48762d439fba54fe0f368ed6")]
public partial class AttackAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private bool IsAttacking { get; set; }
    private ComboAttack? ComboAttack { get; set; }

    private void FinishAttack() {
        this.IsAttacking = false;
    }
    
    protected override Status OnStart() {
        base.OnStart();
        this.IsAttacking = true;
        this.ComboAttack = this.Agent.Value.GetComponent<ComboAttack>();
        this.ComboAttack.CommitRandomStage();
        this.ComboAttack.OnAttackCommitted += this.FinishAttack;
        return Status.Success;
    }
    
    protected override Status OnUpdate() {
        base.OnUpdate();
        return this.IsAttacking ? Status.Running : Status.Success;
    }

    protected override void OnEnd() {
        base.OnEnd();
        this.ComboAttack!.OnAttackCommitted -= this.FinishAttack;
    }
}