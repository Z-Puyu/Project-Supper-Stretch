using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CommonFrameworks.Extensions;
using CommonFrameworks.Processors;
using CommonFrameworks.Trees;
using GameplayAbilitiesSystem.Runtime.Attributes.Processors;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [DisallowMultipleComponent, RequireComponent(typeof(ModifierEnvironment))]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader, IModifiable {
        private sealed class Node {
            internal double BaseValue { get; set; }
            internal double Value { get; set; }
            internal List<IProcessor<Attribute>> Processors { get; } = new List<IProcessor<Attribute>>();
        }

        private TrieDictionary<AttributeKey, char, Node> Attributes { get; } =
            new TrieDictionary<AttributeKey, char, Node>('.');

        [field: SerializeField] private AttributeTable? DefaultBaseAttributes { get; set; }
    
        [field: SerializeField, Required]
        [field: InfoBox(
            "Attribute values are approximated by this after change." +
            "You can overwrite this in specific attribute types."
        )]
        private AttributeApproximator Approximator { get; set; } = new AttributeApproximator();

        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }

        public event UnityAction<AttributeChange>? OnAttributeUpdated;

        private void Awake() {
            this.ModifierEnvironment = this.GetOrAddComponent<ModifierEnvironment>();
            this.ModifierEnvironment.OnModifierUpdated += this.UpdateAttribute;
        }

        private void OnEnable() {
            if (this.Attributes.Count == 0 && this.DefaultBaseAttributes) {
                this.DefaultBaseAttributes.Initialise(this);
            }
        }

        private void RegisterModifierEnvironment(ModifierEnvironment environment) {
            this.ModifierEnvironment = environment;
            this.ModifierEnvironment.OnModifierUpdated += this.UpdateAttribute;
        }

        private void PostAttributeUpdate(Attribute attribute) {
            this.Attributes[attribute.Id].Value = !attribute.IsValueApproximated
                    ? this.Approximator.Process(attribute).Value
                    : attribute.Value;
        }

        private void UpdateAttribute(AttributeKey key) {
            if (!this.Attributes.TryGetValue(key, out Node node)) {
                node = new Node();
                this.Attributes.Add(key, node);
            }

            double oldValue = node.Value;
            AttributeQuery query = new AttributeQuery(this.gameObject, this, key, node.BaseValue, false);
            this.PostAttributeUpdate(this.ModifierEnvironment.Query(ref query, node.Processors));
            this.OnAttributeUpdated?.Invoke(new AttributeChange(key, oldValue, node.Value));
        }

        public double GetCurrent(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) ? node.Value : 0;
        }

        public bool Has(double threshold, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) && node.Value >= threshold;
        }

        public void SetBase(AttributeKey key, double value) {
            try {
                this.Attributes[key].BaseValue = value;
                this.UpdateAttribute(key);
            } catch (KeyNotFoundException e) {
                Debug.LogException(e, this);
            }
        }

        public void Clear() {
            this.Attributes.Clear();
        }

        internal void Initialise(AttributeType attributeType, double value) {
            Node node = new Node();
            node.Processors.AddRange(attributeType.Processors);
            this.Attributes.Add(attributeType.Id, node);
            this.SetBase(attributeType.Id, value);
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

        public void AddModifier(Modifier modifier) {
            this.ModifierEnvironment.AddModifier(modifier);
        }
    }
}