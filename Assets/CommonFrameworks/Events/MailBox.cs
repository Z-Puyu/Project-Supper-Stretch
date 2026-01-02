using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Events {
    public static class MailBox<S, E> where E : IMessage where S : class {
        private static Dictionary<object, Action<Event<S, E>>> Handlers { get; } = new Dictionary<object, Action<Event<S, E>>>();
        
        // ReSharper disable once StaticMemberInGenericType
        private static Dictionary<object, Action> Listeners { get; } = new Dictionary<object, Action>();
        
        private static event Action<Event<S, E>> OnEvent = delegate { };
        private static event Action OnNotified = delegate { };

        internal static void Send(S sender, E @event) {
            MailBox<S, E>.OnEvent.Invoke(new Event<S, E>(sender, @event));
            MailBox<S, E>.OnNotified.Invoke();
        }

        internal static void SendTo(object subscriber, S sender, E @event) {
            if (MailBox<S, E>.Handlers.TryGetValue(subscriber, out Action<Event<S, E>> handler)) {
                handler.Invoke(new Event<S, E>(sender, @event));
            }
        }

        internal static void Register(object subscriber, Action<Event<S, E>> handler) {
            MailBox<S, E>.OnEvent += handler;
            if (!MailBox<S, E>.Handlers.TryAdd(subscriber, handler)) {
                MailBox<S, E>.Handlers[subscriber] += handler;
            }
        }

        internal static void Register(object subscriber, Action handler) {
            MailBox<S, E>.OnNotified += handler;
            if (!MailBox<S, E>.Listeners.TryAdd(subscriber, handler)) {
                MailBox<S, E>.Listeners[subscriber] += handler;
            }
        }
        
        internal static void Unregister(object subscriber, Action<Event<S, E>> handler) {
            if (!MailBox<S, E>.Handlers.TryGetValue(subscriber, out Action<Event<S, E>> existing)) {
                return;
            }
            
            MailBox<S, E>.OnEvent -= handler;
            existing -= handler;
            if (existing is null) {
                MailBox<S, E>.Handlers.Remove(subscriber);
            } else {
                MailBox<S, E>.Handlers[subscriber] = existing;
            }
        }
        
        internal static void Unregister(object subscriber, Action handler) {
            if (!MailBox<S, E>.Listeners.TryGetValue(subscriber, out Action existing)) {
                return;
            }
            
            MailBox<S, E>.OnNotified -= handler;
            existing -= handler;
            if (existing is null) {
                MailBox<S, E>.Listeners.Remove(subscriber);
            } else {
                MailBox<S, E>.Listeners[subscriber] = existing;
            }
        }

        private static void UnregisterAll(Action<Event<S, E>> handler) {
            if (handler is null) {
                return;
            }
            
            HashSet<Delegate> toRemove = handler.GetInvocationList().ToHashSet();
            Delegate[] kept = MailBox<S, E>.OnEvent.GetInvocationList()
                                           .Where(@delegate => !toRemove.Contains(@delegate))
                                           .ToArray();
            MailBox<S, E>.OnEvent = (Action<Event<S, E>>)Delegate.Combine(kept);
        }

        private static void UnregisterAll(Action handler) {
            if (handler is null) {
                return;
            }
            
            HashSet<Delegate> toRemove = handler.GetInvocationList().ToHashSet();
            Delegate[] kept = MailBox<S, E>.OnNotified.GetInvocationList()
                                           .Where(@delegate => !toRemove.Contains(@delegate))
                                           .ToArray();
            MailBox<S, E>.OnNotified = (Action)Delegate.Combine(kept);
        }
        
        internal static void Unregister(object subscriber) {
            if (MailBox<S, E>.Handlers.Remove(subscriber, out Action<Event<S, E>> handler)) {
                MailBox<S, E>.UnregisterAll(handler);
            }

            if (MailBox<S, E>.Listeners.Remove(subscriber, out Action listener)) {
                MailBox<S, E>.UnregisterAll(listener);
            }
        }
    }
}