using System;
using System.Collections.Generic;

namespace CommonFrameworks.Events {
    public static class EventExtensions {
        private readonly record struct SubscriptionDescriptor(Type? PublisherType = null, Type? EventType = null);

        private static readonly Dictionary<SubscriptionDescriptor, Action<object>> ClearActions =
                new Dictionary<SubscriptionDescriptor, Action<object>>();

        private static readonly Dictionary<object, ISet<SubscriptionDescriptor>> Subscriptions =
                new Dictionary<object, ISet<SubscriptionDescriptor>>();

        internal static void RegisterSubscription<S, E>(object subscriber) where S : class where E : IMessage {
            SubscriptionDescriptor subscription = new SubscriptionDescriptor(typeof(S), typeof(E));
            if (!EventExtensions.Subscriptions.TryGetValue(subscriber, out ISet<SubscriptionDescriptor> set)) {
                set = new HashSet<SubscriptionDescriptor> { subscription };
                EventExtensions.Subscriptions.Add(subscriber, set);
            } else {
                set.Add(subscription);
            }

            SubscriptionDescriptor publisher = new SubscriptionDescriptor(PublisherType: typeof(S));
            SubscriptionDescriptor @event = new SubscriptionDescriptor(EventType: typeof(E));
            if (!EventExtensions.ClearActions.ContainsKey(publisher)) {
                EventExtensions.ClearActions.Add(publisher, Mailbox<S, E>.ClearSubscriptions);
            } else if (!EventExtensions.ClearActions.ContainsKey(subscription)) {
                EventExtensions.ClearActions[publisher] += Mailbox<S, E>.ClearSubscriptions;
            }

            if (!EventExtensions.ClearActions.ContainsKey(@event)) {
                EventExtensions.ClearActions.Add(@event, Mailbox<S, E>.ClearSubscriptions);
            } else if (!EventExtensions.ClearActions.ContainsKey(subscription)) {
                EventExtensions.ClearActions[@event] += Mailbox<S, E>.ClearSubscriptions;
            }

            if (!EventExtensions.ClearActions.ContainsKey(subscription)) {
                EventExtensions.ClearActions.Add(subscription, Mailbox<S, E>.ClearSubscriptions);
            }
        }

        public static void Send<S, E>(this S sender, E @event) where S : class where E : IMessage {
            Mailbox<S, E>.Publish(sender, @event);
        }

        public static void SendTo<S, E>(this S sender, object subscriber, E @event) where E : IMessage where S : class {
            Mailbox<S, E>.PublishTo(subscriber, sender, @event);
        }

        public static void Subscribe<S, E>(this object listener, Action<Event<S, E>> handler)
                where S : class where E : IMessage {
            EventExtensions.RegisterSubscription<S, E>(listener);
            Mailbox<S, E>.AddSubscription(listener, handler);
        }

        public static void Unsubscribe<S, E>(this object listener, Action<Event<S, E>> handler)
                where S : class where E : IMessage {
            Mailbox<S, E>.RemoveSubscription(listener, handler);
        }

        public static void Subscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
            EventExtensions.RegisterSubscription<S, E>(listener);
            Mailbox<S, E>.AddSubscription(listener, handler);
        }

        public static void Unsubscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
            Mailbox<S, E>.RemoveSubscription(listener, handler);
        }

        public static void Mute<S, E>(this object listener) where S : class where E : IMessage {
            SubscriptionDescriptor subscription = new SubscriptionDescriptor(typeof(S), typeof(E));
            if (EventExtensions.ClearActions.TryGetValue(subscription, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        public static void Mute<E>(this object listener) where E : IMessage {
            SubscriptionDescriptor @event = new SubscriptionDescriptor(EventType: typeof(E));
            if (EventExtensions.ClearActions.TryGetValue(@event, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        public static void Forget<S>(this object listener) where S : class {
            SubscriptionDescriptor publisher = new SubscriptionDescriptor(PublisherType: typeof(S));
            if (EventExtensions.ClearActions.TryGetValue(publisher, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        public static void Mute(this object listener) {
            if (!EventExtensions.Subscriptions.TryGetValue(listener, out ISet<SubscriptionDescriptor> subscriptions)) {
                return;
            }

            foreach (SubscriptionDescriptor subscription in subscriptions) {
                if (EventExtensions.ClearActions.TryGetValue(subscription, out Action<object> action)) {
                    action.Invoke(listener);
                }
            }
        }
    }
}