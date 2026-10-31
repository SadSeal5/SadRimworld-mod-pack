using System;
using System.Collections.Generic;

namespace SadSeal.Core.Events
{
    public static class ModEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();
        private static readonly object lockObj = new object();

        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            lock (lockObj)
            {
                Type type = typeof(T);
                if (!subscribers.TryGetValue(type, out var list))
                {
                    list = new List<Delegate>();
                    subscribers[type] = list;
                }
                if (!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            lock (lockObj)
            {
                Type type = typeof(T);
                if (subscribers.TryGetValue(type, out var list))
                {
                    list.Remove(handler);
                    if (list.Count == 0)
                    {
                        subscribers.Remove(type);
                    }
                }
            }
        }

        public static void Publish<T>(T eventData)
        {
            List<Delegate> callbacks = null;
            lock (lockObj)
            {
                if (subscribers.TryGetValue(typeof(T), out var list))
                {
                    callbacks = new List<Delegate>(list);
                }
            }

            if (callbacks != null)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    try
                    {
                        ((Action<T>)callbacks[i])(eventData);
                    }
                    catch (Exception ex)
                    {
                        SadSeal.Common.SadLog.Error("EventBus", $"Error executing handler for {typeof(T).Name}: {ex}");
                    }
                }
            }
        }

        public static void Clear()
        {
            lock (lockObj)
            {
                subscribers.Clear();
            }
        }
    }
}
