using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[RequireComponent(typeof(AttributeSet))]
public abstract class GameplayEffectInstigator : MonoBehaviour {
    [NotNull]
    private AttributeSet? SelfAttributes { get; set; }

    private void Awake() {
        this.SelfAttributes = this.GetComponent<AttributeSet>();
    }

    public abstract void Instigate();
}
