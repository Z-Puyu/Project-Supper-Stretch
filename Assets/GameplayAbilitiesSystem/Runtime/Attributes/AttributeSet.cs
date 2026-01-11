using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using CommonFrameworks.Maths;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
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

        public event UnityAction<AttributeKey, AttributeChange> OnAnyAttributeUpdated = delegate { };

        protected override void Awake() {
            base.Awake();
            this.ModifierEnvironment = this.GetOrAddComponent<ModifierEnvironment>();
            this.ModifierEnvironment.OnModifierUpdated += this.Evaluate;
            if (this.Attributes.Count == 0 && this.DefaultBaseAttributes) {
                this.DefaultBaseAttributes.Initialise(this);
            }
        }

        private void Evaluate(AttributeKey key) {
            if (!this.Attributes.TryGetValue(key, out Node node)) {
                node = this.AddNode(key, 0);
            }

            double oldValue = node.Value;
            Attribute updated = this.Query(key, node.BaseValue, node.MaxValue, node.MinValue, node.Approximator);
            if (!updated.HasBeenApproximated) {
                this.Approximator.Approximate(ref updated);
            }

            this.Attributes[updated.Id].Value = updated.Value;
            this.OnAnyAttributeUpdated?.Invoke(key, new AttributeChange(oldValue, node.Value));
        }

        private Attribute Query(
            AttributeKey key, double @base, IEvaluable<IAttributeReader>? max, IEvaluable<IAttributeReader>? min,
            AttributeApproximator? approximator
        ) {
            AttributeQuery query = new AttributeQuery(this.Owner, this, key, @base);
            return this.ModifierEnvironment.Query(ref query, max, min, approximator);
        }

        public double Query(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) ? node.Value : 0;
        }

        public double QueryMax(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node)
                    ? this.Query(key, int.MaxValue, node.MaxValue, node.MinValue, node.Approximator).Value
                    : int.MaxValue;
        }

        public double QueryMin(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node)
                    ? this.Query(key, int.MinValue, node.MaxValue, node.MinValue, node.Approximator).Value
                    : int.MinValue;
        }

        public bool HasAtLeast(double threshold, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) && node.Value >= threshold;
        }

        public bool HasAtMost(double cap, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out Node node) && node.Value <= cap;
        }

        private void SetBase(AttributeKey key, double value) {
            if (!this.Attributes.TryGetValue(key, out Node node)) {
                node = this.AddNode(key, value);
            }

            node.BaseValue = value;
            this.Evaluate(key);
        }

        public void Clear() {
            this.Attributes.Clear();
        }

        internal void Initialise(AttributeType type, double value) {
            if (this.Attributes.ContainsKey(type.Id)) {
#if DEBUG
                Debug.LogError($"Attribute {type.Id} is already initialised", this.Owner);
#endif
            } else {
                this.AddNode(type.Id, value, type);
                this.Evaluate(type.Id);
#if DEBUG
                Debug.Log($"Attribute {type.Id} initialised to {this.Query(type.Id)}", this.Owner);
#endif
            }
        }

        private Node AddNode(AttributeKey key, double value, AttributeType? type = null) {
            Node node = new Node(this, key, value, type);
            node.OnValueChanged += this.OnAnyAttributeUpdated.Invoke;
            this.Attributes.Add(key, node);
            return node;
        }

        public void Observe(AttributeKey attribute, Action<AttributeKey, AttributeChange> callback) {
            if (this.Attributes.TryGetValue(attribute, out Node node)) {
                node.OnValueChanged += callback;
            }
        }

        public void RemoveObserver(AttributeKey attribute, Action<AttributeKey, AttributeChange> callback) {
            if (this.Attributes.TryGetValue(attribute, out Node node)) {
                node.OnValueChanged -= callback;
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
            private double value;

            private AttributeSet Owner { get; }
            private AttributeKey Key { get; }
            internal double BaseValue { get; set; }
            internal IEvaluable<IAttributeReader>? MinValue { get; }
            internal IEvaluable<IAttributeReader>? MaxValue { get; }
            internal AttributeApproximator? Approximator { get; }
            private AttributeCalculator? Derivation { get; }

            internal double Value {
                get => this.value;
                set {
                    double old = this.value;
                    this.value = value;
                    this.OnValueChanged.Invoke(this.Key, new AttributeChange(old, this.value));
                }
            }
            
            internal event Action<AttributeKey, AttributeChange> OnValueChanged = delegate { };

            private void Reevaluate() {
                if (this.Derivation is not null) {
                    this.Owner.SetBase(this.Key, this.Derivation.Evaluate(this.Owner));
                } else {
                    this.Owner.Evaluate(this.Key);
                }
            }
            
            private void OnDependencyChange(AttributeKey k, AttributeChange change) {
                this.Reevaluate();
            }
            
            internal Node(AttributeSet owner, AttributeKey key, double initialValue, AttributeType? definition = null) {
                this.Owner = owner;
                this.Key = key;
                this.BaseValue = initialValue;
                this.MinValue = definition?.MinValue;
                this.MaxValue = definition?.MaxValue;
                this.Approximator = definition?.ApproximatorOverride;
                this.Derivation = definition?.Derivation;
                IEnumerable<object> dependencies = Enumerable.Empty<object>();
                if (this.Derivation is not null) {
                    dependencies = dependencies.Concat(this.Derivation.DependentParameters);
                }

                if (this.MinValue is not null) {
                    dependencies = dependencies.Concat(this.MinValue.DependentParameters);
                }
                
                if (this.MaxValue is not null) {
                    dependencies = dependencies.Concat(this.MaxValue.DependentParameters);
                }
                
                foreach (object param in dependencies.Distinct()) {
                    switch (param) {
                        case AttributeKey k:
                            owner.Observe(k, this.OnDependencyChange);
                            break;
                        case string id:
                            owner.Observe(id, this.OnDependencyChange);
                            break;
                        case AttributeType attribute:
                            owner.Observe(attribute.Id, this.OnDependencyChange);
                            break;
                    }
                }

                this.Reevaluate();
            }
        }
    }
}
