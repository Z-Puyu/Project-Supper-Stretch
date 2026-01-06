using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    public enum ModifierType {
        [InspectorName("Set Base Value")] SetBase = -1,
        Shift = 0,
        Multiplier = 1,
        Offset = 2
    }
}