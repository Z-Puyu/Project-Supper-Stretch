using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFrameworks.CommonUtilities.Processors;
using CommonFrameworks.Trees;
using GameplayAbilitiesSystem.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    [DisallowMultipleComponent]
    public class ModifierEnvironment : MonoBehaviour, IModifiable {
        private sealed class Node {
            internal ModifierValue[] Modifiers { get; } = {
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0)
            };

            private LinkedList<ModifierValue> Overrides { get; } = new LinkedList<ModifierValue>();

            private Dictionary<ModifierValue, Stack<LinkedListNode<ModifierValue>>> OverrideRecords { get; } =
                new Dictionary<ModifierValue, Stack<LinkedListNode<ModifierValue>>>();

            internal bool HasOverride(out ModifierValue value) {
                if (this.Overrides.Count == 0) {
                    value = default;
                    return false;
                }
                
                value = this.Overrides.Last.Value;
                return true;
            }

            internal void AddOverride(ModifierValue value) {
                LinkedListNode<ModifierValue> node = this.Overrides.AddLast(value);
                if (!this.OverrideRecords.TryGetValue(value, out Stack<LinkedListNode<ModifierValue>> nodes)) {
                    nodes = new Stack<LinkedListNode<ModifierValue>>();
                    this.OverrideRecords.Add(value, nodes);
                }
                
                nodes.Push(node);
            }
            
            internal void RemoveOverride(ModifierValue value) {
                if (!this.OverrideRecords.TryGetValue(value, out Stack<LinkedListNode<ModifierValue>> nodes)) {
                    return;
                }
                
                this.Overrides.Remove(nodes.Pop());
                if (nodes.Count == 0) {
                    this.OverrideRecords.Remove(value);
                }
            }
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

            if (modifier.Type == ModifierType.Override) {
                node.AddOverride(modifier.Value);
            } else {
                node.Modifiers[(int)modifier.Type] += modifier.Value;
            }

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
            IProcessor<Attribute>[] processorList = processors.ToArray();
            if (this.Modifiers.TryGetValue(attribute.Id, out Node node) &&
                node.HasOverride(out ModifierValue @override)) {
                attribute = update(@override, ModifierType.Override);
            } else {
                this.CollectModifiers(attribute.Id, modifiers);
                if (!this.IsGlobalEnvironment && this.ParentEnvironment) {
                    return this.ParentEnvironment.Query(attribute, modifiers, processorList);
                }

                for (ModifierType op = ModifierType.Shift; op < ModifierType.Override; op += 1) {
                    attribute = update(modifiers[(int)op], op);
                }
            }

            return attribute;

            Attribute update(ModifierValue modifier, ModifierType op) {
                double value = modifier.ApplyTo(attribute.Value, op);
                attribute = new Attribute(attribute.Source, attribute.Id, value, attribute.IsValueApproximated);
                return processorList.Aggregate(attribute, (current, processor) => processor.Process(current));
            }
        }

        public Attribute Query(Attribute @base, IEnumerable<IProcessor<Attribute>> processors) {
            ModifierValue[] modifiers = {
                new ModifierValue(0),
                new ModifierValue(0),
                new ModifierValue(0)
            };

            return this.Query(@base, modifiers, processors);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Modifiers on {this.gameObject.name}:\n", this.Modifiers.Count + 1);
            foreach (KeyValuePair<AttributeKey, Node> entry in this.Modifiers) {
                for (ModifierType op = ModifierType.Shift; op < ModifierType.Override; op += 1) {
                    sb.Append($"|{entry.Key}:{op} = {entry.Value.Modifiers[(int)op]} ");
                }

                if (entry.Value.HasOverride(out ModifierValue @override)) {
                    sb.AppendLine($"|{entry.Key}:{ModifierType.Override} = {@override}");
                }
            }
            
            return sb.ToString();
        }
    }
}
