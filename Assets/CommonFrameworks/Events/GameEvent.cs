using UnityEngine;

namespace CommonFrameworks.Events {
    public abstract class GameEvent<S> : ScriptableObject where S : class {
        public abstract void Publish(S sender);
    }
}