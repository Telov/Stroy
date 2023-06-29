using System.Collections.Generic;
using General.Audio.Interfaces;
using Game.Controls.Interfaces;
using Game.Vehicle.Interfaces;
using Game.Cameras.Interfaces;
using Game.Tasks.Interfaces;
using Game.QuestPointer;
using UnityEngine.AI;
using UnityEngine;
using Cinemachine;
using Game.Player;
using Game.Tasks;
using Utils;
using Zenject;

namespace Game.Vehicle
{
    public abstract class MonoVehicle : MonoControllable, IControllable, ITaskConsumer
    {
        [SerializeField] protected BoxCollider boxCollider;
        [SerializeField] protected Rigidbody vehicleRigidbody;

        [SerializeField] private float speed;
        [SerializeField] private Outline outline;
        [SerializeField] private Transform[] wheels;
        [SerializeField] private GameObject[] skins;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private ParticleSystem leftTireParticles;
        [SerializeField] private ParticleSystem rightTireParticles;
        [SerializeField] protected CinemachineVirtualCamera virtualCamera;

        protected IAudioService AudioService;
        protected ICameraService CameraService;
        protected IControlsInput ControlsInput;
        protected IPointerManager PointerManager;
        protected PlayerController PlayerController;
        
        private ParticleSystem.EmissionModule _rightEmissionModule;
        private ParticleSystem.EmissionModule _leftEmissionModule;
        private Wheel[] _rightWheels;
        private Wheel[] _leftWheels;
        private Vector3 _startSize;
        private bool _isRiding;
        private int _skinId;

        [Inject]
        private void Construct
        (
            IControlsInput input,
            ICameraService cameraService,
            IPointer pointer,
            IPointerManager pointerManager,
            TaskTracker.Factory taskFactory,
            IAudioService audioService
        )
        {
            ControlsInput = input;
            TaskTracker = taskFactory.Create();
            AudioService = audioService;
            CameraService = cameraService;
            PointerManager = pointerManager;
        }

        public virtual void TakeControl(PlayerController playerController)
        {
            Governed = true;
            agent.enabled = true;
            outline.enabled = true;
            skins[_skinId].SetActive(true);
            rightTireParticles.gameObject.SetActive(true);
            leftTireParticles.gameObject.SetActive(true);
            _startSize = boxCollider.size;
            PlayerController = playerController;
            playerController.transform.parent = transform;
            playerController.gameObject.SetActive(false);
            CameraService.ChangeView(virtualCamera);
            ControlsInput.SetButton(LeaveVehicle);
            enabled = true;
        }


        protected virtual void LeaveVehicle()
        {
            Governed = false;
            agent.enabled = false;
            PointerManager.Disable();
            boxCollider.size = _startSize;
            skins[_skinId].SetActive(false);
            rightTireParticles.gameObject.SetActive(false);
            leftTireParticles.gameObject.SetActive(false);
            PlayerController.transform.parent = null;
            PlayerController.transform.position = FindLandingSide();
            PlayerController.gameObject.SetActive(true);
            TaskTracker.DisableTracker();
            CameraService.PlayerFocus();
            ControlsInput.HideButton();
            outline.enabled = false;
            PlayerController.SetTask();
            Usable = false;
            enabled = false;
        }

        private Vector3 FindLandingSide()
        {
            var vehicleTransform = transform;
            boxCollider.enabled = false;
            for (var i = 0; i < 4; i++)
            {
                var instance = vehicleTransform.position;
                var direction = i % 2 == 0 ? 1 : -1;
                if (i > 1)
                {
                    instance += vehicleTransform.forward * (direction * (boxCollider.size.z / 2));
                }
                else
                {
                    instance += vehicleTransform.right * (direction * (boxCollider.size.x / 2));
                }
                
                instance.y = 1;
                if (Physics.CheckSphere(instance, 0.1f)) continue;
                boxCollider.enabled = true;
                return instance;
            }

            boxCollider.enabled = true;
            return Vector3.zero;
        }

        private void WheelRotation(float velocity)
        {
            foreach (var rightWheel in _rightWheels)
            {
                var rotation = rightWheel.Circumference * velocity;
                rightWheel.WheelTransform.Rotate(new Vector3(rotation, 0, 0));
            }
            
            foreach (var leftWheel in _leftWheels)
            {
                var rotation = leftWheel.Circumference * velocity;
                leftWheel.WheelTransform.Rotate(new Vector3(-rotation, 0, 0));
            }
        }

        private void FixedUpdate()
        {
            var direction = ControlsInput.Direction;
            agent.velocity = new Vector3(direction.x, 0, direction.y) * (speed * Time.fixedDeltaTime);
            WheelRotation(agent.velocity.magnitude);
            if (!_isRiding && direction.magnitude > 0.8f)
            {
                _isRiding = true;
                _rightEmissionModule.enabled = true;
                _leftEmissionModule.enabled = true;
            }
            
            if(_isRiding && direction.magnitude < 0.5f)
            {
                _isRiding = false;
                _rightEmissionModule.enabled = false;
                _leftEmissionModule.enabled = false;
            }
        }

        private void Start()
        {
            var leftList = new List<Wheel>();
            var rightList = new List<Wheel>();
            
            foreach (var wheel in wheels)
            {
                if (wheel.localEulerAngles.y > 1)
                {
                    rightList.Add(new Wheel(wheel));
                }
                else
                {
                    leftList.Add(new Wheel(wheel));
                }
            }

            _leftWheels = leftList.ToArray();
            _rightWheels = rightList.ToArray();

            _rightEmissionModule = rightTireParticles.emission;
            _leftEmissionModule = leftTireParticles.emission;
            _rightEmissionModule.enabled = false;
            _leftEmissionModule.enabled = false;
        }

        private void Awake()
        {
            _skinId = PlayerPrefs.GetInt("SkinID",0);
        }

        protected bool Governed
        {
            get;
            private set;
        }

        public bool Usable { get; set; } = true;

        [field: SerializeField]
        public float Radius
        {
            get;
            private set;
        } = 2;

        public TaskTracker TaskTracker { get; private set; }

        public NavMeshAgent NavMeshAgent => agent;
    }
}
