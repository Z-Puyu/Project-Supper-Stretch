using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Collections;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public sealed class ReadOnlyAttributeSet : IAttributeReader {
        private TrieDictionary<AttributeKey, char, (double value, double max, double min)> Attributes { get; } =
            new TrieDictionary<AttributeKey, char, (double value, double max, double min)>();

        public ReadOnlyAttributeSet(IAttributeReader attributes) {
            foreach (Attribute attribute in attributes) {
                this.Attributes.Add(
                    attribute.Id,
                    (attribute.Value, attributes.QueryMax(attribute.Id), attributes.QueryMin(attribute.Id))
                );
            }
        }
        
        public double Query(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out (double value, double max, double min) value) ? value.value : 0;
        }

        public double QueryMax(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out (double value, double max, double min) value)
                    ? value.max
                    : int.MaxValue;
        }
        
        public double QueryMin(AttributeKey key) {
            return this.Attributes.TryGetValue(key, out (double value, double max, double min) value)
                    ? value.min
                    : int.MinValue;
        }

        public bool HasAtLeast(double threshold, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out (double value, double max, double min) value) &&
                   value.value >= threshold;
        }

        public bool HasAtMost(double cap, AttributeKey key) {
            return this.Attributes.TryGetValue(key, out (double value, double max, double min) value) &&
                   value.value <= cap;
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Attributes.Select(entry => new Attribute(this, entry.Key, entry.Value.value, true))
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}