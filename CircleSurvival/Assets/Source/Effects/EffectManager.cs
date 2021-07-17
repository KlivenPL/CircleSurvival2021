using Assets.KLib.Source.Events;
using Assets.Source._Infrastructure.Pooling;
using Assets.Source._Infrastructure.Spawnable;
using Assets.Source.Events;
using EZCameraShake;
using UnityEngine;
using Zenject;

namespace Assets.Source.Effects {
    class EffectManager : IEventListener<GameOverEvent> {

        private readonly PoolManager poolManager;
        private readonly Background background;
        private readonly AudioManager audioManager;

        [Inject]
        public EffectManager(PoolManager poolManager, Background background, AudioManager audioManager) {
            this.ListenToEvent<GameOverEvent>();

            this.poolManager = poolManager;
            this.background = background;
            this.audioManager = audioManager;
        }

        public void OnEvent(GameOverEvent @event) {
            CameraShaker.Instance.ShakeOnce(3f, 3f, 0.1f, 1f);
            Handheld.Vibrate();
        }

        public void PlayDefuseEffect(Vector2 position) {
            poolManager.Spawn(SpawnableType.Effect_Defuse, (Vector3)position + new Vector3(0, 0, 4), Vector2.zero, Vector3.one, out var effect);
            var main = effect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startColor = background.CurrentColor * 1.5f;
            audioManager.PlaySfx(SoundEffect.Pop, 0.4f);
        }

        public void PlayExplosionEffect(Vector2 position) {
            poolManager.Spawn(SpawnableType.Effect_Explosion, (Vector3)position + new Vector3(0, 0, 4), Vector2.zero, Vector3.one, out var _);
            audioManager.PlayExplosionSfx();
        }
    }
}
