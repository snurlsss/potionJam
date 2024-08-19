using System;
using System.Collections.Generic;
using System.Linq;

namespace PubSub
{
    public class EventHub
    {
        public static EventHub Default { get; private set; } = new EventHub();
        Dictionary<Type, SubscriptionManager> subscriptionManagers = new Dictionary<Type, SubscriptionManager>();
        SubscriptionHolder hubSubscriptions = new SubscriptionHolder();

        public void Publish<T>(T eventData)
        {
            if(TryGetSubManager<T>(out var subManager))
                subManager.Publish(eventData);
        }

        /// <summary>
        /// Checks if there are subscribers for the event before building the event. Useful if the event object is large or is being created a lot.
        /// </summary>
        public void PublishFast<T>(Func<T> eventBuilder)
        {
            if (TryGetSubManager<T>(out var subManager) && subManager.HasSubcribers)
                subManager.Publish(eventBuilder());
        }

        public bool HasSubs<T>()
        {
            return TryGetSubManager<T>(out var subManager) && subManager.HasSubcribers;
        }

        public Subscription<T> Sub<T>(Action<T> action, int priority = 0) 
        { 
            return GetOrCreateSubManager<T>().Sub(action, priority);
        }

        public void SubEasy<T>(Action<T> action, int priority = 0)
        {
            var subscription = Sub(action, priority);
            hubSubscriptions.Add(subscription);
        }

        public void Unsub<T>(Subscription<T> subcription)
        {
            GetOrCreateSubManager<T>().Unsub(subcription);
        }

        public void Unsub<T>(Action<T> action)
        {
            if(hubSubscriptions.TryGetSubFromAction(action, out var sub))
            {
                Unsub(sub);
                hubSubscriptions.Remove(sub);
            }
        }

        private bool TryGetSubManager<T>(out SubscriptionManager<T> subscriptionManager)
        {
            var key = typeof(T);
            if (subscriptionManagers.TryGetValue(key, out var subManager))
            {
                subscriptionManager = (SubscriptionManager<T>)subManager;
                return true;
            }
            subscriptionManager = null;
            return false;
        }

        private SubscriptionManager<T> GetOrCreateSubManager<T>()
        {
            var key = typeof(T);
            if (subscriptionManagers.TryGetValue(key, out var subManager))
            {
                return (SubscriptionManager<T>)subManager;
            }
            else
            {
                var newSubManager = new SubscriptionManager<T>();
                subscriptionManagers[key] = newSubManager;
                return newSubManager;
            }
        }

        private class SubscriptionHolder
        {
            Dictionary<Type, List<Subscription>> subscriptions = new Dictionary<Type, List<Subscription>>();

            public void Add<T>(Subscription<T> subscription)
            {
                GetOrCreateSubList<T>().Add(subscription);
            }

            public void Remove<T>(Subscription<T> subscription)
            {
                GetOrCreateSubList<T>().Remove(subscription);
            }

            public bool TryGetSubFromAction<T>(Action<T> action, out Subscription<T> sub)
            { 
                var subList = GetOrCreateSubList<T>();
                sub = subList.First(x => ((Subscription<T>)x).Action == action) as Subscription<T>;
                return sub != null;
            }

            private List<Subscription> GetOrCreateSubList<T>()
            {
                var key = typeof(T);
                if (subscriptions.TryGetValue(key, out var subList))
                {
                    return subList;
                }
                else
                {
                    var newSubManager = new List<Subscription>();
                    subscriptions[key] = newSubManager;
                    return newSubManager;
                }
            }
        }
    }
}
