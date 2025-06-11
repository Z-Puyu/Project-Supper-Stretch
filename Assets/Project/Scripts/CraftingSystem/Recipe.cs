using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Items;
using UnityEngine;

namespace Project.Scripts.CraftingSystem;

public class Recipe {
    private const float LabourCostMultiplier = 1.25f;
    
    public enum CookingMethod {
        None,
        Boiling,
        Roasting,
        Steaming
    }
    
    public CookingMethod FoodType { get; set; }
    public List<ItemData> Mains { get; init; } = [];
    public List<ItemData> Spices { get; init; } = [];
    public List<ItemData> Additives { get; init; } = [];
    public string FoodName => this.GetFoodName();
    public int FoodWorth => this.GetFoodWorth();

    private string GetFoodName() {
        string type = this.FoodType switch {
            CookingMethod.Boiling when this.Mains.Count > 1 => "Stew",
            CookingMethod.Boiling when this.Mains.Count <= 1 => "Boiled",
            CookingMethod.Roasting => "Roasted",
            CookingMethod.Steaming => "Steamed",
            CookingMethod.None => "Salad",
            var _ => string.Empty
        };
        
        string main = this.Mains.Count switch {
            1 => this.Mains[0].Name,
            > 1 => $"{this.Mains[0].Name} and {this.Mains[1].Name}",
            var _ => throw new ArgumentOutOfRangeException()
        };

        string side = this.Mains.Count switch {
            3 => this.Mains[2].Name,
            > 3 => $"{string.Join(" ,", this.Mains)}, and {this.Mains[^1].Name}",
            var _ => string.Empty
        };

        return this.FoodType switch {
            CookingMethod.Boiling or CookingMethod.None when side == string.Empty => $"{main} {type}",
            CookingMethod.Boiling or CookingMethod.None when side != string.Empty => $"{main} {type} with {side}",
            CookingMethod.Roasting or CookingMethod.Steaming when side == string.Empty => $"{type} {main}",
            CookingMethod.Roasting or CookingMethod.Steaming when side != string.Empty => $"{type} {main} with {side}",
            var _ => throw new ArgumentOutOfRangeException()
        };
    }

    private int GetFoodWorth() {
        float cost = this.Mains.Sum(item => item.Worth) + 
                     this.Spices.Sum(item => item.Worth) +
                     this.Additives.Sum(item => item.Worth);
        return Mathf.CeilToInt(cost * Recipe.LabourCostMultiplier);
    }
}
