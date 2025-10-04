using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 服務定位器
    /// </summary>
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        public static T Get<T>() where T : class, IService
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"[ServiceLocator] Service {type.Name} not found!");
            return null;
        }

        public static void Register<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} already registered!");
                return;
            }

            services[type] = service;
            service.Initialize();
            Debug.Log($"[ServiceLocator] Registered service: {type.Name}");
        }

        public static void Unregister<T>() where T : class, IService
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                service.Shutdown();
                services.Remove(type);
                Debug.Log($"[ServiceLocator] Unregistered service: {type.Name}");
            }
        }

        public static void Clear()
        {
            foreach (var service in services.Values)
            {
                service.Shutdown();
            }
            services.Clear();
            Debug.Log("[ServiceLocator] All services cleared");
        }

        public static bool Has<T>() where T : class, IService
        {
            return services.ContainsKey(typeof(T));
        }
    }
}
