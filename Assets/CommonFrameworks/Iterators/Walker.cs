using System;

namespace CommonFrameworks.Iterators {
    public abstract class Walker<T> : Iterator<T> {
        private Action<T, T>? OnMoveForward { get; }
        private Action<T, T>? OnBacktrack { get; }

        protected Walker(
            Action<T>? onVisit = null, Action<T, T>? onMoveForward = null, Action<T, T>? onBacktrack = null
        ) : base(onVisit) {
            this.OnMoveForward = onMoveForward;
            this.OnBacktrack = onBacktrack;
        }

        protected sealed override void Step(in Move move, ref int steps) {
            switch (move.MoveType) {
                case Move.Type.Forward:
                    this.OnMoveForward?.Invoke(move.From, move.To);
                    break;
                case Move.Type.Backward:
                    this.OnBacktrack?.Invoke(move.From, move.To);
                    break;
            }
            
            base.Step(in move, ref steps);
        }
    }
}