using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public sealed class PostorderIterator<T> : Iterator<T> {
        protected override IEnumerable<Move> Traverse(ITraversable<T> map, T start) {
            if (start is null) {
                yield break;
            }
            
            if (!map.HasOutNeighbours(start, out IEnumerable<T> neighbours)) {
                yield break;
            }
            
            foreach (T neighbour in neighbours) {
                foreach (Move move in this.Traverse(map, neighbour)) {
                    yield return move;
                }
            }
            
            yield return new Move(start);
        }
    }
}
