using System;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public abstract class CustomExecution : EffectExecution {
    [field: SerializeField] protected AttributeRelation? AttributeRelation { get; private set; }
}