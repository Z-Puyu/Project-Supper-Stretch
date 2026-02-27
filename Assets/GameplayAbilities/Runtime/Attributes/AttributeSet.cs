using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Attributes {
    [RequireComponent(typeof(ModifierEnvironment)), AddComponentMenu("")]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader, IModifiable {
        private IDictionary<GameplayAttributeType, Value> Attributes { get; } =
            new Dictionary<GameplayAttributeType, Value>();

        private IDictionary<GameplayAttributeType, Action<GameplayAttributeType, AttributeChange>> Observers { get; } =
            new Dictionary<GameplayAttributeType, Action<GameplayAttributeType, AttributeChange>>();

        [NotNull] [field: SerializeField] private GameObject? Owner { get; set; }

        [field: SerializeField]
        private GameplayAttributeType.ApproximationPolicy DefaultRoundingPolicy { get; set; } =
            GameplayAttributeType.ApproximationPolicy.RoundToNearest;

        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }

        public event UnityAction<GameplayAttributeType, AttributeChange> OnAnyAttributeUpdated = delegate { };

        private void Awake() {
            this.ModifierEnvironment.OnModifierUpdated += this.Evaluate;
        }

        private void Evaluate(GameplayAttributeType key) {
            if (!this.Attributes.TryGetValue(key, out Value oldValue)) {
                return;
            }

            AttributeQuery query = new AttributeQuery(this, key, oldValue.Base);
            this.ModifierEnvironment.Query(ref query);
            double current = query.Evaluate();
            this.Attributes[key] = oldValue with { Current = current, Effective = key.Approximate(current) };
            AttributeValue old = new AttributeValue(oldValue.Base, oldValue.Effective, oldValue.Current);
            AttributeChange change = new AttributeChange(old, this.Query(key));
            this.OnAnyAttributeUpdated.Invoke(key, change);
            if (this.Observers.TryGetValue(key, out Action<GameplayAttributeType, AttributeChange> observer)) {
                observer.Invoke(key, change);
            }
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

        private void SetBase(GameplayAttributeType key, double value) {
            if (!this.Attributes.TryGetValue(key, out Value v)) {
                this.Attributes.Add(key, new Value(value, 0, 0));
            } else {
                this.Attributes[key] = v with { Base = value };
            }
            
            this.Evaluate(key);
        }

        public void Clear() {
            this.Attributes.Clear();
        }

        internal void Initialise(GameplayAttributeType type, double value) {
            if (this.Attributes.ContainsKey(type)) {
#if DEBUG
                Debug.LogError($"Attribute {type.Id} is already initialised", this.Owner);
#endif
            } else {
                this.Attributes.Add(type, new Value(value, 0, 0));
                this.Evaluate(type);
#if DEBUG
                Debug.Log($"Attribute {type.Id} initialised to {this.Query(type).Value}", this.Owner);
#endif
            }
        }
        
        public void Observe(GameplayAttributeType attribute, Action<GameplayAttributeType, AttributeChange> callback) {
            if (!this.Observers.TryAdd(attribute, callback)) {
                this.Observers[attribute] += callback;
            }
        }

        public void RemoveObserver(GameplayAttributeType attribute, Action<GameplayAttributeType, AttributeChange> callback) {
            if (!this.Observers.TryGetValue(attribute, out Action<GameplayAttributeType, AttributeChange>? observer)) {
                return;
            }

            Action<GameplayAttributeType, AttributeChange>? action = observer - callback;
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

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"Attributes on {this.gameObject.name}:\n", this.Attributes.Count + 1);
            foreach (KeyValuePair<GameplayAttributeType, Value> entry in this.Attributes) {
                sb.AppendLine($"|{entry.Key}: {entry.Value.Effective}");
            }

            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        void IModifiable.AddModifier(GameplayAttributeType target, Modifier modifier) {
            if (modifier.Type == ModifierType.SetBase) {
                this.SetBase(target, modifier.Value);
            } else {
                this.ModifierEnvironment.AddModifier(target, modifier);
            }
        }

        private readonly record struct Value(double Base, double Current, double Effective);
    }
}
