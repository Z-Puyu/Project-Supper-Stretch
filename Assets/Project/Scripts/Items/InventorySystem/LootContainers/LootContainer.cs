using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Project.Scripts.Common;
using SaintsField;
using Project.Scripts.Interaction;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Singleton;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.InventorySystem.LootContainers;

[DisallowMultipleComponent, RequireComponent(typeof(InteractableObject))]
public class LootContainer : MonoBehaviour {
    public sealed record class UIData(Inventory Loot, Inventory Inventory) : IPresentable {
        public string FormatAsText() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Loot Container:");
            sb.AppendLine(this.Loot.FormatAsText());
            sb.AppendLine("Inventory:");
            sb.AppendLine(this.Inventory.FormatAsText());
            return sb.ToString();
        }
    }
    
    public static event UnityAction<UIData> OnOpen = delegate { };
    
    [field: SerializeField] private Inventory? OwnerInventory { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private Inventory? LocalInventory { get; set; }
    
    [NotNull] private InteractableObject? Interactable { get; set; }
    private Inventory? CurrentInteractorInventory { get; set; }
    
    [field: SerializeField] private LootTable? LootTable { get; set; }
    
    [field: SerializeField] public LootDropConfig? LootDropConfig { private get; set; }
    
    [field: SerializeField, MinMaxSlider(1, 20)] 
    private Vector2Int DropAmount { get; set; } = new Vector2Int(1, 5);
    
    private bool HasBeenOpenedBefore { get; set; }

    private void Awake() {
        if (!this.LocalInventory) {
            this.LocalInventory = this.GetComponentInChildren<Inventory>();
        }
        
        this.Interactable = this.GetComponent<InteractableObject>();
    }

    private void Start() {
        if (this.OwnerInventory) {
            foreach ((Item item, int count) in this.OwnerInventory.Items) {
                this.Inject(item, count);    
            }
        }
        
        // TODO: Implement concrete loot drop parameters.
        this.Interactable.OnInteraction += this.Open;
    }

    public void Inject(Item item, int count) {
        this.LocalInventory.Add(item, count);
    }

    public void Inject(LootTable table) {
        if (!this.LootTable) {
            this.LootTable = table;
        }
    }

    private float ComputeTotalWeight(LootDropParameters parameters) {
        if (!this.LootTable) {
            return 0;
        }
        
        return this.LootTable.ComputeTotalWeight(parameters);
    }

    private void Open(Interactor interactor) {
        if (!this.HasBeenOpenedBefore) {
            int level = interactor.GetSiblingComponent<ExperienceSystem>().CurrentLevel;
            this.DropRandom(new LootDropParameters(level));
        }

        this.CurrentInteractorInventory = interactor.GetSiblingComponent<Inventory>();
        // Toggle UI event only when the interactor is the player.
        if (!interactor.gameObject.CompareTag("Player")) {
            return;
        }

        Debug.Log($"Open loot container {this.LocalInventory}");
        LootContainer.OnOpen.Invoke(new UIData(this.LocalInventory, this.CurrentInteractorInventory));
    }

    private void DropRandom(LootDropParameters parameters) {
        if (this.HasBeenOpenedBefore || !this.LootTable || this.LootTable.IsEmpty) {
            return;
        }

        foreach (ItemData item in this.LootTable.AlwaysDrop) {
            this.LocalInventory.Add(Item.From(item));    
        }
        
        int count = Random.Range(this.DropAmount.x, this.DropAmount.y + 1);
        float total = this.ComputeTotalWeight(parameters);
        for (int i = 0; i < count; i += 1) {
            float select = Random.Range(0, total);
            float current = 0;
            foreach ((ItemData item, int weight) in this.LootTable) {
                current += weight;
                if (select >= current) {
                    continue;
                }

                this.LocalInventory.Add(Item.From(item));
                break;
            }
        }
        
        this.HasBeenOpenedBefore = true;
    }
}
