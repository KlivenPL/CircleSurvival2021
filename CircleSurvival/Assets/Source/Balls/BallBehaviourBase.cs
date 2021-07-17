using Assets.KLib.Source.Utils.Rotation;
using Assets.Source.Effects;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    abstract class BallBehaviourBase {
        protected BallFacade ballFacade;
        protected EffectManager effectManager;
        protected AudioManager audioManager;

        [Inject]
        void Init(BallFacade ballFacade, EffectManager effectManager, AudioManager audioManager) {
            this.ballFacade = ballFacade;
            this.effectManager = effectManager;
            this.audioManager = audioManager;
        }

        virtual public void OnSpawn() {
            ScaleUp();
        }

        abstract public void OnTouch();

        private void ScaleUp() {
            ballFacade.StartCoroutine(ballFacade.transform.KScale(Vector3.zero, ballFacade.Parameters.Diameter * Vector3.one, 0.15f));
        }
    }
}
