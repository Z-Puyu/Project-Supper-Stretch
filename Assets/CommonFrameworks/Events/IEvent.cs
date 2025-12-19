namespace CommonFrameworks.Events {
    public interface IEvent<out S> where S : class {
        public S Sender { get; }
    }
}
