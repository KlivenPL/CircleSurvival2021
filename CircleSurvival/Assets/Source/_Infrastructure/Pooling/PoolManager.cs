using Assets.KLib.Source.Enums;
using Assets.KLib.Source.Enums.Attributes;
using Assets.Source._Infrastructure.Spawnable;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source._Infrastructure.Pooling {
    class PoolManager : IDisposable {

        public PoolManager() {
            masterParent = GameObject.Find("Pools").transform;
            poolableAttributes = InitalizePoolableAttributes();
        }

        private readonly Dictionary<SpawnableType, Pool> pools = new Dictionary<SpawnableType, Pool>();
        private readonly Dictionary<SpawnableType, PoolableAttribute> poolableAttributes;
        private readonly Transform masterParent;

        public bool Spawn(SpawnableType spawnableType, Vector3 position, Vector3 rotation, Vector3 scale, out GameObject spawnedObj) {
            if (!poolableAttributes.ContainsKey(spawnableType)) {
                spawnedObj = null;
                return false;
            }

            if (!pools.TryGetValue(spawnableType, out var pool)) {
                var poolable = poolableAttributes[spawnableType];
                pool = new Pool(spawnableType, Resources.Load<GameObject>(spawnableType.GetPrefabPath()), masterParent, poolable.StartPoolSize, poolable.MaxPoolSize);
                pools.Add(spawnableType, pool);
            }

            spawnedObj = pool.Spawn(position, rotation, scale);
            return true;

        }

        public bool Despawn(SpawnableType spawnable) {
            if (!poolableAttributes.ContainsKey(spawnable)) {
                return false;
            }

            pools[spawnable].Despawn();
            return true;
        }

        private Dictionary<SpawnableType, PoolableAttribute> InitalizePoolableAttributes() {
            return new Dictionary<SpawnableType, PoolableAttribute>(Enum.GetValues(typeof(SpawnableType)).Cast<SpawnableType>().Where(e => e.IsPoolable()).ToDictionary(e => e, e => e.GetPoolableAttribute()));
        }

        public void Dispose() {
            pools.Clear();
            poolableAttributes.Clear();
        }
    }
}
