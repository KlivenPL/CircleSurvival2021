using Assets.Source._Infrastructure.Spawnable;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source._Infrastructure.Pooling {
    class Pool {
        private static DiContainer _container;
        private readonly int maxPoolSize;
        private readonly GameObject prefab;
        private readonly Transform parentPool;
        private readonly Transform parentSpawned;
        private static DiContainer Container => _container ??= GameObject.Find("GameSceneContext").GetComponent<SceneContext>().Container;

        private readonly Queue<GameObject> pool = new Queue<GameObject>();
        private readonly List<GameObject> spawned = new List<GameObject>();

        public Pool(SpawnableType spawnableType, GameObject prefab, Transform masterParent, int startPoolSize, int maxPoolSize) {
            if (maxPoolSize < 1 || startPoolSize > maxPoolSize)
                throw new System.Exception();

            SpawnableType = spawnableType;
            this.prefab = prefab;
            this.maxPoolSize = maxPoolSize;

            var parent = new GameObject($"{spawnableType}_pool").transform;
            parent.gameObject.AddComponent<PoolMb>().Init(this);
            parent.SetParent(masterParent, true);

            parentPool = new GameObject("Pool").transform;
            parentPool.SetParent(parent, true);

            parentSpawned = new GameObject("Spawned").transform;
            parentSpawned.SetParent(parent, true);

            InitializePool(startPoolSize);
        }

        public SpawnableType SpawnableType { get; private set; }

        private void InitializePool(int startPoolSize) {
            AddObjectsToPool(startPoolSize);
        }

        public GameObject Spawn(Vector3 position, Vector3 rotation, Vector3 scale) {
            if (pool.Count == 0) {
                AddObjectsToPool(1);
            }

            var obj = pool.Dequeue();
            SetPositionAndRotationAndScale(obj.transform, position, rotation, scale);
            obj.transform.SetParent(parentSpawned);
            obj.SetActive(true);
            spawned.Add(obj);

            return obj;
        }

        public void Despawn() {
            var oldestOne = spawned[0];
            spawned.RemoveAt(0);

            oldestOne.SetActive(false);
            oldestOne.transform.SetParent(parentPool);

            if (pool.Count >= maxPoolSize) {
                GameObject.Destroy(oldestOne);
            } else {
                pool.Enqueue(oldestOne);
            }
        }

        public bool Despawn(GameObject go) {
            if (spawned.Remove(go)) {
                go.SetActive(false);
                go.transform.SetParent(parentPool);

                if (pool.Count >= maxPoolSize) {
                    GameObject.Destroy(go);
                } else {
                    pool.Enqueue(go);
                }

                return true;
            }

            return false;
        }

        private void AddObjectsToPool(int count) {
            for (int i = 0; i < count; i++) {
                var newObject = Container.InstantiatePrefab(prefab);

                newObject.transform.SetParent(parentPool);
                newObject.SetActive(false);
                pool.Enqueue(newObject);
            }
        }

        private void SetPositionAndRotationAndScale(Transform obj, Vector3 position, Vector3 rotation, Vector3 scale) {
            obj.position = position;
            obj.eulerAngles = rotation;
            obj.localScale = scale;
        }

        public void Clear() {
            pool.Clear();
            spawned.Clear();
            _container = null;
        }
    }
}
