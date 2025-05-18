using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

internal abstract class CustomExecutionGameplayEffect : GameplayEffect {
    [field: SerializeReference, SubclassSelector]
    private List<GameplayEffectExecutor> Executors { get; set; } = [];
    
    [field: SerializeReference, SubclassSelector]
    private List<AffectedAttribute> AffectedAttributes { get; set; } = [];

    public override IEnumerable<Modifier> Invoke(
        AttributeManagementSystem? instigator, AttributeManagementSystem target,
        IReadOnlyDictionary<string, int> references, int chance = 100
    ) {
        CapturedAttributeData attributes = new CapturedAttributeData(instigator, target);
        IDictionary<string, int> parameters = new Dictionary<string, int>();
        foreach (AffectedAttribute attribute in this.AffectedAttributes) {
            if (references.TryGetValue(attribute.Label, out int value)) {
                parameters.Add(attribute.Label, value);
            } else {
                parameters.Add(attribute.Label, 0);
            }
        }
        
        IReadOnlyDictionary<string, int> readonlyParameters = new ReadOnlyDictionary<string, int>(parameters);
        foreach (GameplayEffectExecutor executor in this.Executors) {
            executor.Execute(attributes, parameters, ref chance);
        }

        return this.AffectedAttributes.Distinct().Select(toModifier);

        Modifier toModifier(AffectedAttribute a) {
            Modifier m = new RuntimeModifier(a.AttributeSetTag, a.EnumAttribute, a.ModifierType, a.Label);
            return Modifier.Configurator.Of(m).AccordingTo(readonlyParameters).Build();
        }
    }
}
