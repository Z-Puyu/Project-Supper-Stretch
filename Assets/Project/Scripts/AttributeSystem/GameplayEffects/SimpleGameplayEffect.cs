using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
public abstract class SimpleGameplayEffect : GameplayEffect {
    [field: SerializeField]
    private List<Modifier> Modifiers { get; set; } = [];

    public override IEnumerable<Modifier> Invoke(
        Attributes.AttributeManagementSystem? instigator, Attributes.AttributeManagementSystem target,
        IReadOnlyDictionary<string, int> magnitudes, int chance = 100
    ) {
        if (chance <= 0 || chance < 100 && UnityEngine.Random.Range(0, 100) < chance) {
            return [];
        }

        return this.Modifiers.Select(configured);

        Modifier configured(Modifier m) =>
                Modifier.Configurator.Of(m).BasedOn(instigator, target).AccordingTo(magnitudes).Build();
    }
}
