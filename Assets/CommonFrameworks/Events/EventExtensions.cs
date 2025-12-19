using System;

namespace CommonFrameworks.Events {
    public static class EventExtensions {
        public static void Publish<S, E>(this E @event) where E : IEvent<S> where S : class {
            MailBox<S, E>.Publish(@event);
        }
        
        public static void Whisper<S, E>(this E @event, object subscriber) where E : IEvent<S> where S : class {
            MailBox<S, E>.PublishTo(subscriber, @event);
        }
        
        public static void Subscribe<S, E>(this object listener, Action<E> handler) where S : class where E : IEvent<S> {
            MailBox<S, E>.Register(listener, handler);
        }
        
        public static void Unsubscribe<S, E>(this object listener, Action<E> handler) where S : class where E : IEvent<S> {
            MailBox<S, E>.Unregister(listener, handler);
        }

        public static void Subscribe<S, E>(this object listener, Action handler) where S : class where E : IEvent<S> {
            MailBox<S, E>.Register(listener, handler);
        }
        
        public static void Unsubscribe<S, E>(this object listener, Action handler) where S : class where E : IEvent<S> {
            MailBox<S, E>.Unregister(listener, handler);
        }
        
        public static void UnsubscribeAll<S, E>(this object listener) where S : class where E : IEvent<S> {
            MailBox<S, E>.Unregister(listener);
        }
    }
}
