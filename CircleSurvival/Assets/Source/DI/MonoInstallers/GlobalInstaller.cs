using Assets.KLib.Source.Interfaces;
using Assets.KLib.Source.Utils.SaveManagers;
using Assets.Source.Effects;
using Assets.Source.Scores;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller {
    public override void InstallBindings() {
        Container.BindInterfacesAndSelfTo<Score>().AsSingle();
        Container.Bind<ISaveManager>().To<KPlayerProps>().AsSingle();
        Container.BindInstance(GameObject.Find("Audio").GetComponent<AudioManager>()).AsSingle();
        Container.BindInstance(GameObject.Find("_Menu").GetComponent<Menu>()).AsSingle();
        Container.BindInstance(GameObject.Find("BG").GetComponent<Background>()).AsSingle();
    }
}