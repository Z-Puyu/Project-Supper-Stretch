using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public enum ModifierType {
    [InspectorName("Base Offset")] BaseOffset, 
    [InspectorName("Multiplier")] Multiplier,
    [InspectorName("Final Offset")] FinalOffset
}
