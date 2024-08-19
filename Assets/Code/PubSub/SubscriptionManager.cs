using System;
using System.Collections.Generic;

namespace PubSub
{
    internal abstract class SubscriptionManager { }

    internal class SubscriptionManager<T> : SubscriptionManager
    {
        SortedSet<Subscription<T>> subscriptions;
        static EventOrder<T> eventOrder = new EventOrder<T>();
        int publishDepth = 0;

        public bool HasSubcribers => subscriptions.Count > 0;
        public bool IsPublishing => publishDepth > 0;

        public SubscriptionManager()
        {
            subscriptions = new SortedSet<Subscription<T>>(eventOrder);
        }

        public Subscription<T> Sub(Action<T> action, int priority)
        {
            var sub = new Subscription<T>(action, priority);
            subscriptions.Add(sub);
            return sub;
        }

        public void Unsub(Subscription<T> subscription)
        {
            subscription.IsAlive = false;
            if (!IsPublishing)
            {
                subscriptions.Remove(subscription);
            }
        }

        public void Publish(T eventData)
        {
            publishDepth++;
            foreach (var sub in subscriptions)
            {
                if(sub.IsAlive)
                    sub.Action.Invoke(eventData);
            }
            publishDepth--;

            if (!IsPublishing)
            {
                subscriptions.RemoveWhere(x => !x.IsAlive);
            }
        }

        private class EventOrder<EventType> : IComparer<Subscription<EventType>>
        {
            public int Compare(Subscription<EventType> x, Subscription<EventType> y)
            {
                return x.Priority > y.Priority ? -1 : 1;
            }
        }
    }
}


