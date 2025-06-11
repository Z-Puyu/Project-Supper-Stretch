using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.InteractionSystem;
using Project.Scripts.Items;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.InventorySystem.LootContainers;

[RequireComponent(typeof(Inventory), typeof(InteractableObject))]
public class LootContainer : MonoBehaviour {
    public sealed record class UIData(Inventory LootInventory, Inventory PlayerInventory);
    
    public static event UnityAction<UIData> OnOpen = delegate { };
    
    [NotNull] private Inventory? Inventory { get; set; }
    [NotNull] private InteractableObject? Interactable { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private LootTable? LootTable { get; set; }
    
    [field: SerializeField] private LootDropConfig? LootDropConfig { get; set; }
    
    [field: SerializeField, MinMaxSlider(1, 20)] 
    private Vector2Int DropAmount { get; set; } = new Vector2Int(1, 5);
    
    public bool IsInitialised { get; private set; }

    private void Awake() {
        this.Inventory = this.GetComponent<Inventory>();
        this.Interactable = this.GetComponent<InteractableObject>();
    }

    private void Start() {
        // TODO: Implement concrete loot drop parameters.
        this.Interactable.OnInteraction += this.Open;
    }

    private float ComputeTotalWeight(LootDropParameters parameters) {
        Func<KeyValuePair<ItemData, int>, float>? weightFunction = !this.LootDropConfig 
                ? null 
                : loot => this.LootDropConfig.ComputeDropFactor(loot.Key, loot.Value, parameters);
        return this.LootTable.ComputeTotalWeight(weightFunction);
    }

    private void Open(Interactor interactor) {
        if (!this.IsInitialised) {
            this.DropRandom(new LootDropParameters());
        }

        // Toggle UI event only when the interactor is the player.
        if (!interactor.gameObject.CompareTag("Player")) {
            return;
        }

        Debug.Log($"Open loot container {this.Inventory}");
        LootContainer.OnOpen.Invoke(new UIData(this.Inventory, interactor.GetComponent<Inventory>()));
    }

    public void DropRandom(LootDropParameters parameters) {
        if (this.IsInitialised) {
            Debug.Log("Loot container is already initialised. Should not drop loot again.");
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
        
        this.IsInitialised = true;
    }
}
