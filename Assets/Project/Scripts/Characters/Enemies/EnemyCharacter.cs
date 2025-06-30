using Project.Scripts.Interaction.ObjectDetection;
using SaintsField;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Items.InventorySystem.LootContainers;
using Project.Scripts.Util.Components;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Characters.Enemies;

public class EnemyCharacter : GameCharacter<Enemy> {
    protected override void Awake() {
        base.Awake();
        this.Animator.runtimeAnimatorController = this.CharacterData.Animations;
    }
    
    protected override void Start() {
        base.Start();
        if (!this.HasChildComponent(out LootContainer loot)) {
            return;
        }

        loot.Inject(this.CharacterData.LootTable!);
    }

    protected override void OnPause() {
        base.OnPause();
        if (this.TryGetComponent(out NavMeshAgent navmeshAgent)) {
            navmeshAgent.enabled = false;
        }

        if (this.TryGetComponent(out BehaviorGraphAgent agent)) {
            agent.enabled = false;
        }
    }
    
    protected override void OnPlay() {
        base.OnPlay();
        if (this.TryGetComponent(out NavMeshAgent navmeshAgent)) {
            navmeshAgent.enabled = true;
        }   
        
        if (this.TryGetComponent(out BehaviorGraphAgent agent)) {
            agent.enabled = true;
        }
    }
}
