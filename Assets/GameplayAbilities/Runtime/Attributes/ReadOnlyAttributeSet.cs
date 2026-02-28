using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameplayAbilities.Attributes {
    public sealed class ReadOnlyAttributeSet : IAttributeReader {
        private IReadOnlyDictionary<GameplayAttributeType, Entry> Attributes { get; }

        public ReadOnlyAttributeSet(IAttributeReader attributes) {
            this.Attributes = new ReadOnlyDictionary<GameplayAttributeType, Entry>(
                attributes.ToDictionary(
                    attribute => attribute.Type,
                    attribute => new Entry(
                        attribute.Value, attributes.QueryMax(attribute.Type), attributes.QueryMin(attribute.Type)
                    )
                )
            );
        }
        
        public AttributeValue Query(GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Entry entry) ? entry.Value : AttributeValue.Zero;
        }

        public double QueryMax(GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Entry entry) ? entry.Max : int.MaxValue;
        }
        
        public double QueryMin(GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Entry entry) ? entry.Min : int.MinValue;
        }

        public bool HasAtLeast(double threshold, GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Entry entry) && entry.Value.Value >= threshold;
        }

        public bool HasAtMost(double cap, GameplayAttributeType key) {
            return this.Attributes.TryGetValue(key, out Entry entry) && entry.Value.Value <= cap;
        }

        public IEnumerator<GameplayAttribute> GetEnumerator() {
            foreach ((GameplayAttributeType key, Entry entry) in this.Attributes) {
                yield return new GameplayAttribute(key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private readonly record struct Entry(AttributeValue Value, double Max, double Min);
    }
}