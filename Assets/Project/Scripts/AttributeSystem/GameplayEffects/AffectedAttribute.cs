using System;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public readonly record struct AffectedAttribute(AttributeType Attribute, Modifier.Type ModifierType, string Label);
