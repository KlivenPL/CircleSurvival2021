using Assets.Source.Balls;
using UnityEngine;
using Zenject;

namespace Assets.Source.DI.MonoInstallers.Balls {
    public class BombBallInstaller : MonoInstaller {

        public override void InstallBindings() {
            Container.BindInstance(GetComponent<BallFacade>()).AsSingle();
            Container.Bind(typeof(BallBehaviourBase), typeof(BombBallBehaviour), typeof(IFixedTickable)).To<BombBallBehaviour>().AsSingle();
            Container.BindInstance(GetComponent<Rigidbody2D>()).AsSingle();
        }
    }
}