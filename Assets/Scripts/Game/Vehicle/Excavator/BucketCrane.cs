using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Game.Controls.Interfaces;
using Game.Particles;
using Game.Particles.Enum;
using Game.Pit;
using Game.Player;
using Game.QuestPointer;
using Game.Tasks;
using Game.Vehicle.Enum;
using General.Constants;
using UnityEngine;
using Zenject;

namespace Game.Vehicle.Excavator
{
    public class BucketCrane : MonoVehicle
    {
        [SerializeField] private CinemachineVirtualCamera pitVirtualCamera;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Animator animator;
        [SerializeField] private TrailRenderer trailRendererRight;
        [SerializeField] private TrailRenderer trailRendererLeft;

        private float _timeCount;
        private bool _isWorked;
        private bool _isTutorial;
        private MemoryPool<MonoParticles> _particlesPool;
        private List<PitController> _pitControllers;
        private PitController _pitController;
        private IClickHandler _clickHandler;
        private ITutorialService _tutorialService;
        private IPointerManager _pointerManager;
        
        private event Action OnDirtDestroy = () => { }; 
        private static readonly int Action1 = Animator.StringToHash("Action");

        [Inject]
        private void Construct(MemoryPool<MonoParticles> particles, IClickHandler clickHandler, IPointerManager pointerManager, ITutorialService tutorialService)
        {
            if (!PlayerPrefs.HasKey("TutorialComplete"))
            {
                _isTutorial = true;
                _tutorialService = tutorialService;
            }
            
            _particlesPool = particles;
            _clickHandler = clickHandler;
            _pointerManager = pointerManager;
        }

        public override void TakeControl(PlayerController playerController)
        {
            if (_isTutorial)
            {
                _tutorialService.OpenDown("Move Joystick");
            }
            base.TakeControl(playerController);
            _pointerManager.Disable();
            _pitControllers = FindObjectsOfType<PitController>().Where(x => !x.isClear).Select(x => x).ToList();
            _pointerManager.SetPointers(transform, _pitControllers.Select(x => x.transform).ToList(),8);
        }


        private IEnumerator Rotate(Transform target)
        {
            trailRendererRight.emitting = false;
            trailRendererLeft.emitting = false;
            _timeCount = 0f;
            var elapsedTime = 4;
            var rotationSpeed = 0.6f;
            
            var relativePos = target.parent.position - transform.position;
            relativePos.y = 0;
            var targetRotation = Quaternion.LookRotation(relativePos);

            boxCollider.enabled = false;
            vehicleRigidbody.isKinematic = true;
            NavMeshAgent.enabled = false;
            vehicleRigidbody.Sleep();
            while (_timeCount < elapsedTime)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _timeCount / elapsedTime);
                _timeCount += Time.fixedDeltaTime * rotationSpeed;

                yield return new WaitForFixedUpdate();
            }
        }

        private void Complete()
        {
            if (_pitControllers.Count > 0)
            {
                var transformList = _pitControllers.Where(x => !x.isClear).Select(x => x.transform).ToList();
                _pointerManager.SetPointers(transform, transformList);
            }
            else
            {
                _pointerManager.Disable();
            }

            _timeCount = 10;
            trailRendererRight.emitting = true;
            trailRendererLeft.emitting = true;
            vehicleRigidbody.WakeUp();
            vehicleRigidbody.isKinematic = false;
            boxCollider.enabled = true;
            NavMeshAgent.enabled = true;
            ControlsInput.SetButton(LeaveVehicle);
            CameraService.ChangeView(virtualCamera);
            enabled = true;
            _clickHandler.SetActive(false);
            _pitController.isClear = true;
            TaskTracker.OnComplete -= Complete;
        }

        private void MakeDamage(RaycastHit hit)
        {
            animator.SetTrigger(Action1);
            var dirt = hit.transform.GetComponent<Dirt>();
            if (dirt)
            {
                var particleInstance = _particlesPool.Spawn();
                particleInstance.SetParticle(EParticles.DustPoof);
                particleInstance.SetActive(true);
                particleInstance.transform.position = hit.point;
                particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));
                AudioService.PlayOneShot(AudioConstants.Damage);
                dirt.TakeDamage(OnDirtDestroy);
            }
        }

        private void Update()
        {
            if (_isTutorial && ControlsInput.Direction.magnitude > 0.2f)
            {
                _tutorialService.Close();
                _tutorialService.OpenUp("Dig a hole");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isWorked) return;
            if (other.CompareTag("Pit"))
            {
                if (_isTutorial)  _tutorialService.OpenDown("Tap to dig");
                _pitController = other.GetComponentInParent<PitController>();
                _clickHandler.SetLayerMask(layerMask);
                _clickHandler.OnGetHit += MakeDamage;
                _clickHandler.SetActive(true);
                StartCoroutine(nameof(Rotate), other.transform);
                _isWorked = true;

                TaskTracker.SetActive();
                TaskTracker.OnComplete += Complete;
                OnDirtDestroy = () =>
                {
                    AudioService.PlayOneShot(AudioConstants.Destroy);
                    TaskTracker.IncreaseValue();
                    if (IsTaskDone)
                    {
                        ControlsInput.PlayAnimation();
                    }
                };
                CameraService.ChangeView(pitVirtualCamera);
                ControlsInput.HideButton();
                enabled = false;
            }
        }
        
        public override EPointerType GetName => EPointerType.Bucket;
        public override bool IsTaskDone => TaskTracker.IsTaskDone;
    }
}
