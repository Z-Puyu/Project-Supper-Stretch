using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public sealed class LevelOrderIterator<T> : Iterator<T> {
        protected override IEnumerable<Move> Traverse(ITraversable<T> map, T start) {
            if (start is null) {
                yield break;
            }
            
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(start);
            while (queue.TryDequeue(out T curr)) {
                yield return new Move(curr);
                if (!map.HasOutNeighbours(curr, out IEnumerable<T> neighbours)) {
                    continue;
                }

                foreach (T neighbour in neighbours) {
                    queue.Enqueue(neighbour);
                }
            }
        }
    }
}
