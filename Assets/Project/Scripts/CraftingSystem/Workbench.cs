using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items;
using UnityEngine;
using Attribute = Project.Scripts.AttributeSystem.Attributes.Attribute;

namespace Project.Scripts.CraftingSystem;

[RequireComponent(typeof(AttributeSet))]
public class Workbench : MonoBehaviour {
    public static event Action<Workbench, Recipe> OnCraft = delegate { };
    
    private Recipe? Recipe { get; set; }
    [NotNull] private AttributeSet? AttributeSet { get; set; }

    [field: SerializeField] public int MaxMainIngredients { get; private set; } = 3;
    [field: SerializeField] public int MaxSpices { get; private set; } = 2;
    [field: SerializeField] public int MaxAdditives { get; private set; } = 2;

    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
    }

    public void NewSession() {
        this.Recipe = new Recipe();
    }

    public (Item item, int count) Craft() {
        if (this.Recipe is null) {
            throw new ArgumentException("No recipe to craft.");
        }
        
        Workbench.OnCraft.Invoke(this, this.Recipe);
        List<Modifier> modifiers = this.AttributeSet.Select(asCharacterModifier).ToList();
        IEnumerable<Modifier> reversedModifiers = modifiers.Select(m => -m);
        // TODO: Integrate systems.
        Item food = new Item(ItemType.Food, this.Recipe.FoodName, this.Recipe.FoodWorth, []);
        
        return (food, 1);
        
        static Modifier asCharacterModifier(Attribute attribute) => 
                Modifier.Builder.Of(attribute.CurrentValue, (CharacterAttribute)attribute.Type).FinalOffset();
    }
    
    public void AddMainIngredient(ItemData item) {
        if (this.Recipe is null) {
            this.NewSession();
        } else if (this.Recipe.Mains.Count < this.MaxMainIngredients) {
            this.Recipe.Mains.Add(item);
        }
    }
    
    public void AddSpice(ItemData item) {
        if (this.Recipe is null) {
            this.NewSession();
        } else if (this.Recipe.Spices.Count < this.MaxSpices) {
            this.Recipe.Spices.Add(item);
        }
    }
    
    public void AddAdditive(ItemData item) {
        if (this.Recipe is null) {
            this.NewSession();
        } else if (this.Recipe.Additives.Count < this.MaxAdditives) {
            this.Recipe.Additives.Add(item);
        }
    }
}