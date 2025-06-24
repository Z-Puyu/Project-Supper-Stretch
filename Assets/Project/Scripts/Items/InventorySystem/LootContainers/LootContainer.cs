using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Project.Scripts.Common;
using SaintsField;
using Project.Scripts.Interaction;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.InventorySystem.LootContainers;

[DisallowMultipleComponent, RequireComponent(typeof(Inventory), typeof(InteractableObject))]
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
    
    [NotNull] private Inventory? Inventory { get; set; }
    [NotNull] private InteractableObject? Interactable { get; set; }
    private Inventory? CurrentInteractorInventory { get; set; }
    
    [NotNull] 
    [field: SerializeField] 
    public LootTable? LootTable { private get; set; }
    
    [field: SerializeField] public LootDropConfig? LootDropConfig { private get; set; }
    
    [field: SerializeField, MinMaxSlider(1, 20)] 
    private Vector2Int DropAmount { get; set; } = new Vector2Int(1, 5);
    
    /*private List<Item> AlwaysDrop { get; init; } = [];
    private List<Item> GuaranteedToExistAtLeastOne { get; init; } = [];*/
    
    private bool HasBeenOpenedBefore { get; set; }

    private void Awake() {
        this.Inventory = this.GetComponent<Inventory>();
        this.Interactable = this.GetComponent<InteractableObject>();
    }

    private void Start() {
        // TODO: Implement concrete loot drop parameters.
        this.Interactable.OnInteraction += this.Open;
    }

    public void Inject(Item item, int count) {
        this.Inventory.Add(item, count);
    }

    private float ComputeTotalWeight(LootDropParameters parameters) {
        Func<KeyValuePair<ItemData, int>, float>? weightFunction = !this.LootDropConfig 
                ? null 
                : loot => this.LootDropConfig.ComputeDropFactor(loot.Key, loot.Value, parameters);
        return this.LootTable.ComputeTotalWeight(weightFunction);
    }

    private void Open(Interactor interactor) {
        if (!this.HasBeenOpenedBefore) {
            this.DropRandom(new LootDropParameters());
        }

        this.CurrentInteractorInventory = interactor.GetComponent<Inventory>();
        // Toggle UI event only when the interactor is the player.
        if (!interactor.gameObject.CompareTag("Player")) {
            return;
        }

        Debug.Log($"Open loot container {this.Inventory}");
        LootContainer.OnOpen.Invoke(new UIData(this.Inventory, interactor.GetComponent<Inventory>()));
    }

    private void DropRandom(LootDropParameters parameters) {
        if (this.HasBeenOpenedBefore || this.LootTable.IsEmpty) {
            return;
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

                this.Inventory.Add(Item.From(item));
                break;
            }
        }
        
        this.HasBeenOpenedBefore = true;
    }
}
