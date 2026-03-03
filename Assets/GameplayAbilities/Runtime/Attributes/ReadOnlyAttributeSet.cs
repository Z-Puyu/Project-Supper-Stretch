using System.Collections;
using System.Collections.Generic;

namespace GameplayAbilities.Attributes {
    public sealed class ReadOnlyAttributeSet : IAttributeReader {
        private static readonly IDictionary<IAttributeReader, ReadOnlyAttributeSet> Cache =
                new Dictionary<IAttributeReader, ReadOnlyAttributeSet>();

        private IDictionary<GameplayAttributeType, Entry> Attributes { get; } =
            new Dictionary<GameplayAttributeType, Entry>();

        private ReadOnlyAttributeSet() { }
        
        public static ReadOnlyAttributeSet From(IAttributeReader attributes) {
            if (!ReadOnlyAttributeSet.Cache.TryGetValue(attributes, out ReadOnlyAttributeSet set)) {
                set = new ReadOnlyAttributeSet();
                ReadOnlyAttributeSet.Cache.Add(attributes, set);
            }
            
            set.Attributes.Clear();
            foreach (GameplayAttribute attribute in attributes) {
                set.Attributes.Add(
                    attribute.Type,
                    new Entry(
                        attribute.Value, attributes.QueryMax(attribute.Type), attributes.QueryMin(attribute.Type)
                    )
                );
            }
                
            return set;
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