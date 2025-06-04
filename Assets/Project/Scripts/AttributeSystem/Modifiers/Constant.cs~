using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public class Constant : Magnitude {
    [field: SerializeField]
    private int Value { get; set; }
    
    public override float Evaluate() {
        return this.Value;
    }

    public override float Evaluate(Enum tag) {
        return this.Value;
    }

    public override string ToString() {
        return $"{this.Value}";
    }
}
