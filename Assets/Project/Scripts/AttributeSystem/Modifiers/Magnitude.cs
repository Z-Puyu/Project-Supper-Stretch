using System;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public abstract class Magnitude {
    public abstract float GetValueWith(AttributeSet? attributes = null);
}