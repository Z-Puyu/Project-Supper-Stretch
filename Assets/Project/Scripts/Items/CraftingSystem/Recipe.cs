using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Util.Linq;

namespace Project.Scripts.Items.CraftingSystem;

public class Recipe : IPresentable {
    private ItemType ItemType { get; init; }
    private ItemType IngredientType { get; init; }
    private List<Item> Ingredients { get; init; } = [];
    public ModifierManager ModifierManager { get; init; } = new ModifierManager();
    public CookingMethod? CookingMethod { get; set; }

    public string FoodName => this.IsEmpty || !this.CookingMethod
            ? string.Empty
            : this.CookingMethod.FormatFoodName(this[this.IngredientType].Distinct().ToList());
    
    private int RawCost => this.Ingredients.Sum(item => item.Worth);
    private bool IsEmpty => this.Ingredients.Any(item => item.Type == this.IngredientType);
    private IEnumerable<Item> this[ItemType type] => this.Ingredients.Where(item => item.Type == type);

    public Recipe(ItemType type, ItemType ingredientType) {
        this.ItemType = type;
        this.IngredientType = ingredientType;
    }

    public void SwitchMethod(CookingMethod? prev, CookingMethod method) {
        if (prev) {
            prev.FoodModifiers.ForEach(this.ModifierManager.Remove);
        }
        
        method.FoodModifiers.ForEach(this.ModifierManager.Add);
        this.CookingMethod = method;
    }

    public void AddIngredient(Item item) {
        this.Ingredients.Add(item);
        item.Properties.ForEach(this.ModifierManager.Add);
    }

    public void RemoveIngredient(Item item) {
        if (!this.Ingredients.Remove(item)) {
            throw new ArgumentException($"Item {item} is not in the recipe");
        }
        
        item.Properties.ForEach(this.ModifierManager.Remove);
    }

    public Item Cook() {
        return new Item(this.ItemType, this.FoodName, this.RawCost, this.ModifierManager.NetModifiers);
    }

    public string FormatAsText() {
        StringBuilder sb = new StringBuilder(this.FoodName);
        Dictionary<ModifierKey, Modifier> modifiers = [];
        foreach (Modifier m in this.ModifierManager.NetModifiers) {
            if (!modifiers.TryAdd(m.Key, m)) {
                modifiers[m.Key] += m;
            }
        }

        foreach (Modifier m in modifiers.Values) {
            sb.AppendLine(m.ToString());
        }
        
        return sb.ToString();
    }
}
