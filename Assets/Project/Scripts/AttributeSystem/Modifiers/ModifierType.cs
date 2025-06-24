using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public enum ModifierType {
    [InspectorName("Base Offset")] BaseOffset, 
    [InspectorName("Multiplier")] Multiplier,
    [InspectorName("Modifier Scaling")] ModifierScaling,
    [InspectorName("Final Offset")] FinalOffset
}
