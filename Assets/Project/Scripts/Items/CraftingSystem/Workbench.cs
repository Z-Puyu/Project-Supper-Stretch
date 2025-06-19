using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
public class Workbench : MonoBehaviour {
    private Recipe? Recipe { get; set; }
    [NotNull] private AttributeSet? AttributeSet { get; set; }
    [field: SerializeField] private List<CookingMethod> CookingMethods { get; set; } = [];
    [field: SerializeField] public int MaxMainIngredients { get; private set; } = 3;
    [field: SerializeField] public int MaxSpices { get; private set; } = 2;
    [field: SerializeField] public int MaxAdditives { get; private set; } = 2;
    
    [field: SerializeField, Header("Attributes")]
    [field: AdvancedDropdown(nameof(this.AttributeSet.AllAccessibleAttributes))]
    private AttributeKey FoodCostAttribute { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AttributeSet.AllAccessibleAttributes))]
    private AttributeKey CookingTimeAttribute { get; set; }

    [field: SerializeField, AdvancedDropdown(nameof(this.AttributeSet.AllAccessibleAttributes))]
    private List<AttributeKey> FoodPropertyAttributes { get; set; } = [];
    
    [field: SerializeField, Header("Gameplay Effects")] 
    private GameplayEffect? OnChangeCookingMethod { get; set; }
    
    [field: SerializeField]
    private GameplayEffect? OnConsumeFood { get; set; }
    
    public event Action<int, Item> OnCraft = delegate { };
    public event Action<Recipe> OnRecipeChanged = delegate { };

    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
    }

    public Recipe NewSession() {
        return this.Recipe = new Recipe();
    }

    public void Craft() {
        if (this.Recipe is null) {
            throw new ArgumentException("No recipe to craft.");
        }
        
        int cost = this.AttributeSet.ReadCurrent(this.FoodCostAttribute.FullName);
        IEnumerable<Modifier> modifiers = this.FoodPropertyAttributes.Select(toModifier);
        Item food = new Item(ItemType.Food, this.Recipe.FoodName, cost, [], [this.OnConsumeFood]);
        this.OnCraft.Invoke(this.AttributeSet.ReadCurrent(this.CookingTimeAttribute.FullName), food);
        return;

        Modifier toModifier(AttributeKey key) =>
                Modifier.Builder.Of(this.AttributeSet.ReadCurrent(key.FullName), key).Build();
    }

    public void ChangeCookingMethod(int idx) {
        this.Recipe ??= this.NewSession();
        CookingMethod? current = this.Recipe.CookingMethod;
        CookingMethod next = this.CookingMethods[idx];
        if (current == next) {
            return;
        }

        if (this.OnChangeCookingMethod) {
            GameplayEffectExecutionArgs args = !current
                    ? new GameplayEffectExecutionArgs.Builder().WithCustomModifiers(next.Properties).Build()
                    : new GameplayEffectExecutionArgs.Builder()
                      .WithCustomModifiers(current.Properties.Select(modifier => -modifier))
                      .WithCustomModifiers(next.Properties).Build();
            this.AttributeSet.AddEffect(this.OnChangeCookingMethod, args);
        }
        
        this.Recipe.CookingMethod = this.CookingMethods[idx];
        this.OnRecipeChanged.Invoke(this.Recipe);
    }
    
    public void AddIngredient(Item item) {
        this.Recipe ??= this.NewSession(); 
        this.Recipe.AddIngredient(item);
        this.OnRecipeChanged.Invoke(this.Recipe);
    }
}