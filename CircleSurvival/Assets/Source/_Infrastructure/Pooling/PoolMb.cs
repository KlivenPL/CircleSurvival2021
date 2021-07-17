using UnityEngine;

namespace Assets.Source._Infrastructure.Pooling {
    class PoolMb : MonoBehaviour {
        public void Init(Pool pool) {
            Pool = pool;
        }

        public Pool Pool { get; private set; }

        private void OnDestroy() {
            Pool.Clear();
        }
    }
}
