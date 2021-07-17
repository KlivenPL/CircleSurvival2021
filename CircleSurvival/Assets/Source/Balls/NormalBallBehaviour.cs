using Assets.KLib.Source.Effects.LoopedAnimations;
using Assets.KLib.Source.Events;
using Assets.Source.Effects;
using Assets.Source.Events;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    class NormalBallBehaviour : BallBehaviourBase, IFixedTickable {

        private Renderer fillRenderer;
        private SpriteRenderer spriteRenderer;
        private PositionAnimation positionAnimation;
        private Background background;
        private Menu menu;
        private float timer;

        [Inject]
        public void Init(
            SpriteRenderer spriteRenderer,
            Renderer fillRenderer,
            PositionAnimation positionAnimation,
            Background background,
            Menu menu) {
            this.spriteRenderer = spriteRenderer;
            this.fillRenderer = fillRenderer;
            this.positionAnimation = positionAnimation;
            this.background = background;
            this.menu = menu;
        }

        public override void OnSpawn() {
            base.OnSpawn();
            timer = ballFacade.Parameters.DefuseTime;
            spriteRenderer.color = menu.ThemedCirclesEnabled ? background.CurrentColor * 1.25f : Color.green;
            positionAnimation.StartAnimation();
        }

        public override void OnTouch() {
            effectManager.PlayDefuseEffect(ballFacade.transform.position);
            ballFacade.Despawn();
        }

        public void FixedTick() {
            if (ballFacade.IsGameOver)
                return;

            if ((timer -= Time.fixedDeltaTime) <= 0) {
                KEvent.Fire<GameOverEvent>(new GameOverEvent(ballFacade));
                effectManager.PlayExplosionEffect(ballFacade.transform.position);
                ballFacade.Despawn();
                return;
            }

            Animate();
        }

        private void Animate() {
            var percent = (ballFacade.Parameters.DefuseTime - timer) / ballFacade.Parameters.DefuseTime;

            fillRenderer.material.SetFloat("_Cutoff", 1f - percent / 2f);
            positionAnimation.animables[0].Speed = percent * 60f;
        }
    }
}
