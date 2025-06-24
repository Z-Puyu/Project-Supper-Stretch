using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public readonly record struct ModifierKey(string TargetAttribute, ModifierType Type) {
    public override string ToString() {
        return $"{this.TargetAttribute} {this.Type}";
    }
}
