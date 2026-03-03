using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GameplayAbilities.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Modifiers {
    [DisallowMultipleComponent]
    public class ModifierEnvironment : MonoBehaviour, IModifiable {
        [field: SerializeField] private bool IsGlobalEnvironment { get; set; }
        [field: SerializeField] private ModifierEnvironment? ParentEnvironment { get; set; }

        private IDictionary<GameplayAttributeType, Node> Modifiers { get; } =
            new ConcurrentDictionary<GameplayAttributeType, Node>();

        public event UnityAction<GameplayAttributeType> OnModifierUpdated = delegate { };

        public void AddModifier(GameplayAttributeType target, Modifier modifier) {
            if (modifier.Value == 0) {
                return;
            }

            if (!this.Modifiers.TryGetValue(target, out Node node)) {
                this.Modifiers.Add(target, node = new Node());
            }

            node.Add(modifier);
            this.OnModifierUpdated.Invoke(target);
        }

        internal void Query(ref AttributeQuery query) {
            if (this.Modifiers.TryGetValue(query.AttributeType, out Node node)) {
                foreach (Modifier modifier in node) {
                    query.AddModifier(modifier);
                }
            }

            if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                this.ParentEnvironment.Query(ref query);
            }
        }

        private sealed class Node : IEnumerable<Modifier> {
            internal double BaseValueOverride { get; private set; } = 0;
            private double Shift { get; set; } = 0;
            private double Multiplier { get; set; } = 0;
            private double PositiveOffset { get; set; } = 0;
            private double NegativeOffset { get; set; } = 0;

            internal void Add(Modifier mod) {
                switch (mod.Type) {
                    case ModifierType.SetBase:
                        this.BaseValueOverride = mod.Value;
                        break;
                    case ModifierType.Shift:
                        this.Shift += mod.Value;
                        break;
                    case ModifierType.Multiplier:
                        this.Multiplier += mod.Value;
                        break;
                    case ModifierType.Offset when mod.Value >= 0 && Math.Abs(this.NegativeOffset) >= mod.Value:
                        this.NegativeOffset += mod.Value;
                        break;
                    case ModifierType.Offset when mod.Value >= 0 && Math.Abs(this.NegativeOffset) < mod.Value:
                        this.PositiveOffset += mod.Value + this.NegativeOffset;
                        this.NegativeOffset = 0;
                        break;
                    case ModifierType.Offset when mod.Value < 0:
                        this.NegativeOffset += mod.Value;
                        break;
                }
            }

            public IEnumerator<Modifier> GetEnumerator() {
                yield return new Modifier(ModifierType.SetBase, this.BaseValueOverride);
                yield return new Modifier(ModifierType.Shift, this.Shift);
                yield return new Modifier(ModifierType.Multiplier, this.Multiplier);
                yield return new Modifier(ModifierType.Offset, this.PositiveOffset);
                yield return new Modifier(ModifierType.Offset, this.NegativeOffset);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }
        }
    }
}
