namespace CommonFrameworks.Events;

public readonly record struct Event<S, E>(S Sender, E Data) : IEvent;
