using System.Collections.Generic;
using Project.Scripts.Audio;
using Project.Scripts.Characters.Enemies.AI;
using Project.Scripts.Items.InventorySystem.LootContainers;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Singleton;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Characters.Enemies;

public class EnemyCharacter : GameCharacter<Enemy> {
    private static HashSet<EnemyCharacter> AggressiveEnemies { get; } = [];

    [field: SerializeField] private EngagedInCombat? OnCombatStatusChanged { get; set; }
    
    protected override void Start() {
        base.Start();
        if (!this.HasChildComponent(out LootContainer loot)) {
            return;
        }

        if (this.OnCombatStatusChanged) {
            this.OnCombatStatusChanged.Event += this.RegisterEnemy;
        }

        loot.Inject(this.CharacterData.LootTable!);
    }

    private void OnDestroy() {
        if (this.OnCombatStatusChanged) {
            this.OnCombatStatusChanged.Event -= this.RegisterEnemy;
        }
    }

    private void RegisterEnemy(GameObject agent, GameObject _, bool isEngaged) {
        if (agent != this.gameObject) {
            return;
        }
                
        if (isEngaged) {
            EnemyCharacter.AggressiveEnemies.Add(this);
            if (EnemyCharacter.AggressiveEnemies.Count == 1) {
                Singleton<AmbientTrack>.Instance.TransitionToCombat();
            }
        } else {
            EnemyCharacter.AggressiveEnemies.Remove(this);
            if (EnemyCharacter.AggressiveEnemies.Count == 0) {
                Singleton<AmbientTrack>.Instance.TransitionToBackground();
            }
        }
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

    protected override void DyingFrom(GameObject? source) {
        base.DyingFrom(source);
        EnemyCharacter.AggressiveEnemies.Remove(this);
        if (EnemyCharacter.AggressiveEnemies.Count == 0) {
            Singleton<AmbientTrack>.Instance.TransitionToBackground();
        }
    }
}
