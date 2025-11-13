using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFrameworks.CommonUtilities.Processors;
using CommonFrameworks.Extensions;
using CommonFrameworks.Trees;
using GameplayAbilitiesSystem.Runtime.Attributes.Processors;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [DisallowMultipleComponent]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader, IModifiable {
        private sealed class Node {
            internal double BaseValue { get; set; }
            internal double Value { get; set; }
            internal List<IProcessor<Attribute>> Processors { get; } = new List<IProcessor<Attribute>>();
        }

        private TrieDictionary<AttributeKey, char, Node> Attributes { get; } =
            new TrieDictionary<AttributeKey, char, Node>('.');

        [field: SerializeField] private AttributeTable DefaultBaseAttributes { get; set; }
        
        [field: SerializeField]
        [field: InfoBox(
            "Attribute values are approximated by this after change." +
            "You can overwrite this in specific attribute types."
        )]
        private AttributeApproximator Approximator { get; set; }

        [field: SerializeField, ReadOnly] private ModifierEnvironment ModifierEnvironment { get; set; }

        public event UnityAction<AttributeChange> OnAttributeUpdated;

        private void Awake() {
            this.RegisterModifierEnvironment(this.GetInParentOrAddComponent<ModifierEnvironment>());
        }

        private void OnEnable() {
            if (this.Attributes.Count == 0 && this.DefaultBaseAttributes) {
                this.DefaultBaseAttributes.Initialise(this);
            }
        }

        private void RegisterModifierEnvironment(ModifierEnvironment environment) {
            if (this.ModifierEnvironment) {
                this.ModifierEnvironment.OnModifierUpdated -= this.UpdateAttribute;
            }

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
            Attribute @base = new Attribute(this, key, node.BaseValue, false);
            this.PostAttributeUpdate(this.ModifierEnvironment.Query(@base, node.Processors));
            this.OnAttributeUpdated?.Invoke(new AttributeChange(key, oldValue, node.Value));
        }

        public void MoveIntoEnvironment(ModifierEnvironment environment) {
            this.transform.SetParent(environment.transform);
            this.ModifierEnvironment = environment;
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

        private void OnValidate() {
            this.RegisterModifierEnvironment(this.GetInParentOrAddComponent<ModifierEnvironment>());
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
