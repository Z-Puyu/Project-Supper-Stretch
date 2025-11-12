using System.Collections.Generic;
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
    public class AttributeSet : MonoBehaviour, IAttributeReader {
        private IEnumerable<AttributeKey> keys;
        private IEnumerable<double> values;

        private sealed class Node {
            internal double BaseValue { get; set; }
            internal double Value { get; set; }
            internal List<IProcessor<Attribute>> Processors { get; } = new List<IProcessor<Attribute>>();
        }

        private TrieDictionary<AttributeKey, char, Node> Attributes { get; } =
            new TrieDictionary<AttributeKey, char, Node>('.');

        [field: SerializeField]
        [field: InfoBox(
            "Attribute values are approximated by this after change." +
            "You can overwrite this in specific attribute types."
        )]
        private AttributeApproximator Approximator { get; set; }

        [field: SerializeField, ReadOnly] private ModifierEnvironment ModifierEnvironment { get; set; }

        public event UnityAction<AttributeChange> OnAttributeUpdated;
        
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

        private void Awake() {
            this.RegisterModifierEnvironment(this.GetInParentOrAddComponent<ModifierEnvironment>());
        }

        public double GetCurrent(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) ? node.Value : 0;
        }

        public bool Has(double threshold, AttributeKey key) {
            return this.GetCurrent(key) >= threshold;
        }

        private void OnValidate() {
            this.RegisterModifierEnvironment(this.GetInParentOrAddComponent<ModifierEnvironment>());
        }
    }
}
