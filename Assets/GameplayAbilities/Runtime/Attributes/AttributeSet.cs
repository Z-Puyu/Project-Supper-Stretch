using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Attributes {
    [RequireComponent(typeof(ModifierEnvironment))]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader {
        private IDictionary<GameplayAttributeType, Value> Attributes { get; } =
            new ConcurrentDictionary<GameplayAttributeType, Value>();

        private IDictionary<GameplayAttributeType, Action<AttributeChange>> Observers { get; } =
            new Dictionary<GameplayAttributeType, Action<AttributeChange>>();

        [field: SerializeField]
        private GameplayAttributeType.RoundingMethod DefaultRoundingPolicy { get; set; } =
            GameplayAttributeType.RoundingMethod.RoundToNearest;

        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }

        public event UnityAction<GameplayAttributeType, AttributeChange> OnAnyAttributeUpdated = delegate { };

        private void Awake() {
            this.ModifierEnvironment = this.GetComponent<ModifierEnvironment>();
            this.ModifierEnvironment.OnModifierUpdated += this.Evaluate;
        }
        
        private void Initialise(IDictionary<GameplayAttributeType, double> values) {
            this.Attributes.Clear();
            foreach ((GameplayAttributeType type, double value) in values) {
                this.Attributes.Add(type, new Value(value, 0, 0));
            }
            
            foreach (GameplayAttributeType type in values.Keys) {
                foreach (GameplayAttributeType dependency in type.GetDependencies()) {
                    this.Observe(dependency, _ => this.Evaluate(type));
                }
            }
            
            foreach (GameplayAttributeType type in values.Keys) {
                this.Evaluate(type);
#if DEBUG
                Debug.Log($"Attribute {type.Id} initialised to {this.Query(type).Value}");
#endif
            }
        }

        private void Evaluate(GameplayAttributeType key) {
            if (!this.Attributes.TryGetValue(key, out Value value)) {
                this.Attributes.Add(key, value = new Value(0, 0, 0));
            }
            
            double @base = value.Base;
            double current = this.QueryCurrentValue(key, ref @base);
            this.Attributes[key] = new Value(@base, current, key.Approximate(current));
            AttributeValue old = new AttributeValue(value.Base, value.Effective, value.Current);
            AttributeChange change = new AttributeChange(old, this.Query(key));
            this.TriggerCallbacks(key, change);
        }

        private void TriggerCallbacks(GameplayAttributeType key, AttributeChange change) {
            this.OnAnyAttributeUpdated.Invoke(key, change);
            if (this.Observers.TryGetValue(key, out Action<AttributeChange> observer)) {
                observer.Invoke(change);
            }
        }

        private double QueryCurrentValue(GameplayAttributeType key, ref double @base) {
            AttributeQuery query = new AttributeQuery(this, key, @base);
            this.ModifierEnvironment.Query(ref query);
            return query.Evaluate(out @base);
        }

        public AttributeValue Query(GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Value value)
                    ? new AttributeValue(value.Base, value.Effective, value.Current)
                    : AttributeValue.Zero;
        }

        public double QueryMax(GameplayAttributeType key) {
            return this.Attributes.ContainsKey(key) ? key.Approximate(key.Clamp(int.MaxValue, this)) : int.MaxValue;
        }

        public double QueryMin(GameplayAttributeType key) {
            return this.Attributes.ContainsKey(key) ? key.Approximate(key.Clamp(int.MinValue, this)) : int.MinValue;
        }

        public bool HasAtLeast(double threshold, GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Value value) && value.Effective >= threshold;
        }

        public bool HasAtMost(double cap, GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Value value) && value.Effective <= cap;
        }

        public void Clear() {
            this.Attributes.Clear();
        }
        
        public void Observe(GameplayAttributeType attribute, Action<AttributeChange> callback) {
            if (!this.Observers.TryAdd(attribute, callback)) {
                this.Observers[attribute] += callback;
            }
        }

        public void RemoveObserver(GameplayAttributeType attribute, Action<AttributeChange> callback) {
            if (!this.Observers.TryGetValue(attribute, out Action<AttributeChange>? observer)) {
                return;
            }

            Action<AttributeChange>? action = observer - callback;
            if (action is null) {
                this.Observers.Remove(attribute);
                return;
            }

            this.Observers[attribute] = action;
        }

        public IEnumerator<GameplayAttribute> GetEnumerator() {
            foreach ((GameplayAttributeType key, Value value) in this.Attributes) {
                yield return new GameplayAttribute(key, new AttributeValue(value.Base, value.Effective, value.Current));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Attributes on {this.gameObject.name}:\n", this.Attributes.Count + 1);
            foreach (KeyValuePair<GameplayAttributeType, Value> entry in this.Attributes) {
                sb.AppendLine($"|{entry.Key}: {entry.Value.Effective}");
            }

            return sb.ToString();
        }

        private readonly record struct Value(double Base, double Current, double Effective);
    }
}
