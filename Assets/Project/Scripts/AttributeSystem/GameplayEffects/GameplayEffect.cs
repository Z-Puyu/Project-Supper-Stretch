using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public abstract class GameplayEffect : ScriptableObject, IVisitor<AttributeSet> {
    [field: SerializeField]
    private List<Modifier> Modifiers { get; set; } = [];

    [field: SerializeField, Range(1, 100)]
    private int Chance { get; set; } = 100;
    
    private bool IsSuccessfullyApplied => UnityEngine.Random.Range(0, 100) < this.Chance;
    
    public void ConfigureModifierMagnitude(string label, int magnitude) {
        this.Modifiers.ForEach(modifier => modifier.ConfigureMagnitude(label, magnitude));
    }

    public void Visit(AttributeSet attributes) {
        if (!this.IsSuccessfullyApplied) {
            return;
        }
        
        this.Modifiers.ForEach(attributes.ModifierManager.Accept);
    }
}
