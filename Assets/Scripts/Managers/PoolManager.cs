using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class PoolConfig
        {
            public PoolEnum type;
            public int initialCount = 10;
        }

        [SerializeField] private PoolConfig[] poolConfigs;

        private readonly Dictionary<PoolEnum, Queue<Poolable>> poolDict = new();
        private readonly Dictionary<PoolEnum, Poolable> prefabDict = new();

        private void Awake()
        {
            InitializePools();
        }

        public void InitializePools()
        {
            poolDict.Clear();
            prefabDict.Clear();
            
            foreach (var config in poolConfigs)
            {
                Poolable prefab = Resources.Load<Poolable>($"Poolables/{config.type}");
                if (prefab == null)
                {
                    Debug.LogError($"Poolable prefab not found at Resources/Poolables/{config.type}");
                    continue;
                }

                prefabDict[config.type] = prefab;
                poolDict[config.type] = new Queue<Poolable>();

                for (int i = 0; i < config.initialCount; i++)
                {
                    // Fill the pool with inactive objects
                    var instance = CreateNewInstance(config.type);
                    ReturnToPool(instance);
                }
            }
            print("init pool state:");
            LogFullPoolState();
        }

        /// <summary>
        /// Creates a new poolable instance without enqueuing it.
        /// </summary>
        private Poolable CreateNewInstance(PoolEnum type)
        {
            Poolable prefab = prefabDict[type];
            Poolable instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instance.Type = type;
            instance.Initialize();
            instance.gameObject.SetActive(false);
            return instance;
        }

        /// <summary>
        /// Generic version: returns the object as the desired type.
        /// </summary>
        public T GetFromPool<T>(PoolEnum type) where T : Poolable
        {

            if (!poolDict.ContainsKey(type))
            {
                Debug.LogError($"No pool found for type {type}");
                return null;
            }

            Poolable obj = poolDict[type].Count > 0
                ? poolDict[type].Dequeue()
                : CreateNewInstance(type);
            print($"alert: requested {type} and got {obj}");

            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                obj.OnGetFromPool();
            }


            if (obj is T tObj)
            {
                print($"alert: returning {obj}");

                return tObj;
            }

            Debug.LogError($"Object from pool is not of type {typeof(T)}");
            return null;
        }

        public void ReturnToPool(Poolable poolable)
        {
            poolable.OnReturnToPool();
            poolDict[poolable.Type].Enqueue(poolable);
            print($"alert: returned {poolable.Type} to pool!");
        }
        

        private void LogFullPoolState()
        {
            Debug.Log("====== Pool State ======");
            foreach (var kvp in poolDict)
            {
                Debug.Log($"Type: {kvp.Key}, Count: {kvp.Value.Count}");
            }
            Debug.Log("========================");
        }

    }

    public enum PoolEnum
    {
        None = 0,
        Arrow = 1,
        ExplodableTile = 2, // not in use!
        ExplodableTileParticles = 3,
        LavaSplashParticles = 4,
        LavaBurstParticles = 5,
        ClashParticles = 6,
        PlayerHitEnemyParticles = 7,
        EnemyHitPlayerParticles = 8,
        EnemyHitWallParticles = 9,
        ExplodableTileParticlesV2 = 10,
    }
}
