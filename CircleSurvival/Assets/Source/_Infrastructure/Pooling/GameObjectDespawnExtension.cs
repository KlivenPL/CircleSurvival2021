using UnityEngine;

namespace Assets.Source._Infrastructure.Pooling {
    public static class GameObjectDespawnExtension {
        public static void Despawn(this GameObject go) {
            if (go && go.transform.parent && go.transform.parent.parent) {
                var poolMb = go.transform.parent.parent.GetComponent<PoolMb>();
                if (poolMb.Pool != null) {
                    poolMb.Pool.Despawn(go);
                }
            } else if (go) {
                GameObject.Destroy(go);
            }
        }
    }
}
