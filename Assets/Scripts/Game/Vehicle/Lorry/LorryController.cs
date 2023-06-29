using System.Collections.Generic;
using System.Linq;
using Game.Garbage;
using Game.Particles;
using Game.Particles.Enum;
using Game.Player;
using Game.QuestPointer;
using Game.Tasks;
using Game.Vehicle.Enum;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.Vehicle.Lorry
{
    public class LorryController : MonoVehicle
    {
        private MemoryPool<MonoParticles> _particlesPool;

        [SerializeField] private int bootStartSize;
        [SerializeField] private LorryBoot boot;
        [SerializeField] private Transform[] trashbox;

        private bool _isTutorial;
        private IOutlineManager _outlineManager;
        private ITutorialService _tutorialService;

        private readonly List<IPointer> _pointers = new();
        private readonly List<Outline> _particlesList = new();
        private readonly List<MonoGarbage> _garbageList = new();

        [Inject]
        private void Construct(MemoryPool<MonoParticles> particles, ITutorialService tutorialService,
            IOutlineManager outlineManager)
        {
            if (!PlayerPrefs.HasKey("TutorialComplete"))
            {
                _isTutorial = true;
                _tutorialService = tutorialService;
            }

            _particlesPool = particles;
            _outlineManager = outlineManager;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!Governed) return;

            if (boot.IsAble && other.TryGetComponent(out MonoGarbage garbage) && !boot.IsUnloading)
            {
                if (_isTutorial) _tutorialService.CloseDown();
                var particleInstance = _particlesPool.Spawn();
                particleInstance.SetParticle(EParticles.DustPoof);
                particleInstance.SetActive(true);
                particleInstance.transform.position = other.transform.position;
                particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));

                boot.IsLoading = true;
                _garbageList.Remove(garbage);
                SelectNextTarget();
                boot.Increase(garbage);
                garbage.DisableTreeModel();
                garbage.PlayGrabAnimation(boot.transform, DestroyGarbage);
            }

            if (_isTutorial && boot.Count == 3)
            {
                _tutorialService.Close();
                _tutorialService.OpenUp("Drop trash into trashbox");
                _tutorialService.OpenDown("Move to trashbox");
            }

            if (other.CompareTag("Trashbox") && !boot.IsUnloading && !boot.IsLoading)
            {
                if(_isTutorial && boot.Count == 3) PlayerPrefs.SetString("TutorialComplete", "Complete");
                boot.Unload(other.transform, TaskTracker.IncreaseValue);
            }
        }

        public override void TakeControl(PlayerController playerController)
        {
            base.TakeControl(playerController);
            if (_isTutorial)
            {
                _tutorialService.OpenUp("Collect all trash");
                _tutorialService.OpenDown("Move to glowing trash");
            }

            if (!IsTaskDone)
            {
                foreach (var trashTransform in trashbox)
                {
                    var pointer = PointerManager.SpawnPointer();
                    pointer.SetPoint(transform, trashTransform.position, EPointerType.Trashbox);
                    _pointers.Add(pointer);
                }
            }

            var garbages = FindObjectsOfType<MonoGarbage>();
            if (garbages.Length > 0)
            {
                foreach (var garbage in garbages)
                {
                    if (_garbageList.Contains(garbage)) continue;
                    if (garbage.GetComponent<Outline>()) continue;
                    var outline = garbage.AddComponent<Outline>();
                    outline.OutlineWidth = 3;
                    outline.OutlineColor = Color.yellow;
                    _garbageList.Add(garbage);
                    _particlesList.Add(outline);
                }
            }

            var inactiveCount = boot.Count;
            boot.SetActive(true);
            _particlesList.ForEach(x => x.enabled = true);
            _outlineManager.SetShines(_particlesList);
            TaskTracker.SetActive(garbages.Length, inactiveCount);
            //boot.MaxCount = Mathf.Min(TaskTracker.MaxValue ,12);
            boot.MaxCount = bootStartSize;
            SelectNextTarget();
        }

        protected override void LeaveVehicle()
        {
            base.LeaveVehicle();
            boot.SetActive(false);
            _particlesList.ForEach(x => x.enabled = false);
            _pointers.ForEach(pointer => pointer.Disable());
            _outlineManager.Disable();
        }

        private void SelectNextTarget()
        {
            var transformList = _garbageList.Select(x => x.transform);
            PointerManager.SetPointers(transform, transformList.ToList());
            if (IsTaskDone)
            {
                ControlsInput.PlayAnimation();
            }
        }

        private void DestroyGarbage(MonoGarbage instance)
        {
            instance.gameObject.SetActive(false);
            boot.VisualIncrease();
            boot.UpdateText();
        }

        public override EPointerType GetName => EPointerType.Lorry;
        public override bool IsTaskDone => TaskTracker.IsTaskDone;
    }
}