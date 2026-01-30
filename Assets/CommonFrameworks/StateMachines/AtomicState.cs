namespace CommonFrameworks.StateMachines {
    public abstract class AtomicState<S> : State<S> where S : AtomicState<S>, new() { }
}
