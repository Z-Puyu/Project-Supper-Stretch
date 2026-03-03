using System.Collections.Generic;
using System.Linq;

namespace GameplayAbilities.Attributes.Evaluation {
    public interface IAttributeCalculationOperation {
        public IEnumerable<GameplayAttributeType> Dependencies => Enumerable.Empty<GameplayAttributeType>();
        
        public double Perform(double input, IAttributeReader context);
    }
}
