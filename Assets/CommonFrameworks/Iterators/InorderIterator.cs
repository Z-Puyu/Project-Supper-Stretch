using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Iterators;

public sealed class InorderIterator<T> : Iterator<T> {
    protected override IEnumerable<Move> Traverse(ITraversable<T> map, T start) {
        if (start is null) {
            yield break;
        }
            
        if (!map.HasOutNeighbours(start, out IEnumerable<T> neighbours)) {
            yield break;
        }
            
        foreach (T neighbour in neighbours) {
            Move[] moves = this.Traverse(map, neighbour).ToArray();
            for (int i = 0; i < moves.Length / 2; i += 1) {
                yield return moves[i];
            }
                
            yield return new Move(start);
                
            for (int i = moves.Length / 2; i < moves.Length; i += 1) {
                yield return moves[i];
            }
        }
    }
}