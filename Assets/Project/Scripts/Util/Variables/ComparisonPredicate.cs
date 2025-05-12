using System;
using UnityEngine;

namespace Project.Scripts.Util.Variables;

[Serializable]
public class ComparisonPredicate<T> : Predicate<T> where T : IComparable {
    private enum Verb {
        Equal,
        Unequal,
        GreaterThan, 
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

    [field: SerializeField]
    private Verb ValidationRule { get; set; }

    public override bool Holds() {
        if (this.Subject == null) {
            return true;
        }
        
        if (this.Object == null) {
            return false;
        }
        
        T? subject = this.Subject.Read();
        T? @object = this.Object.Read();
        if (subject == null) {
            return @object == null ? this.ValidationRule == Verb.Equal : this.ValidationRule == Verb.Unequal;
        }
        
        return this.ValidationRule switch {
            Verb.Equal => subject.CompareTo(@object) == 0,
            Verb.Unequal => subject.CompareTo(@object) != 0,
            Verb.GreaterThan => subject.CompareTo(@object) > 0,
            Verb.LessThan => subject.CompareTo(@object) < 0,
            Verb.GreaterThanOrEqual => subject.CompareTo(@object) >= 0,
            Verb.LessThanOrEqual => subject.CompareTo(@object) <= 0,
            var _ => throw new ArgumentOutOfRangeException()
        };
    }
}

[Serializable] public class IntegerComparison : ComparisonPredicate<int>;
[Serializable] public class FloatComparison : ComparisonPredicate<float>;
[Serializable] public class EnumComparison : ComparisonPredicate<Enum>;
