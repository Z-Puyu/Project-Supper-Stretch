using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using CommonFrameworks.Processors;
using GameplayAbilitiesSystem.Runtime.Attributes.Processors;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [DisallowMultipleComponent, RequireComponent(typeof(ModifierEnvironment))]
    public sealed class AttributeSet : BehaviourComponent, IAttributeReader, IModifiable {
        private TrieDictionary<AttributeKey, char, Node> Attributes { get; } =
            new TrieDictionary<AttributeKey, char, Node>('/');

        [field: SerializeField] private AttributeTable? DefaultBaseAttributes { get; set; }
    
        [field: SerializeField, Required]
        [field: InfoBox(
            "Attribute values are approximated by this after change." +
            "You can overwrite this in specific attribute types."
        )]
        private AttributeApproximator Approximator { get; set; } = new AttributeApproximator();

        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }

        public event UnityAction<AttributeChange>? OnAttributeUpdated;

        protected override void Awake() {
            base.Awake();
            this.ModifierEnvironment = this.GetOrAddComponent<ModifierEnvironment>();
            this.ModifierEnvironment.OnModifierUpdated += this.UpdateAttribute;
            if (this.Attributes.Count == 0 && this.DefaultBaseAttributes) {
                this.DefaultBaseAttributes.Initialise(this);
            }
        }

        private void PostAttributeUpdate(Attribute attribute) {
            if (!attribute.HasBeenApproximated) {
                this.Approximator.Process(ref attribute);
            }
            
            this.Attributes[attribute.Id].Value = attribute.Value;
        }

        private void UpdateAttribute(AttributeKey key) {
            if (!this.Attributes.TryGetValue(key, out Node node)) {
                node = new Node(0);
                this.Attributes.Add(key, node);
            }

            double oldValue = node.Value;
            AttributeQuery query = new AttributeQuery(this.Owner, this, key, node.BaseValue);
            this.PostAttributeUpdate(this.ModifierEnvironment.Query(ref query, node.Processors));
            this.OnAttributeUpdated?.Invoke(new AttributeChange(key, oldValue, node.Value));
        }

        public double Query(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) ? node.Value : 0;
        }

        public bool HasAtLeast(double threshold, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) && node.Value >= threshold;
        }

        private void SetBase(AttributeKey key, double value) {
            if (!this.Attributes.TryGetValue(key, out Node node)) {
                node = new Node(value);
                this.Attributes.Add(key, node);
            } else {
                node.BaseValue = value;
            }
            
            this.UpdateAttribute(key);
        }

        public void Clear() {
            this.Attributes.Clear();
        }

        internal void Initialise(AttributeType attributeType, double value) {
            if (this.Attributes.ContainsKey(attributeType.Id)) {
                Debug.LogError($"Attribute {attributeType.Id} is already initialised", this.Owner);
            } else {
                Node node = new Node(value, attributeType.Processors);
                this.Attributes.Add(attributeType.Id, node);
                this.UpdateAttribute(attributeType.Id);
            }
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Attributes.Select(entry => new Attribute(this, entry.Key, entry.Value.Value, true))
                       .GetEnumerator();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Attributes on {this.gameObject.name}:\n", this.Attributes.Count + 1);
            foreach (KeyValuePair<AttributeKey, Node> entry in this.Attributes) {
                sb.AppendLine($"|{entry.Key}: {entry.Value.Value}");
            }
            
            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        void IModifiable.AddModifier(Modifier modifier) {
            if (modifier.Type == ModifierType.SetBase) {
                this.SetBase(modifier.Target, modifier.Value);
            } else {
                this.ModifierEnvironment.AddModifier(modifier);
            }
        }
        
        private sealed class Node {
            internal double BaseValue { get; set; }
            internal double Value { get; set; }
            internal List<IProcessor<Attribute>> Processors { get; } = new List<IProcessor<Attribute>>();

            internal Node(double initialValue, IEnumerable<IProcessor<Attribute>>? processors = null) {
                this.BaseValue = initialValue;
                if (processors is not null) {
                    this.Processors.AddRange(processors);
                }
            }
        }
    }
}