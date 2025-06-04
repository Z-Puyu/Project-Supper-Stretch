using System;
using Project.Scripts.Util.BooleanLogic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.StateMachine.Variables;

[Serializable]
public sealed record class StateMachineVariableValidation<T> : StateTransitionGuard {
    private enum Verb {
        [RichLabel("=")]  
        Equals,
        
        [RichLabel("≠")]
        NotEquals,
        
        [RichLabel(">")]
        GreaterThan,
        
        [RichLabel("<")]
        LessThan,
        
        [RichLabel("≥")]
        GreaterThanOrEqualTo,
        
        [RichLabel("≤")]
        LessThanOrEqualTo,
    }
    
    [field: SerializeField]
    private string Variable { get; set; } = string.Empty;
    
    [field: SerializeField, Dropdown("GetVerbs")]
    private Verb ValidationRule { get; set; } = Verb.Equals;
    
    [field: SerializeField]
    private T? Value { get; set; }

    private DropdownList<Verb> GetVerbs() {
        bool isComparable = typeof(IComparable).IsAssignableFrom(typeof(T));
        return new DropdownList<Verb> {
            { "=", Verb.Equals },
            { "≠", Verb.NotEquals },
            { ">", Verb.GreaterThan, !isComparable },
            { "<", Verb.LessThan, !isComparable },
            { "≥", Verb.GreaterThanOrEqualTo, !isComparable },
            { "≤", Verb.LessThanOrEqualTo, !isComparable }
        };
    }
    
    public override bool Holds() {
        if (!this.Context.TryGetValue(this.Variable, out Variable value)) {
            return false; 
        }

        if (value is IComparable comparable) {
            return this.ValidationRule switch {
                Verb.Equals => comparable.CompareTo(this.Value) == 0,
                Verb.NotEquals => comparable.CompareTo(this.Value) != 0,
                Verb.GreaterThan => comparable.CompareTo(this.Value) > 0,
                Verb.LessThan => comparable.CompareTo(this.Value) < 0,
                Verb.GreaterThanOrEqualTo => comparable.CompareTo(this.Value) >= 0,
                Verb.LessThanOrEqualTo => comparable.CompareTo(this.Value) <= 0,
                var _ => false
            };
        }

        return this.ValidationRule switch {
            Verb.Equals => value.Equals(this.Value),
            Verb.NotEquals => !value.Equals(this.Value),
            var _ => false
        };
    }
}
