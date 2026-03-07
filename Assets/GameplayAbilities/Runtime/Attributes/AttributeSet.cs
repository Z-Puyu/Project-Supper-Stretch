using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using GameplayAbilities.Attributes.EffectTriggers;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Attributes {
    [RequireComponent(typeof(ModifierEnvironment))]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader {
        private IDictionary<GameplayAttributeType, Value> Attributes { get; } =
            new ConcurrentDictionary<GameplayAttributeType, Value>();

        private IDictionary<GameplayAttributeType, Action<AttributeChange>> Observers { get; } =
            new ConcurrentDictionary<GameplayAttributeType, Action<AttributeChange>>();

        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }
        [field: SerializeField] private AttributeTable? DefaultStartingAttributes { get; set; }

        [field: SerializeField]
        private List<AttributeSetEffectTrigger> EffectTriggers { get; set; } =
            new List<AttributeSetEffectTrigger>();
        
        [field: SerializeField]
        private GameplayAttributeType.RoundingMethod DefaultRoundingPolicy { get; set; } =
            GameplayAttributeType.RoundingMethod.RoundToNearest;

        public event UnityAction<GameplayAttributeType, AttributeChange> OnAnyAttributeUpdated = delegate { };

        private void Awake() {
            this.ModifierEnvironment = this.GetComponent<ModifierEnvironment>();
        }

        private void OnEnable() {
            this.ModifierEnvironment.OnModifierUpdated += this.Evaluate;
        }
        
        private void OnDisable() {
            this.ModifierEnvironment.OnModifierUpdated -= this.Evaluate;
        }

        private void Start() {
            if (this.Attributes.Count == 0 && this.DefaultStartingAttributes) {
                this.Initialise(this.DefaultStartingAttributes);
            }
        }

        /// <summary>
        /// Initialises the attribute set with the given starting values.
        /// </summary>
        /// <param name="attributes">The starting values for the attributes.</param>
        public void Initialise(IEnumerable<KeyValuePair<GameplayAttributeType, double>> attributes) {
            this.Attributes.Clear();
            IReadOnlyDictionary<GameplayAttributeType, double> map = attributes.ToDictionary(x => x.Key, x => x.Value);
            foreach ((GameplayAttributeType type, double value) in map) {
                this.Attributes.Add(type, new Value(value, 0, 0));
            }
            
            foreach (GameplayAttributeType type in map.Keys) {
                foreach (GameplayAttributeType dependency in type.GetDependencies()) {
                    this.Observe(dependency, _ => this.Evaluate(type));
                }
            }
            
            foreach (GameplayAttributeType type in map.Keys) {
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
            AttributeChange change = new AttributeChange(key, old, this.Query(key));
            this.TriggerCallbacks(change);
            this.TriggerInternalEffects(change);
        }

        private void TriggerInternalEffects(AttributeChange change) {
            foreach (AttributeSetEffectTrigger trigger in this.EffectTriggers) {
                trigger.TryTrigger(change);
            }
        }

        private void TriggerCallbacks(AttributeChange change) {
            this.OnAnyAttributeUpdated.Invoke(change.AttributeType, change);
            if (this.Observers.TryGetValue(change.AttributeType, out Action<AttributeChange> observer)) {
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
        
        /// <summary>
        /// Registers a callback to be invoked when the given attribute changes.
        /// </summary>
        /// <param name="attribute">The attribute to observe.</param>
        /// <param name="callback">The callback to invoke when the attribute changes.</param>
        public void Observe(GameplayAttributeType attribute, Action<AttributeChange> callback) {
            if (!this.Observers.TryAdd(attribute, callback)) {
                this.Observers[attribute] += callback;
            }
        }

        /// <summary>
        /// Removes a previously registered callback from the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to remove the observer from.</param>
        /// <param name="callback">The callback to remove.</param>
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
