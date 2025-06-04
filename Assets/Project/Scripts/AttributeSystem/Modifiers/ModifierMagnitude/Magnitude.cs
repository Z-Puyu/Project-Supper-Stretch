using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public abstract class Magnitude {
    public abstract float Value { get; }

    public virtual Magnitude BasedOn(AttributeSet? self, AttributeSet target) {
        return this;
    }

    public virtual Magnitude With(string label, int magnitude) {
        return this;
    }

    public virtual Magnitude With(IReadOnlyDictionary<string, int> magnitudes) {
        return this;
    }
}
