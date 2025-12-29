using System;
using System.Collections.Generic;

namespace CommonFrameworks.Iterators {
    public abstract class Iterator<T> : IIterator<T> {
        protected readonly struct Move {
            public enum Type { Forward, Backward, Visit }
            
            public Type MoveType { get; }
            public T From { get; }
            public T To { get; }
            
            public Move(T from, T to, Type type) {
                this.From = from;
                this.To = to;
                this.MoveType = type;
            }
            
            public Move(T to) : this(default!, to, Type.Visit) { }
        }
        
        private Action<T>? OnVisit { get; }
        
        public Iterator(Action<T>? onVisit = null) {
            this.OnVisit = onVisit;
        }

        protected virtual void Step(in Move move, ref int steps) {
            this.OnVisit?.Invoke(move.To);
            steps += 1;
        }

        private void Iterate(IEnumerator<Move> enumerator, Predicate<T>? until = null, int count = -1) {
            int steps = 0;
            while (enumerator.MoveNext()) {
                Move move = enumerator.Current;
                this.Step(move, ref steps);
                bool shouldStop = (until?.Invoke(move.To) ?? false) || (count > 0 && steps >= count);
                if (shouldStop) {
                    break;
                }
            }
        }

        public void Iterate(ITraversable<T> map, Predicate<T>? until = null, int count = -1) {
            using IEnumerator<Move> enumerator = this.Traverse(map, map.Start).GetEnumerator();
            this.Iterate(enumerator, until, count);
        }

        public void Iterate(ITraversable<T> map, T source, Predicate<T>? until = null, int count = -1) {
            using IEnumerator<Move> enumerator = this.Traverse(map, source).GetEnumerator();
            this.Iterate(enumerator, until, count);
        }

        protected abstract IEnumerable<Move> Traverse(ITraversable<T> map, T start);
    }
}