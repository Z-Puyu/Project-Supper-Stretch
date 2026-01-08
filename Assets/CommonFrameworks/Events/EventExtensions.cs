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

        /// <summary>
        /// Sends an event to all subscribers.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="event">The event message to send.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Send<S, E>(this S sender, E @event) where S : class where E : IMessage {
            Mailbox<S, E>.Publish(sender, @event);
        }

        /// <summary>
        /// Sends an event to a specific subscriber.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="subscriber">The target subscriber.</param>
        /// <param name="event">The event message to send.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void SendTo<S, E>(this S sender, object subscriber, E @event) where S : class where E : IMessage {
            Mailbox<S, E>.PublishTo(subscriber, sender, @event);
        }

        /// <summary>
        /// Subscribes to an event from a specific sender.
        /// </summary>
        /// <param name="listener">The listener subscribing to the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Subscribe<S, E>(this object listener, Action<Event<S, E>> handler)
                where S : class where E : IMessage {
            Mailbox<S, E>.AddSubscription(listener, handler);
        }

        /// <summary>
        /// Unsubscribes an event handler from an event from a specific sender.
        /// </summary>
        /// <param name="listener">The listener unsubscribing from the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Unsubscribe<S, E>(this object listener, Action<Event<S, E>> handler)
                where S : class where E : IMessage {
            Mailbox<S, E>.RemoveSubscription(listener, handler);
        }

        /// <summary>
        /// Subscribes to an event from a specific sender.
        /// </summary>
        /// <param name="listener">The listener subscribing to the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Subscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
            Mailbox<S, E>.AddSubscription(listener, handler);
        }

        /// <summary>
        /// Unsubscribes an event handler from an event from a specific sender.
        /// </summary>
        /// <param name="listener">The listener unsubscribing from the event.</param>
        /// <param name="handler">The event handler delegate.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Unsubscribe<S, E>(this object listener, Action handler) where S : class where E : IMessage {
            Mailbox<S, E>.RemoveSubscription(listener, handler);
        }

        /// <summary>
        /// Blocks an event listener from receiving a specific event previously subscribed from a specific sender.
        /// </summary>
        /// <param name="listener">The listener to block the events.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Block<S, E>(this object listener) where S : class where E : IMessage {
            SubscriptionDescriptor subscription = new SubscriptionDescriptor(typeof(S), typeof(E));
            if (EventExtensions.ClearActions.TryGetValue(subscription, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        /// <summary>
        /// Blocks an event listener from receiving any previously subscribed event of a specific type.
        /// </summary>
        /// <param name="listener">The listener to block the events.</param>
        /// <typeparam name="E">The type of the event message.
        /// It must implement the <see cref="IMessage"/> interface.</typeparam>
        public static void Block<E>(this object listener) where E : IMessage {
            SubscriptionDescriptor @event = new SubscriptionDescriptor(EventType: typeof(E));
            if (EventExtensions.ClearActions.TryGetValue(@event, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        /// <summary>
        /// Blocks an event listener from receiving any previously subscribed event from a specific sender.
        /// </summary>
        /// <param name="listener">The listener to block the events.</param>
        /// <typeparam name="S">The type of the sender. It must be a reference type.</typeparam>
        public static void Blacklist<S>(this object listener) where S : class {
            SubscriptionDescriptor publisher = new SubscriptionDescriptor(PublisherType: typeof(S));
            if (EventExtensions.ClearActions.TryGetValue(publisher, out Action<object> action)) {
                action.Invoke(listener);
            }
        }

        /// <summary>
        /// Blocks an event listener from receiving all previously subscribed events.
        /// </summary>
        /// <param name="listener">The listener to block the events.</param>
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