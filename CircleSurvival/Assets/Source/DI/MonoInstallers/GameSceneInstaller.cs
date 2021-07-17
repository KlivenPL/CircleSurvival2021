using Assets.KLib.Source.Input;
using Assets.Source._Infrastructure.Pooling;
using Assets.Source.Balls;
using Assets.Source.Cameras;
using Assets.Source.DI;
using Assets.Source.Effects;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller {

    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private CameraManager cameraManager;

    public override void InstallBindings() {

#if UNITY_EDITOR
        InstallEditor();
#elif UNITY_ANDROID
        InstallMobile();
#elif UNITY_IOS
        InstallMobile();
#endif

        Container.Bind<PoolManager>().AsSingle();
        Container.Bind<EffectManager>().AsSingle();
        Container.BindInstance(ballSpawner).AsSingle();
        Container.BindInstance(cameraManager).AsSingle();
        Container.BindInstance(Camera.main).WithId(InjectId.GameCamera).AsSingle();
        Container.BindInstance(GameObject.Find("Background").GetComponent<Background>()).AsSingle();
    }

    private void InstallMobile() {
        Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle();
    }

    private void InstallEditor() {
        Container.BindInterfacesAndSelfTo<MobileInputKeyboardMock>().AsSingle();
    }
}