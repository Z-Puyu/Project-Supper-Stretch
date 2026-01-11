using System.Collections.Generic;

namespace CommonFrameworks.Maths {
    public interface IEvaluable<in T> {
        public double Evaluate(T context);
        public ICollection<object> DependentParameters { get; }
    }
}
