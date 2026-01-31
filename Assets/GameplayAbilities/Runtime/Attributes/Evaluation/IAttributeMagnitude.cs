using System.Collections.Generic;

namespace GameplayAbilities.Attributes.Evaluation {
    public interface IAttributeMagnitude {
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null);
    }
}