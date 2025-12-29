using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public interface ITraversable<T> {
        public T Start { get; }
        public bool HasOutNeighbours(T vertex, out IEnumerable<T> children);
    }
}