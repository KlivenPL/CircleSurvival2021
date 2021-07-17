using Assets.KLib.Source.Enums.Attributes;

namespace Assets.Source._Infrastructure.Spawnable {
    public enum SpawnableType {
        #region Balls

        [Poolable(startPoolSize: 6, maxPoolSize: 20)]
        [PrefabPath("Balls/Ball_Normal")]
        Ball_Normal,

        [Poolable(startPoolSize: 6, maxPoolSize: 20)]
        [PrefabPath("Balls/Ball_Bomb")]
        Ball_Bomb,

        #endregion

        #region Effects

        [Poolable(startPoolSize: 6, maxPoolSize: 10)]
        [PrefabPath("Effects/Effect_Defuse")]
        Effect_Defuse,

        [Poolable(startPoolSize: 2, maxPoolSize: 5)]
        [PrefabPath("Effects/Effect_Explosion")]
        Effect_Explosion,

        #endregion
    }
}

