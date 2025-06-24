using System;
using System.Collections.Generic;
using System.Text;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.CraftingSystem;

[CreateAssetMenu(fileName = "Cooking Method", menuName = "Crafting/Cooking Method")]
public class CookingMethod : ScriptableObject {
    private enum Category {
        Salad,
        Boil,
        Roast,
        Steam
    }

    [field: SerializeField] private Category FoodType { get; set; } = Category.Salad;
    [field: SerializeField] private string Name { get; set; } = string.Empty;
    [field: SerializeField] private string AlternativeName { get; set; } = string.Empty;
    [field: SerializeField] public int BaseCraftTime { get; private set; } = 1;
    [field: SerializeField] public List<Modifier> FoodModifiers { get; private set; } = [];

    private string SelectName() {
        if (this.AlternativeName == string.Empty) {
            return this.Name;
        }

        return Random.Range(-1, 1) >= 0 ? this.Name : this.AlternativeName;
    }

    public string FormatFoodName(List<Item> ingredients) {
        int nMainIngredients = ingredients.Count;
        string type = this.SelectName();
        StringBuilder sb = new StringBuilder();
        if (nMainIngredients >= 2) {
            sb.Append($"{ingredients[0].Name} and {ingredients[1].Name}");
        } else {
            sb.Append(ingredients[0].Name);
        }

        string main = sb.ToString();
        sb.Clear();
        if (nMainIngredients > 2) {
            sb.AppendJoin(", ", ingredients.Slice(2, nMainIngredients - 1));
            sb.Append($" and {ingredients[^1].Name}");
        }

        string side = sb.ToString();
        return this.FoodType switch {
            Category.Boil or Category.Salad when side == string.Empty => side == string.Empty
                    ? $"{main} {type}"
                    : $"{main} {type} with {side}",
            Category.Roast or Category.Steam when side == string.Empty => side == string.Empty
                    ? $"{type} {main}"
                    : $"{type} {main} with {side}",
            var _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string ToString() {
        return this.Name;
    }
}
