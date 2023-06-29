using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UltimateFracturing;
using Game.Garbage;
using Game.Particles;
using Game.Particles.Enum;
using Game.Player;
using Game.Tasks;
using Game.Vehicle.Enum;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Vehicle.Bulldozer
{
    public class BulldozerController : MonoVehicle
    {
        [SerializeField] private Transform clawsCenter;
        [SerializeField] private Transform PointForSpawnGrable;
        [SerializeField] private GameObject particles;
        [SerializeField] private Animator animator;

        private bool _isTutorial;
        private IOutlineManager _outlineManager;
        private ITutorialService _tutorialService;
        private IMemoryPool<MonoParticles> _particlesPool;
        private IMemoryPool<MonoGarbage> _garbagePool;

        private readonly List<Outline> _particlesList = new();
        private readonly List<Transform> _allDebris = new();

        [Inject]
        private void Construct
        (
            MemoryPool<MonoParticles> particlesPool,
            MemoryPool<MonoGarbage> garbageMemoryPool,
            ITutorialService tutorialService,
            IOutlineManager outlineManager
        )
        {
            if (!PlayerPrefs.HasKey("TutorialComplete"))
            {
                _isTutorial = true;
                _tutorialService = tutorialService;
            }

            _outlineManager = outlineManager;
            _garbagePool = garbageMemoryPool;
            _particlesPool = particlesPool;
        }

        public override void TakeControl(PlayerController playerController)
        {
            base.TakeControl(playerController);
            if (_isTutorial)
            {
                _tutorialService.OpenUp("Turn into garbage");
                _tutorialService.OpenDown("Move to glowing things");
            }

            StartEngine();
        }

        private async void StartEngine()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            var debrises = FindObjectsOfType<MonoDebris>();
            foreach (var debris in debrises)
            {
                if (_allDebris.Contains(debris.transform)) continue;
                _allDebris.Add(debris.transform);
                var outline = debris.AddComponent<Outline>();
                _particlesList.Add(outline);
            }

            var chunks = FindObjectsOfType<DieTimer>();
            foreach (var timer in chunks)
            {
                if (_allDebris.Contains(timer.transform)) continue;
                _allDebris.Add(timer.transform);
                var instance = Instantiate(particles, timer.transform.position, Quaternion.identity);
                var outline = timer.AddComponent<Outline>();
                instance.transform.parent = timer.transform;
                _particlesList.Add(outline);
            }

            SelectNextTarget();
            var count = debrises.Length;
            var count1 = chunks.Length;
            _particlesList.ForEach(x =>
            {
                if (x) x.enabled = true;
            });
            TaskTracker.SetActive(count + count1);

            _outlineManager.SetShines(_particlesList);
            animator.enabled = true;
        }

        protected override void LeaveVehicle()
        {
            base.LeaveVehicle();
            _particlesList.ForEach(x =>
            {
                if (x) x.enabled = false;
            });

            _outlineManager.Disable();

            animator.enabled = false;
        }

        private void SelectNextTarget()
        {
            PointerManager.SetPointers(transform, _allDebris);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!Governed) return;

            if (other.TryGetComponent(out MonoDebris debris))
            {
                if (_isTutorial) _tutorialService.CloseDown();
                _allDebris.Remove(other.transform);
                SelectNextTarget();
                Destroy(debris);
                other.GetComponent<Collider>().enabled = false;
                other.transform.DOMove(clawsCenter.position, 0.4f);
                other.transform.DOScale(0, 0.4f).OnComplete(() =>
                {
                    SpawnGarbage(debris.IsTree, other.transform.position);
                    Destroy(other.gameObject);
                });
                animator.SetTrigger("Hit");
            }

            if (other.TryGetComponent(out DieTimer chunk))
            {
                _allDebris.Remove(other.transform);
                SelectNextTarget();
                Destroy(chunk);
                other.GetComponent<Collider>().enabled = false;
                other.transform.DOMove(clawsCenter.position, 0.4f);
                other.transform.DOScale(0, 0.4f).OnComplete(() =>
                {
                    SpawnGarbage(false, other.transform.position);
                    Destroy(other.gameObject);
                });
                animator.SetTrigger("Hit");
            }
        }

        private void SpawnGarbage(bool isTree, Vector3 spawnPosition)
        {
            var particleInstance = _particlesPool.Spawn();
            particleInstance.SetParticle(isTree ? EParticles.LeafExplosion : EParticles.DustPoof);
            particleInstance.SetActive(true);
            particleInstance.transform.position = spawnPosition;
            particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));
            var instance = _garbagePool.Spawn();
            instance.gameObject.SetActive(true);
            var bulldozerTransform = transform;
            var instanceTransform = instance.transform;
            instance.SetActiveModel(isTree);
            var forward = bulldozerTransform.forward;
            var position = bulldozerTransform.position - forward;
            instanceTransform.position = PointForSpawnGrable.position;
            Vector3 rotateObj = new Vector3();
            rotateObj.y = Random.Range(0, 360f);
            instanceTransform.transform.rotation = Quaternion.Euler(rotateObj);
            instance.PlayAppearAnimation(transform.position - transform.forward * 3);

            TaskTracker.IncreaseValue();
            if (IsTaskDone)
            {
                ControlsInput.PlayAnimation();
            }
        }

        public override EPointerType GetName => EPointerType.Bulldozer;
        public override bool IsTaskDone => TaskTracker.IsTaskDone;
    }
}