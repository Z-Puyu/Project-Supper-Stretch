using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.Localisation;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[CreateAssetMenu(fileName = "ModifierLocalisations", menuName = "Attribute System/Modifier Localisation Mapping")]
public class ModifierLocalisationMapping : GameplayTagLocalisation<AttributeType, Modifier> {
    [field: SerializeField]
    private List<ModifierType> IncludedModifierTypes { get; set; } =
        [..Enum.GetValues(typeof(ModifierType)).Cast<ModifierType>()];
    
    public override string Map(Modifier key) {
        return this.Entries.GetValueOrDefault(key.Key.ToString(), "Unknown modifier {value}");   
    }

    protected override void Rewrite(AttributeType node, IDictionary<string, string> current) {
        foreach (ModifierType type in this.IncludedModifierTypes) {
            string key = new ModifierKey(node.Name, type).ToString();
            if (current.TryGetValue(key, out string? value)) {
                this.Entries.Add(key, value);
            } else {
                this.Entries.Add(key, $"{key} {{value}}");
            }
        }
    }
}
