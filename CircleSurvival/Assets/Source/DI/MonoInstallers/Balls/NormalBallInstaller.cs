using Assets.KLib.Source.Effects.LoopedAnimations;
using Assets.Source.Balls;
using UnityEngine;
using Zenject;

namespace Assets.Source.DI.MonoInstallers.Balls {
    public class NormalBallInstaller : MonoInstaller {

        public override void InstallBindings() {
            Container.BindInstance(GetComponent<BallFacade>()).AsSingle();
            Container.Bind(typeof(BallBehaviourBase), typeof(NormalBallBehaviour), typeof(IFixedTickable)).To<NormalBallBehaviour>().AsSingle();
            Container.BindInstance(transform.GetChild(0).GetComponent<Renderer>());
            Container.BindInstance(GetComponent<PositionAnimation>()).AsSingle();
            Container.BindInstance(GetComponent<SpriteRenderer>()).AsSingle();
            Container.BindInstance(GetComponent<Rigidbody2D>()).AsSingle();
        }
    }
}