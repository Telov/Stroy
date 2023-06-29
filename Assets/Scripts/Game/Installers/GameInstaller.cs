using General.Settings;
using Game.Controls;
using General.Audio;
using Game.Garbage;
using Cinemachine;
using Game.Cameras;
using Game.Levels;
using Game.MiniGames;
using Game.Particles;
using Game.Pause;
using Game.Player;
using Game.QuestPointer;
using Game.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private SerializableAudioSettings audioSettings;
        [SerializeField] private SerializableLevels levels;
        [SerializeField] private MonoPointer monoPointer;
        [SerializeField] private MonoTaskUI taskUI;
        [SerializeField] private Slider taskSlider;
        [SerializeField] private CraneGame craneGame;
        [SerializeField] private MonoWinPanel winPanel;
        [SerializeField] private ClickHandler clickHandler;
        [SerializeField] private MonoGarbage monoGarbagePrefab;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private MonoParticles particleInstance;
        [SerializeField] private SerializablePauseSettings pauseSettings;
        [SerializeField] private SerializablePointerSettings pointerSettings;
        [SerializeField] private SerializableTutorialSettings tutorialSettings;
        [SerializeField] private SerializableTogglerSettings togglerSettings;
        [SerializeField] private SerializableOutlineSettings outlineSettings;
        [SerializeField] private SerializableControlsSettings controlsSettings; 
        
        public override void InstallBindings()
        {
            Container
                .BindInstance(playerController)
                .AsSingle();
            
            Container
                .BindInterfacesTo<MonoTaskUI>()
                .FromInstance(taskUI)
                .AsSingle();

            Container
                .BindInterfacesTo<MonoPointer>()
                .FromInstance(monoPointer)
                .AsSingle();

            Container
                .BindInterfacesTo<ControlsInput>()
                .AsSingle()
                .WithArguments(controlsSettings);
            
            Container
                .BindInterfacesTo<TutorialService>()
                .AsSingle()
                .WithArguments(tutorialSettings);

            Container
                .BindInterfacesTo<SceneService>()
                .AsSingle()
                .WithArguments(levels)
                .NonLazy();

            Container
                .BindInterfacesTo<CraneGame>()
                .FromInstance(craneGame)
                .AsSingle();

            Container
                .BindInterfacesTo<OutlineManager>()
                .AsSingle()
                .WithArguments(outlineSettings);

            Container
                .BindInterfacesTo<ClickHandler>()
                .FromInstance(clickHandler)
                .AsSingle();

            Container
                .BindInterfacesTo<PauseService>()
                .AsSingle()
                .WithArguments(pauseSettings)
                .NonLazy();

            Container
                .BindInterfacesTo<MonoWinPanel>()
                .FromInstance(winPanel)
                .AsSingle();

            Container
                .BindInterfacesTo<CameraService>()
                .AsSingle()
                .WithArguments(virtualCamera);
            
            Container
                .Bind<AudioToggler>()
                .AsSingle()
                .WithArguments(togglerSettings)
                .NonLazy();


            Container
                .BindInterfacesTo<PointerManager>()
                .AsSingle()
                .WithArguments(pointerSettings);
                
            Container
                .BindInterfacesTo<AudioService>()
                .AsSingle()
                .WithArguments(audioSettings);
            
            
            audioSettings.Initialize();

            Container
                .BindMemoryPool<MonoGarbage, MemoryPool<MonoGarbage>>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(monoGarbagePrefab)
                .UnderTransformGroup("Garbage MemoryPool");

            Container
                .BindMemoryPool<MonoParticles, MemoryPool<MonoParticles>>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(particleInstance)
                .UnderTransformGroup("Particle MemoryPool");

            Container
                .BindFactory<TaskTracker, TaskTracker.Factory>().WithArguments(taskSlider);
        }
    }
}
