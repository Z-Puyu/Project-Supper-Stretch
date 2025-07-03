using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.Common;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public class AttributeBasedHealth : Health {
    [NotNull]
    [field: SerializeField, Required] 
    private AttributeSet? AttributeSet { get; set; }

    [field: SerializeField, HideIf(nameof(this.AttributeSet), null), AdvancedDropdown(nameof(this.AllAttributes))] 
    private string HealthAttribute { get; set; } = string.Empty;
    
    private string MaxHealthAttribute { get; set; } = string.Empty;

    protected override bool IsAttributeBased => true;

    private AdvancedDropdownList<string> AllAttributes =>
            this.AttributeSet ? this.AttributeSet.AllAccessibleAttributes : [];

    protected override void Start() {
        base.Start();
        this.AttributeSet.OnAttributeChanged += handleAttributeChange;
        return;

        void handleAttributeChange(AttributeChange change) {
            if (change.Type != this.HealthAttribute && change.Type != this.MaxHealthAttribute) {
                return;
            }

            this.UpdateMaxHealth(this.AttributeSet.ReadMax(this.HealthAttribute));
            this.UpdateHealth(this.AttributeSet.ReadCurrent(this.HealthAttribute));
        }
    }

    public override void Initialise() {
        if (this.AttributeSet.Defined[this.HealthAttribute].HowToClamp == AttributeType.ClampPolicy.CapByAttribute) {
            this.MaxHealthAttribute = this.AttributeSet.Defined[this.HealthAttribute].Cap;
        }
        
        this.UpdateMaxHealth(this.AttributeSet.ReadMax(this.HealthAttribute));
    }

    protected override void TakeDamage(Damage damage, HitBoxTag where = HitBoxTag.Generic) {
        base.TakeDamage(damage, where);
        if (!damage.Source || !damage.Source.TryGetComponent(out IAttributeReader instigator)) {
            Logging.Error($"Cannot execute damage because {damage.Source} cannot access any attribute set", this);
            return;
        }

        if (!damage.Effect) {
            Logging.Error("Attribute-based health requires a gameplay effect on incoming damage", this);
            return;
        }
            
        // Health component does not handle damage gameplay effects directly but delegates them to the AttributeSet.
        GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(instigator)
                                                                      .OfLevel(damage.Multiplier / 100.0f).Build();
        this.AttributeSet.AddEffect(damage.Effect, instigator, args);
    }

    public override void Heal(int amount, GameObject? source) {
        throw new System.NotImplementedException();
    }
}
