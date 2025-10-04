using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 物件池
    /// </summary>
    public class ObjectPool
    {
        private readonly GameObject prefab;
        private readonly Transform parent;
        private readonly Queue<GameObject> pool = new Queue<GameObject>();
        private readonly int initialSize;

        public ObjectPool(GameObject prefab, int initialSize = 10, Transform parent = null)
        {
            this.prefab = prefab;
            this.initialSize = initialSize;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }

        public GameObject Get()
        {
            GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
            obj.SetActive(true);
            return obj;
        }

        public T Get<T>() where T : Component
        {
            return Get().GetComponent<T>();
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        public void Return(Component component)
        {
            Return(component.gameObject);
        }

        public void Clear()
        {
            while (pool.Count > 0)
            {
                Object.Destroy(pool.Dequeue());
            }
        }
    }

    /// <summary>
    /// 物件池管理器
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager instance;
        public static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("PoolManager");
                    instance = go.AddComponent<PoolManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private readonly Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        public void CreatePool(string poolName, GameObject prefab, int initialSize = 10)
        {
            if (pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"[PoolManager] Pool '{poolName}' already exists!");
                return;
            }

            Transform poolParent = new GameObject($"Pool_{poolName}").transform;
            poolParent.SetParent(transform);

            pools[poolName] = new ObjectPool(prefab, initialSize, poolParent);
            Debug.Log($"[PoolManager] Created pool: {poolName}");
        }

        public GameObject Get(string poolName)
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                return pool.Get();
            }

            Debug.LogError($"[PoolManager] Pool '{poolName}' not found!");
            return null;
        }

        public T Get<T>(string poolName) where T : Component
        {
            GameObject obj = Get(poolName);
            return obj != null ? obj.GetComponent<T>() : null;
        }

        public void Return(string poolName, GameObject obj)
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogError($"[PoolManager] Pool '{poolName}' not found!");
            }
        }

        public void Return(string poolName, Component component)
        {
            Return(poolName, component.gameObject);
        }

        public void ClearPool(string poolName)
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                pool.Clear();
                pools.Remove(poolName);
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
        }
    }
}
