using System;
using System.Collections.Generic;

namespace CommonFrameworks.Maths {
    [Serializable]
    public abstract class Calculation<V, T> {
        public ICollection<object> AuxiliaryParameters { get; } = new HashSet<object>();
        
        public abstract V Apply(V input, T context);
    }
}
