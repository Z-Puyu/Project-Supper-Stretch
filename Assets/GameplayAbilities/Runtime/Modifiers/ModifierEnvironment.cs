using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameplayAbilities.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Modifiers {
    [DisallowMultipleComponent]
    public class ModifierEnvironment : MonoBehaviour, IModifiable {
        [field: SerializeField] private bool IsGlobalEnvironment { get; set; }
        [field: SerializeField] private ModifierEnvironment? ParentEnvironment { get; set; }

        private IDictionary<GameplayAttributeType, Node> Modifiers { get; } =
            new Dictionary<GameplayAttributeType, Node>();

        public event UnityAction<GameplayAttributeType> OnModifierUpdated = delegate { };

        public void AddModifier(GameplayAttributeType target, Modifier modifier) {
            if (modifier.Type == ModifierType.SetBase) {
                Debug.LogError($"Cannot set the base attribute value via an {nameof(ModifierEnvironment)}.", this);
            }
            
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
                    query.Modifiers[modifier.Priority] += modifier.Value;
                }
            }
            
            if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                this.ParentEnvironment.Query(ref query);
            } 
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Modifiers on {this.gameObject.name}:\n", this.Modifiers.Count + 1);
            foreach (KeyValuePair<GameplayAttributeType, Node> entry in this.Modifiers) {
                for (ModifierType op = ModifierType.Shift; op < ModifierType.Offset; op += 1) {
                    sb.Append($"|{entry.Key}:{op} = {entry.Value[op].Value} ");
                }
            }

            return sb.ToString();
        }

        private sealed class Node : IEnumerable<Modifier> {
            private double Shift { get; set; } = 0;
            private double Multiplier { get; set; } = 0;
            private double PositiveOffset { get; set; } = 0;
            private double NegativeOffset { get; set; } = 0;

            internal Modifier this[ModifierType op] => op switch {
                ModifierType.Shift => new Modifier(op, this.Shift),
                ModifierType.Multiplier => new Modifier(op, this.Multiplier),
                ModifierType.Offset => new Modifier(op, this.PositiveOffset + this.NegativeOffset),
                var _ => throw new ArgumentOutOfRangeException(nameof(op), op, string.Empty)
            };

            internal void Add(Modifier mod) {
                switch (mod.Type) {
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
