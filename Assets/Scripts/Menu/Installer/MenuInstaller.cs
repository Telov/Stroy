using General.Audio;
using General.Settings;
using Menu.Loading;
using UnityEngine;
using Zenject;

namespace Menu.Installer
{
    public class MenuInstaller : MonoInstaller
    {
        [SerializeField] private SerializableTogglerSettings togglerSettings;
        [SerializeField] private LoadingScreen loadingScreen; 
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<LoadingScreen>()
                .FromInstance(loadingScreen)
                .AsSingle();

            Container
                .Bind<AudioToggler>()
                .AsSingle()
                .WithArguments(togglerSettings)
                .NonLazy();
        }
    }
}