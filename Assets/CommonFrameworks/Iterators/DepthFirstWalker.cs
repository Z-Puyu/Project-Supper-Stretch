using System;
using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public sealed class DepthFirstWalker<T> : Walker<T> {
        public DepthFirstWalker(
            Action<T> onVisit = null, Action<T, T> onMoveForward = null, Action<T, T> onBacktrack = null
        ) : base(onVisit, onMoveForward, onBacktrack) { }

        protected override IEnumerable<Move> Traverse(ITraversable<T> map, T start) {
            if (start is null) {
                yield break;
            }

            yield return new Move(start);
            if (!map.HasOutNeighbours(start, out IEnumerable<T> neighbours)) {
                yield break;
            }

            foreach (T u in neighbours) {
                foreach (Move move in dfs(start, u)) {
                    yield return move;
                }
                
                yield return new Move(u, start, Move.Type.Backward);
            }
            
            yield break;

            IEnumerable<Move> dfs(T parent, T source) {
                if (source is null) {
                    yield break;
                }
                
                yield return new Move(parent, source, Move.Type.Forward);
                if (!map.HasOutNeighbours(source, out IEnumerable<T> neighbourhood)) {
                    yield break;
                }
                
                foreach (T neighbour in neighbourhood) {
                    foreach (Move move in dfs(source, neighbour)) {
                        yield return move;
                    }
                }
                
                yield return new Move(source, parent, Move.Type.Backward);
            }
        }
    }
}
