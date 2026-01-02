using System;

namespace CommonFrameworks.Events {
    public static class EventExtensions {
        public static void Send<S, E>(this S sender, E @event) where S : class where E : IMessage {
            MailBox<S, E>.Send(sender, @event);
        }
        
        public static void SendTo<S, E>(this S sender, object subscriber, E @event) where E : IMessage where S : class {
            MailBox<S, E>.SendTo(subscriber, sender, @event);
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
}