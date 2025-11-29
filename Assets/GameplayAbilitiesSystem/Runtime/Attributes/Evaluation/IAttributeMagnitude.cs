using System.Collections.Generic;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    public interface IAttributeMagnitude {
        public double Evaluate(IAttributeReader attributes, IReadOnlyDictionary<string, double> userData);
    }
}
