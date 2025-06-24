using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent]
public class Workbench : MonoBehaviour {
    private Recipe? Recipe { get; set; }
    private float CraftTime { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))] 
    private ItemType ProductItemType { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))] 
    private ItemType IngredientItemType { get; set; }
    
    [field: SerializeField] private List<CookingMethod> CookingMethods { get; set; } = [];
    [field: SerializeField] public int MaxMainIngredients { get; private set; } = 3;
    [field: SerializeField] public int MaxSpices { get; private set; } = 2;
    [field: SerializeField] public int MaxAdditives { get; private set; } = 2;

    private AdvancedDropdownList<ItemType> AllItemTypes => ObjectCache<ItemDefinition>.Instance.Objects.LeafNodes();
    
    public event Action<int, Item> OnCraft = delegate { };
    public event Action<int, Recipe> OnRecipeChanged = delegate { };

    private void Start() {
        this.NewSession();
    }

    public void NewSession() {
        this.Recipe = new Recipe(this.ProductItemType, this.IngredientItemType);
    }

    public void Craft() {
        if (this.Recipe is null) {
            Logging.Error("No recipe to craft.", this);
            return;
        }

        this.OnCraft.Invoke(Mathf.FloorToInt(this.CraftTime), this.Recipe.Cook());
        this.NewSession();
    }

    public void ChangeCookingMethod(int idx) {
        if (this.Recipe is null) {
            Logging.Error("No recipe to change cooking method.", this);
            return;
        }
        
        CookingMethod? current = this.Recipe.CookingMethod;
        CookingMethod next = this.CookingMethods[idx];
        if (current == next) {
            return;
        }

        if (current) {
            this.CraftTime -= current.BaseCraftTime;
        }
        
        this.CraftTime += this.CookingMethods[idx].BaseCraftTime;
        this.Recipe.SwitchMethod(current, next);
        this.OnRecipeChanged.Invoke(Mathf.FloorToInt(this.CraftTime), this.Recipe);
    }
    
    public void AddIngredient(Item item) {
        if (!item.Type.Flags.HasFlag(ItemFlag.CraftingMaterial)) {
            Logging.Error($"{item} is not a food ingredient", this);
            return;
        }

        if (this.Recipe is null) {
            Logging.Error("No recipe to add ingredient.", this);
            return;
        }

        this.CraftTime += item.CraftTime;
        this.Recipe.AddIngredient(item);
        this.OnRecipeChanged.Invoke(Mathf.FloorToInt(this.CraftTime), this.Recipe);
    }

    public void RemoveIngredient(Item item) {
        if (!item.Type.Flags.HasFlag(ItemFlag.CraftingMaterial)) {
            Logging.Error($"{item} is not a food ingredient", this);
            return;
        }

        if (this.Recipe is null) {
            Logging.Error("No recipe to remove ingredient.", this);
            return;
        }
        
        this.CraftTime -= item.CraftTime;
        this.Recipe.RemoveIngredient(item);
        this.OnRecipeChanged.Invoke(Mathf.FloorToInt(this.CraftTime), this.Recipe);
    }
}