using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 遊戲事件介面
    /// </summary>
    public interface IGameEvent { }

    /// <summary>
    /// 事件總線
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> handlers = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!handlers.ContainsKey(type))
            {
                handlers[type] = new List<Delegate>();
            }

            handlers[type].Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
            }
        }

        public static void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var type = typeof(T);
            if (handlers.TryGetValue(type, out var list))
            {
                foreach (var handler in list.ToArray())
                {
                    try
                    {
                        (handler as Action<T>)?.Invoke(gameEvent);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[EventBus] Error handling event {type.Name}: {e.Message}");
                    }
                }
            }
        }

        public static void Clear()
        {
            handlers.Clear();
            Debug.Log("[EventBus] All handlers cleared");
        }

        public static void Clear<T>() where T : IGameEvent
        {
            var type = typeof(T);
            if (handlers.ContainsKey(type))
            {
                handlers.Remove(type);
            }
        }
    }
}
