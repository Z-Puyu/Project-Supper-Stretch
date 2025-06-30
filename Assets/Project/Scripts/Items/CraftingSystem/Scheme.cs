using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.CraftingSystem;

[Serializable]
public class Scheme : GameplayTagNode {
    [field: SerializeField] private List<string> PossibleProductNames { get; set; } = [];
    [field: SerializeField] public int BaseCraftCost { get; private set; } = 1;
    [field: SerializeField] public List<Modifier> Modifiers { get; private set; } = [];
    
    public override IList<GameplayTagNode> Children => [];
    
    protected override void OnRename() {
        this.TracePath<SchemeDefinition, Scheme>();
    }

    public string FormatName(List<Item> ingredients) {
        string name = this.PossibleProductNames[Random.Range(0, this.PossibleProductNames.Count)];
        if (ingredients.Count == 1) {
            return $"{ingredients[0].Name} {name}";
        }
        
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin(", ", ingredients.Take(ingredients.Count - 1).Select(ingredient => ingredient.Name))
          .Append(ingredients.Count > 2 ? ", " : " ")
          .Append($"and {ingredients[^1].Name}");
        return $"{sb} {name}";
    }

    public override string ToString() {
        return this.Name;
    }
}
