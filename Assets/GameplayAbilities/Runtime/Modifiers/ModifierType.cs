using UnityEngine;

namespace GameplayAbilities.Modifiers {
    public enum ModifierType {
        [InspectorName("Set Base Value")] SetBase = 0,
        Shift = 1,
        Multiplier = 2,
        Offset = 3
    }
}