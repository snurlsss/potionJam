using System;

namespace PubSub
{
    public abstract class Subscription
    {
        public int Priority { get; private set; }
        public bool IsAlive { get; internal set; } = true;

        protected Subscription(int priority)
        {
            Priority = priority;
        }

    }

    public class Subscription<T> : Subscription
    {
        public Action<T> Action { get; private set; }

        internal Subscription(Action<T> action, int priority) : base(priority)
        {
            Action = action;
        }
    }
}
