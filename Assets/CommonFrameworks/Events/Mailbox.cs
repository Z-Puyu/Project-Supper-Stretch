using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Events {
    public static class Mailbox<S, E> where S : class where E : IMessage {
        private static readonly IDictionary<object, ISet<Action<Event<S, E>>>> RegisteredHandlers =
                new Dictionary<object, ISet<Action<Event<S, E>>>>();
        
        private static readonly IDictionary<object, Action<Event<S, E>>> Handlers =
                new Dictionary<object, Action<Event<S, E>>>();

        private static readonly IDictionary<Action, Action<Event<S, E>>> ParameterlessHandlers =
                new Dictionary<Action, Action<Event<S, E>>>();

        private static event Action<Event<S, E>> OnEvent = delegate { };

        internal static void Send(S sender, E @event) {
            Mailbox<S, E>.OnEvent.Invoke(new Event<S, E>(sender, @event));
        }

        internal static void SendTo(object subscriber, S sender, E @event) {
            if (Mailbox<S, E>.Handlers.TryGetValue(subscriber, out Action<Event<S, E>> handler)) {
                handler.Invoke(new Event<S, E>(sender, @event));
            }
        }

        internal static bool Register(object subscriber, Action<Event<S, E>> handler) {
            if (Mailbox<S, E>.RegisteredHandlers.TryGetValue(subscriber, out ISet<Action<Event<S, E>>> existing)) {
                if (!existing.Add(handler)) {
                    return false;
                }
            } else {
                Mailbox<S, E>.RegisteredHandlers.Add(subscriber, new HashSet<Action<Event<S, E>>> { handler });
            }
            
            Mailbox<S, E>.OnEvent += handler;
            if (!Mailbox<S, E>.Handlers.TryAdd(subscriber, handler)) {
                Mailbox<S, E>.Handlers[subscriber] += handler;
            }

            return true;
        }

        internal static void Register(object subscriber, Action handler) {
            if (Mailbox<S, E>.ParameterlessHandlers.ContainsKey(handler)) {
                return;
            }

            Action<Event<S, E>> action = _ => handler.Invoke();
            if (!Mailbox<S, E>.Register(subscriber, action)) {
                return;
            }

            Mailbox<S, E>.ParameterlessHandlers[handler] = action;
        }

        internal static void Unregister(object subscriber, Action<Event<S, E>> handler) {
            if (!Mailbox<S, E>.Handlers.TryGetValue(subscriber, out Action<Event<S, E>>? existing)) {
                return;
            }

            Mailbox<S, E>.OnEvent -= handler;
            existing -= handler;
            if (existing is null) {
                Mailbox<S, E>.Handlers.Remove(subscriber);
            } else {
                Mailbox<S, E>.Handlers[subscriber] = existing;
            }
        }

        internal static void Unregister(object subscriber, Action handler) {
            if (!Mailbox<S, E>.ParameterlessHandlers.Remove(handler, out Action<Event<S, E>> existing)) {
                return;
            }

            Mailbox<S, E>.Unregister(subscriber, existing);
        }

        internal static void Unregister(object subscriber) {
            if (!Mailbox<S, E>.Handlers.Remove(subscriber, out Action<Event<S, E>> handler)) {
                return;
            }

            HashSet<Delegate> invalid = handler.GetInvocationList().ToHashSet();
            IEnumerable<Delegate> kept = Mailbox<S, E>.OnEvent.GetInvocationList()
                                                      .Where(del => !invalid.Contains(del));
            Mailbox<S, E>.OnEvent = (Action<Event<S, E>>)Delegate.Combine(kept.ToArray());
        }
    }
}
