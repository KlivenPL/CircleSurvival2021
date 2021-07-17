using Assets.KLib.Source.Events;
using Assets.KLib.Source.Utils.Rotation;
using Assets.Source.Events;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    class BombBallBehaviour : BallBehaviourBase, IFixedTickable {

        private float timer;

        public override void OnSpawn() {
            base.OnSpawn();
            audioManager.PlaySfx(SoundEffect.BlackHole, 0.6f);
            timer = ballFacade.Parameters.DefuseTime;
        }

        public override void OnTouch() {
            if (!ballFacade.IsGameOver && timer > 0)
                KEvent.Fire<GameOverEvent>(new GameOverEvent(ballFacade));
        }

        public void FixedTick() {
            if (ballFacade.IsGameOver)
                return;

            if ((timer -= Time.fixedDeltaTime) <= 0 && timer != -1) {
                ScaleDown();
                ballFacade.StartCoroutine(DespawnCoroutine());
                timer = -1;
                return;
            }
        }

        private void ScaleDown() {
            ballFacade.StartCoroutine(ballFacade.transform.KScale(ballFacade.transform.localScale, new Vector3(0.01f, 0.01f, 0.01f), 0.1f));
        }

        private IEnumerator DespawnCoroutine() {
            yield return new WaitForSeconds(0.5f);
            ballFacade.Despawn();
        }
    }
}
