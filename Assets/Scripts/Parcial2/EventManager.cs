using System;
using System.Collections.Generic;

namespace Patterns.Observer.Struct_EventManager
{
    public static class EventManager
    {
        private static readonly Dictionary<Type, Delegate> _listeners = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            var type = typeof(T);
            _listeners[type] = _listeners.TryGetValue(type, out var existing)
                ? Delegate.Combine(existing, listener)
                : listener;
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var existing))
                return;

            var remaining = Delegate.Remove(existing, listener);
            if (remaining == null)
                _listeners.Remove(type);
            else
                _listeners[type] = remaining;
        }

        public static void Raise<T>(T eventData) where T : struct
        {
            if (_listeners.TryGetValue(typeof(T), out var listeners))
                ((Action<T>)listeners).Invoke(eventData);
        }
    }
}