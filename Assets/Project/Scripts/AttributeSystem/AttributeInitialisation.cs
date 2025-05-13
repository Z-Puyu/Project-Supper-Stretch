using System;

namespace Project.Scripts.AttributeSystem;

[Serializable]
public readonly record struct AttributeInitialisation(
    AttributeType Type,
    int Value,
    AttributeType Cap = AttributeType.None
);
