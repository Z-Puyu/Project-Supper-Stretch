using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.Items.CraftingSystem;

public class Recipe {
    public Scheme? Scheme { get; set; }
    private List<Item> Ingredients { get; init; } = [];

    public string FoodName => this.IsEmpty || this.Scheme is null
            ? string.Empty
            : this.Scheme.FormatName(this.Ingredients);
    
    public bool IsEmpty => !this.Ingredients.Any();

    public void AddIngredient(Item item) {
        this.Ingredients.Add(item);
    }

    public void RemoveIngredient(Item item) {
        if (!this.Ingredients.Remove(item)) {
            throw new ArgumentException($"Item {item} is not in the recipe");
        }
    }

    public Item Cook(string type) {
        return Item.New(type, this.Scheme?.FormatName(this.Ingredients) ?? "Undefined", 0);
    }
}
