using System;
using System.Collections.Generic;
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
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ingredients.Count - 1; i += 1) {
            sb.Append($"{ingredients[i].Name}, ");
        }
        
        sb.Append($"and {ingredients[^1].Name}");
        return $"{sb} {name}";
    }

    public override string ToString() {
        return this.Name;
    }
}
