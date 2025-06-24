using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem.LootContainers;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Pooling;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Enemies;

public class EnemyCharacter : GameCharacter<Enemy> {
    [field: SerializeField, Required] private LootContainer? LootContainer { get; set; }
    
    protected override void Awake() {
        base.Awake();
        this.Animator!.runtimeAnimatorController = this.CharacterData.Animations;
        if (!this.LootContainer) {
            this.LootContainer = this.GetComponentInChildren<LootContainer>();
        }
    }
    
    protected override void Start() {
        base.Start();
        this.LootContainer!.LootTable = this.CharacterData.LootTable;
        this.GetComponent<NavMeshAgent>().enabled = true;
        this.GetComponent<BehaviorGraphAgent>().enabled = true;
        this.LootContainer.gameObject.SetActive(false);
    }

    protected override void DyingFrom(GameObject? source) {
        foreach ((Item item, int count) in this.Inventory.AllItems) {
            this.LootContainer!.Inject(item, count);    
        }
        
        base.DyingFrom(source);
    }
}
