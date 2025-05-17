using System;
using System.Collections.Generic;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public abstract class Magnitude {
    public abstract float Evaluate();

    public virtual Magnitude BasedOn(Attributes.AttributeManagementSystem? self, Attributes.AttributeManagementSystem target) {
        return this;
    }

    public virtual Magnitude With(string label, int magnitude) {
        return this;
    }

    public virtual Magnitude With(IReadOnlyDictionary<string, int> magnitudes) {
        return this;
    }
}
