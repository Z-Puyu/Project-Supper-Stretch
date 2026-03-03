using System.Collections.Generic;
using GameplayAbilities.Common;

namespace GameplayAbilities.Attributes.Evaluation {
    public interface IAttributeMagnitude {
        public double Evaluate(IAttributeReader? attributes, IUserData? userData = null);
    }
}