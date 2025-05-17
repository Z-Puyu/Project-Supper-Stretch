using System;
using Project.Scripts.AttributeSystem.AttributeTypes;

namespace Project.Scripts.AttributeSystem;

[Serializable]
public readonly record struct AttributeInitialisation(
    AttributeType Type,
    int Value,
    AttributeType Cap = AttributeType.None
);
