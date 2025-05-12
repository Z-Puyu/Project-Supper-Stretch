using System;
using UnityEngine;

namespace Project.Scripts.Util.Variables;

[Serializable]
public class EqualityPredicate<T> : Predicate<T> {
    public override bool Holds() {
        if (this.Subject == null) {
            return true;
        }
        
        if (this.Object == null) {
            return false;
        }
        
        T? subject = this.Subject.Read();
        T? @object = this.Object.Read();
        return subject?.Equals(@object) ?? @object == null;
    }
}

[Serializable] public class IntegerEquality : EqualityPredicate<int>;
[Serializable] public class FloatEquality : EqualityPredicate<float>;
[Serializable] public class BooleanEquality : EqualityPredicate<bool>;
[Serializable] public class StringEquality : EqualityPredicate<string>;
[Serializable] public class ObjectEquality : EqualityPredicate<object>;
[Serializable] public class UnityObjectEquality : EqualityPredicate<UnityEngine.Object>;
[Serializable] public class Vector3Equality : EqualityPredicate<Vector3>;
[Serializable] public class Vector2Equality : EqualityPredicate<Vector2>;
[Serializable] public class FlagEquality : EqualityPredicate<Enum>;
