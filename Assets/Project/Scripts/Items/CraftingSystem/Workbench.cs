using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent]
public class Workbench : MonoBehaviour {
    private List<string> Moves { get; init; } = [];
    public (Item? ingredient, bool isRemoved) LastOperation { get; private set; } = (null, false);
    public Recipe? Recipe { get; private set; }
    public float Cost { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllSchemes))] 
    private List<string> Schemes { get; set; } = [];
    
    [field: SerializeReference] 
    private ItemProducer? Producer { get; set; }

    public Dictionary<Modifier, int> Modifiers { get; init; } = [];
    
    private AdvancedDropdownList<string> AllSchemes => ObjectCache<SchemeDefinition>.Instance.Objects.LeafTags();
    
    public event Action<int, Item> OnCraft = delegate { };
    public event Action<int, Recipe> OnRecipeChanged = delegate { };

    private void Start() {
        this.NewSession();
    }

    private void NewSession() {
        this.Recipe = new Recipe();
    }

    public void Put(Item ingredient) {
        if (!ingredient.Type.HasFlag(ItemFlag.CraftingMaterial)) {
            Logging.Error($"{ingredient} is not a crafting material", this);
            return;
        }
        
        this.Recipe?.AddIngredient(ingredient);
        this.LastOperation = (ingredient, false);
        ingredient.Process(this);
        this.Moves.Add($"Add {ingredient.Name}");
        this.NotifyRecipeChange();
    }

    public void Remove(Item ingredient) {
        if (!ingredient.Type.HasFlag(ItemFlag.CraftingMaterial)) {
            Logging.Error($"{ingredient} is not a crafting material", this);
            return;
        }
        
        this.Recipe?.RemoveIngredient(ingredient);
        this.LastOperation = (ingredient, true);
        ingredient.Process(this);
        this.Moves.Add($"Remove {ingredient.Name}");
        this.NotifyRecipeChange();
    }

    public bool TryProduce(out Item item) {
        if (this.Producer is null || this.Recipe is null) {
            Logging.Error("No producer or recipe to produce.", this);
            item = Item.New("", "", 0);
            return false;
        }

        if (this.Recipe.IsEmpty) {
            item = Item.New("", "", 0);
            return false;
        }
        
        item = this.Producer.Produce(this.Recipe, this.Modifiers.Select(pair => pair.Key * pair.Value));
        return true;
    }

    public void Craft() {
        if (!this.TryProduce(out Item product)) {
            return;
        }
        
        this.OnCraft.Invoke(0, product);
        this.NewSession();
    }

    public void ChangeScheme(int idx) {
        if (this.Recipe is null) {
            Logging.Error("No recipe to change scheme.", this);
            return;
        }
        
        Scheme? current = this.Recipe.Scheme;
        Scheme? next = this.Schemes[idx].Definition<SchemeDefinition, Scheme>();
        if (current == next) {
            return;
        }

        if (next is null) {
            Logging.Error($"No scheme with tag {this.Schemes[idx]}", this);
            return;
        }

        if (current is not null) {
            this.Cost -= current.BaseCraftCost;
        }
        
        this.Recipe.Scheme = next;
        this.Cost += next.BaseCraftCost;
        this.NotifyRecipeChange();
    }

    private void NotifyRecipeChange() {
        if (this.Recipe is null) {
            throw new InvalidOperationException("No recipe to notify.");
        }
        
        int cost = this.Recipe.IsEmpty ? 0 : Mathf.Max(1, Mathf.FloorToInt(this.Cost));
        this.OnRecipeChanged.Invoke(cost, this.Recipe);
    }
}