using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Pit;
using Game.Player;
using Game.QuestPointer;
using Game.Tasks;
using Game.Vehicle.Enum;
using Zenject;

namespace Game.Vehicle.Mixer
{
    public class Mixer : MonoVehicle
    {
        [SerializeField, Range(0, 1)] private float rotationSpeed;
        [SerializeField] private TrailRenderer trailRendererRight;
        [SerializeField] private TrailRenderer trailRendererLeft;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem smokeParticles;

        private PitController _pitController;
        private IPointerManager _pointerManager;
        private ITutorialService _tutorialService;
        private List<PitController> _pitControllers;

        private bool _isTutorial;
        private bool _isRunning;

        [Inject]
        private void Construct(IPointerManager pointerManager, ITutorialService tutorialService)
        {
            if (!PlayerPrefs.HasKey("TutorialComplete"))
            {
                _isTutorial = true;
                _tutorialService = tutorialService;
            }
            _pointerManager = pointerManager;
        }

        public override void TakeControl(PlayerController playerController)
        {
            base.TakeControl(playerController);
            if(_isTutorial) _tutorialService.OpenUp("Fill pit");
            TaskTracker.SetActive();
            _pitControllers = FindObjectsOfType<PitController>().Where(x => x.isClear).ToList();
            _pointerManager.Disable();
            if (_pitControllers.Count > 0)
            {
                var transformList = _pitControllers.Select(x => x.transform).ToList();
                _pointerManager.SetPointers(transform, transformList, 6);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Governed || _isRunning) return;

            if (!other.CompareTag("Pit")) return;
            var center = other.transform.parent;
            _pitController = center.GetComponent<PitController>();
            
            if (_pitController.isClear) FillPit(center);
        }

        private void FillPit(Transform center)
        {
            trailRendererLeft.emitting = false;
            trailRendererRight.emitting = false;
            StartCoroutine(nameof(Rotate), center);
            _isRunning = true;
            ControlsInput.HideButton();

            enabled = false;
        }

        private void ReturnState()
        {
            _isRunning = false;
            trailRendererLeft.emitting = true;
            trailRendererRight.emitting = true;
            _pitControllers.Remove(_pitController);
            if (_pitControllers.Count > 0)
            {
                var transformList = _pitControllers.Select(x => x.transform).ToList();
                _pointerManager.SetPointers(transform, transformList);
            }
            else
            {
                _pointerManager.Disable();
            }
            
            vehicleRigidbody.WakeUp();
            vehicleRigidbody.isKinematic = false;
            boxCollider.enabled = true;
            NavMeshAgent.enabled = true;

            TaskTracker.IncreaseValue();
            if (IsTaskDone)
            {
                ControlsInput.PlayAnimation();
            }

            smokeParticles.gameObject.SetActive(false);
            var emission = particles.emission;
            emission.enabled = false;

            ControlsInput.SetButton(LeaveVehicle);
            CameraService.ChangeView(virtualCamera);
            enabled = true;
        }

        private IEnumerator Rotate(Transform target)
        {
            if (_isRunning) yield break;
            var isChanged = false;
            var elapsedTime = 2;
            var timeCount = 0f;
            var relativePos = target.position - transform.position;
            relativePos.y = 0;
            var targetRotation = Quaternion.LookRotation(relativePos) * Quaternion.Euler(new Vector3(1, 180, 1));

            boxCollider.enabled = false;
            vehicleRigidbody.isKinematic = true;
            NavMeshAgent.enabled = false;
            vehicleRigidbody.Sleep();
            
            while (timeCount < elapsedTime)
            {
                if (!isChanged && timeCount > 0.5f)
                {
                    smokeParticles.gameObject.SetActive(true);
                    particles.gameObject.SetActive(true);
                    var emission = particles.emission;
                    emission.enabled = true;
                }
                
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeCount / elapsedTime);
                timeCount += Time.fixedDeltaTime * rotationSpeed;

                if (!isChanged && timeCount > 1)
                {
                    _pitController.FillCement(ReturnState);
                    isChanged = true;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public override EPointerType GetName => EPointerType.Mixer;
        public override bool IsTaskDone => TaskTracker.IsTaskDone;
    }
}