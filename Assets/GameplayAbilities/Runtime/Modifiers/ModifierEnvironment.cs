using System;
using System.Collections.Generic;
using System.Text;
using CommonFrameworks.Collections;
using CommonFrameworks.Maths;
using GameplayAbilities.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Modifiers {
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

            node.Add(modifier);
            this.OnModifierUpdated.Invoke(modifier.Target);
        }

        private void CollectModifiers(AttributeKey attribute, (ModifierValue modifier, ModifierType op)[] modifiers) {
            if (!this.Modifiers.TryGetValue(attribute, out Node node)) {
                return;
            }

            for (int i = 0; i < node.Modifiers.Length; i += 1) {
                modifiers[i].modifier += node.Modifiers[i].modifier;
            }
        }

        private void Query(
            ref AttributeQuery query, (ModifierValue modifier, ModifierType op)[] modifiers, 
            in IEvaluable<IAttributeReader>? max, in IEvaluable<IAttributeReader>? min
        ) {
            this.CollectModifiers(query.Id, modifiers);
            if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                this.ParentEnvironment.Query(ref query, modifiers, max, min);
            } else {
                foreach ((ModifierValue modifier, ModifierType op) in modifiers) {
                    double value = modifier.ApplyTo(query.Value, op);
                    if (max is not null) {
                        value = Math.Min(max.Evaluate(query.Source), value);
                    }

                    if (min is not null) {
                        value = Math.Max(min.Evaluate(query.Source), value);
                    }

                    query.Value = value;
                }
            }
        }

        internal void Query(
            ref AttributeQuery query, in IEvaluable<IAttributeReader>? max, in IEvaluable<IAttributeReader>? min
        ) {
            (ModifierValue modifier, ModifierType op)[] modifiers = {
                (ModifierValue.Zero, ModifierType.Shift), 
                (ModifierValue.Zero, ModifierType.Multiplier), 
                (ModifierValue.Zero, ModifierType.Offset), 
                (ModifierValue.Zero, ModifierType.Offset)
            };
            
            this.Query(ref query, modifiers, max, min);
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
            private ModifierValue Shift { get; set; } = ModifierValue.Zero;
            private ModifierValue Multiplier { get; set; } = ModifierValue.Zero;
            private ModifierValue PositiveOffset { get; set; } = ModifierValue.Zero;
            private ModifierValue NegativeOffset { get; set; } = ModifierValue.Zero;

            internal (ModifierValue modifier, ModifierType op)[] Modifiers => new[] {
                (this.Shift, ModifierType.Shift), 
                (this.Multiplier, ModifierType.Multiplier),
                (this.PositiveOffset, ModifierType.Offset), 
                (this.NegativeOffset, ModifierType.Offset)
            };

            internal void Add(Modifier modifier) {
                switch (modifier.Type) {
                    case ModifierType.Shift:
                        this.Shift += modifier.Value;
                        break;
                    case ModifierType.Multiplier:
                        this.Multiplier += modifier.Value;
                        break;
                    case ModifierType.Offset when modifier.Value >= 0 && -this.NegativeOffset > modifier.Value:
                        this.NegativeOffset += modifier.Value;
                        break;
                    case ModifierType.Offset when modifier.Value >= 0 && -this.NegativeOffset < modifier.Value:
                        this.PositiveOffset += modifier.Value + this.NegativeOffset;
                        this.NegativeOffset = ModifierValue.Zero;
                        break;
                    case ModifierType.Offset when modifier.Value >= 0 && -this.NegativeOffset == modifier.Value:
                        this.NegativeOffset = ModifierValue.Zero;
                        break;
                    case ModifierType.Offset when modifier.Value < 0:
                        this.NegativeOffset += modifier.Value;
                        break;
                }
            }

            internal Node Duplicate() {
                return new Node {
                    Shift = this.Shift,
                    Multiplier = this.Multiplier,
                    PositiveOffset = this.PositiveOffset,
                    NegativeOffset = this.NegativeOffset
                };
            }
        }
    }
}
