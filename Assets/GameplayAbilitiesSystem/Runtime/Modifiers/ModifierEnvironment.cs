using System;
using System.Collections.Generic;
using System.Text;
using CommonFrameworks.Collections;
using CommonFrameworks.Maths;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using Attribute = GameplayAbilitiesSystem.Runtime.Attributes.Attribute;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    [DisallowMultipleComponent]
    public class ModifierEnvironment : MonoBehaviour, IModifiable {
        [field: SerializeField] private bool IsGlobalEnvironment { get; set; }

        [field: SerializeField, Required, HideIf(nameof(this.IsGlobalEnvironment))]
        private ModifierEnvironment? ParentEnvironment { get; set; }

        private TrieDictionary<AttributeKey, char, Node> Modifiers { get; } =
            new TrieDictionary<AttributeKey, char, Node>();

        public event UnityAction<AttributeKey> OnModifierUpdated = delegate { };

        public void AddModifier(Modifier modifier) {
            if (modifier.Type == ModifierType.SetBase) {
                Debug.LogError($"Cannot set the base attribute value via an {nameof(ModifierEnvironment)}.", this);
            }
            
            if (modifier.Value == 0) {
                return;
            }

            if (!this.Modifiers.TryGetValue(modifier.Target, out Node node)) {
                node = this.Modifiers.FindLongestPrefixKey(modifier.Target, out KeyValuePair<AttributeKey, Node> pair)
                        ? pair.Value.Duplicate()
                        : new Node();
                this.Modifiers.Add(modifier.Target, node);
            }

            node.Modifiers[(int)modifier.Type] += modifier.Value;
            this.OnModifierUpdated.Invoke(modifier.Target);
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
            ref AttributeQuery query, ModifierValue[] modifiers, in IEvaluable<IAttributeReader>? max,
            in IEvaluable<IAttributeReader>? min, in AttributeApproximator? approximator
        ) {
            this.CollectModifiers(query.Id, modifiers);
            if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                return this.ParentEnvironment.Query(ref query, modifiers, max, min, approximator);
            }

            for (ModifierType op = ModifierType.Shift; op < ModifierType.Offset; op += 1) {
                double value = modifiers[(int)op].ApplyTo(query.Value, op);
                if (max is not null) {
                    value = Math.Min(max.Evaluate(query.Source), value);
                }

                if (min is not null) {
                    value = Math.Max(min.Evaluate(query.Source), value);
                }

                Attribute attribute = new Attribute(query.Source, query.Id, value, query.IsValueApproximated);
                approximator?.Approximate(ref attribute);
                query.Value = attribute.Value;
                query.IsValueApproximated = attribute.HasBeenApproximated;
            }

            return new Attribute(query.Source, query.Id, query.Value, query.IsValueApproximated);
        }

        internal Attribute Query(
            ref AttributeQuery query, in IEvaluable<IAttributeReader>? max, in IEvaluable<IAttributeReader>? min,
            in AttributeApproximator? approximator
        ) {
            ModifierValue[] modifiers = { ModifierValue.Zero, ModifierValue.Zero, ModifierValue.Zero };
            return this.Query(ref query, modifiers, max, min, approximator);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Modifiers on {this.gameObject.name}:\n", this.Modifiers.Count + 1);
            foreach (KeyValuePair<AttributeKey, Node> entry in this.Modifiers) {
                for (ModifierType op = ModifierType.Shift; op < ModifierType.Offset; op += 1) {
                    sb.Append($"|{entry.Key}:{op} = {entry.Value.Modifiers[(int)op]} ");
                }
            }

            return sb.ToString();
        }

        private sealed class Node {
            internal ModifierValue[] Modifiers { get; } = {
                ModifierValue.Zero, ModifierValue.Zero, ModifierValue.Zero
            };

            internal Node Duplicate() {
                Node node = new Node();
                for (int i = 0; i < this.Modifiers.Length; i += 1) {
                    node.Modifiers[i] = this.Modifiers[i];
                }

                return node;
            }
        }
    }
}
