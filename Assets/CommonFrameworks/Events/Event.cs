namespace CommonFrameworks.Events;

public readonly record struct Event<S, E>(S Sender, E Message) where S : class where E : IMessage;
