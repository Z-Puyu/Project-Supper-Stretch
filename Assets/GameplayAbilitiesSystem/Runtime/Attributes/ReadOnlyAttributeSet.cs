using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Trees;

namespace GameplayAbilitiesSystem.Runtime.Attributes;

public sealed class ReadOnlyAttributeSet : IAttributeReader {
    private TrieDictionary<AttributeKey, char, double> Attributes { get; } =
        new TrieDictionary<AttributeKey, char, double>();

    public ReadOnlyAttributeSet(IAttributeReader attributes) {
        foreach (Attribute attribute in attributes) {
            this.Attributes.Add(attribute.Id, attribute.Value);
        }
    }
        
    public double GetCurrent(AttributeKey key) {
        return this.Attributes.TryGetValue(key, out double value) ? value : 0;
    }
        
    public bool Has(double threshold, AttributeKey key) {
        return this.Attributes.TryGetValue(key, out double value) && value >= threshold;
    }

    public IEnumerator<Attribute> GetEnumerator() {
        return this.Attributes.Select(entry => new Attribute(this, entry.Key, entry.Value, true)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}