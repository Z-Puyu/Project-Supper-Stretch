using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.CommonUtilities.Processors;
using CommonFrameworks.Trees;
using GameplayAbilitiesSystem.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    [DisallowMultipleComponent]
    public class ModifierEnvironment : MonoBehaviour {
        private sealed class Node {
            internal ModifierValue[] Modifiers { get; } = new ModifierValue[4] {
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0)
            };
        }

        [field: SerializeField] private bool IsGlobalEnvironment { get; set; }

        [field: SerializeField, Required, HideIf(nameof(this.IsGlobalEnvironment))]
        private ModifierEnvironment ParentEnvironment { get; set; }

        private TrieDictionary<AttributeKey, char, Node> Modifiers { get; } =
            new TrieDictionary<AttributeKey, char, Node>();

        public event UnityAction<AttributeKey> OnModifierUpdated;

        public void AddModifier(Modifier modifier) {
            if (modifier.Value == 0) {
                return;
            }

            if (!this.Modifiers.TryGetValue(modifier.Target, out Node node)) {
                node = new Node();
                this.Modifiers.Add(modifier.Target, node);
            }

            node.Modifiers[(int)modifier.Type] += modifier.Value;
            this.OnModifierUpdated?.Invoke(modifier.Target);
        }

        private void CollectModifiers(AttributeKey attribute, ModifierValue[] modifiers) {
            if (!this.Modifiers.TryGetValue(attribute, out Node node)) {
                return;
            }

            for (int i = 0; i < node.Modifiers.Length; i += 1) {
                modifiers[i] += node.Modifiers[i];
            }
        }

        private Attribute Query(
            Attribute attribute, ModifierValue[] modifiers, IEnumerable<IProcessor<Attribute>> processors
        ) {
            this.CollectModifiers(attribute.Id, modifiers);
            IProcessor<Attribute>[] processorList = processors.ToArray();
            if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                return this.ParentEnvironment.Query(attribute, modifiers, processorList);
            }
            
            for (ModifierType op = ModifierType.Shift; op <= ModifierType.Override; op += 1) {
                double value = modifiers[(int)op].ApplyTo(attribute.Value, op);
                attribute = new Attribute(attribute.Source, attribute.Id, value, attribute.IsValueApproximated);
                attribute = processorList.Aggregate(attribute, (current, processor) => processor.Process(current));
            }

            return attribute;
        }

        public Attribute Query(Attribute @base, IEnumerable<IProcessor<Attribute>> processors) {
            ModifierValue[] modifiers = {
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0)
            };

            return this.Query(@base, modifiers, processors);
        }
    }
}
