using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.Items.CraftingSystem;

public class Recipe {
    public CookingMethod? CookingMethod { get; set; }
    public Dictionary<ItemType, List<Item>> Ingredients { get; private init; } = [];
    public int CookingTime { get; set; }

    public string FoodName => this.IsEmpty || !this.CookingMethod
            ? string.Empty
            : this.CookingMethod.FormatFoodName(this[ItemType.FoodIngredient].Distinct().ToList());
    
    public int RawCost => this.Ingredients.Values.Sum(list => list.Sum(item => item.Worth));
    public bool IsEmpty => this[ItemType.FoodIngredient].Count == 0;
    
    public List<Item> this[ItemType type] => this.Ingredients.TryGetValue(type, out List<Item> items) ? items : [];

    public void AddIngredient(Item item) {
        if (!this.Ingredients.TryGetValue(item.Type, out List<Item> items)) {
            this.Ingredients[item.Type] = [item];
        } else {
            items.Add(item);
        }
    }
}
