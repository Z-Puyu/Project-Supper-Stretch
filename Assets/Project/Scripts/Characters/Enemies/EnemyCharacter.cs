using SaintsField;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem.LootContainers;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies;

public class EnemyCharacter : GameCharacter<Enemy> {
    [field: SerializeField, Required] private LootContainer? LootContainer { get; set; }
    
    protected override void Awake() {
        base.Awake();
        this.Animator.runtimeAnimatorController = this.CharacterData.Animations;
        if (!this.LootContainer) {
            this.LootContainer = this.GetComponentInChildren<LootContainer>();
        }
    }
    
    protected override void Start() {
        base.Start();
        this.LootContainer!.LootTable = this.CharacterData.LootTable;
        this.LootContainer.gameObject.SetActive(false);
    }

    protected override void DyingFrom(GameObject? source) {
        foreach ((Item item, int count) in this.Inventory.Items) {
            this.LootContainer!.Inject(item, count);    
        }
        
        base.DyingFrom(source);
    }
}
