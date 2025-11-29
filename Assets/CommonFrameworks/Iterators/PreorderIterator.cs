using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public sealed class PreorderIterator<T> : Iterator<T> {
        protected override IEnumerable<Move> Traverse(ITraversable<T> map, T start) {
            if (start is null) {
                yield break;
            }

            yield return new Move(start);
            if (!map.HasOutNeighbours(start, out IEnumerable<T> neighbours)) {
                yield break;
            }

            foreach (T u in neighbours) {
                foreach (Move move in this.Traverse(map, u)) {
                    yield return move;
                }
            }
        }
    }
}
