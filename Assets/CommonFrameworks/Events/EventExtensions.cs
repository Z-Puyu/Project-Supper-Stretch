using System;

namespace CommonFrameworks.Events;

public static class EventExtensions {
    public static void Publish<S, E>(this S sender, E @event) where S : class where E : IMessage {
        MailBox<S, E>.Publish(sender, @event);
    }
        
    public static void Whisper<S, E>(this S sender, E @event, object subscriber) where E : IMessage where S : class {
        MailBox<S, E>.PublishTo(subscriber, sender, @event);
    }
        
    public static void Subscribe<S, E>(this object listener, Action<Event<S, E>> handler) where S : class where E : IMessage {
        MailBox<S, E>.Register(listener, handler);
    }
        
    public static void Unsubscribe<S, E>(this object listener, Action<Event<S, E>> handler) where S : class where E : IMessage {
        MailBox<S, E>.Unregister(listener, handler);
    }

    public static void Subscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
        MailBox<S, E>.Register(listener, handler);
    }
        
    public static void Unsubscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
        MailBox<S, E>.Unregister(listener, handler);
    }
        
    public static void UnsubscribeAll<S, E>(this object listener) where S : class where E : IMessage {
        MailBox<S, E>.Unregister(listener);
    }
}