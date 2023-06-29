using System.Threading.Tasks;
using Game.MiniGames.Interfaces;
using Game.Controls.Interfaces;
using Game.Cameras.Interfaces;
using Game.Vehicle.Interfaces;
using Cinemachine;
using DG.Tweening;
using Game.Player;
using Game.QuestPointer;
using Game.Tasks;
using Game.Tasks.Interfaces;
using Game.Vehicle.Enum;
using UnityEngine;
using Zenject;
using Utils;

namespace Game.Vehicle.Crane
{
    public class CraneController : MonoControllable, IControllable, ITaskConsumer
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private LogicCollision logicCollision;
        [SerializeField] private Rigidbody ballPhysics;
        [SerializeField] private GameObject ball;
        [SerializeField] private float amplitude = 70;

        private ICraneGame _craneGame;
        private IControlsInput _controlsInput;
        private ITutorialService _tutorialService;
        private ICameraService _cameraService;
        private IPointerManager _pointerManager;
        private PlayerController _playerController;
        private float _offsetX;
        private float _ballMass;
        private const float Lenght = 4.8f;
        private float _startAmplitude;
        private float _estimatedTime;
        private bool _isFalling;
        private bool _isTutorial;
        private float _timeCount;
        private Vector3 _ballPosition;

        [Inject]
        private void Construct
        (
            IControlsInput controlsInput,
            ICameraService cameraService,
            ICraneGame craneGame,
            IPointerManager pointerManager,
            ITutorialService tutorialService,
            TaskTracker.Factory taskFactory
        )
        {
            TaskTracker = taskFactory.Create();
            _pointerManager = pointerManager;
            _controlsInput = controlsInput;
            _cameraService = cameraService;
            if (!PlayerPrefs.HasKey("TutorialComplete"))
            {
                _isTutorial = true;
                _tutorialService = tutorialService;
            }

            _craneGame = craneGame;
        }

        public void TakeControl(PlayerController playerController)
        {
            ball.SetActive(true);
            TaskTracker.SetActive();
            if (_isTutorial)
            {
                _tutorialService.OpenUp("Destroy house");
                _tutorialService.OpenDown("Click to aim");
            }
            _pointerManager.Disable();
            _playerController = playerController;
            _playerController.gameObject.SetActive(false);
            TaskTracker.OnComplete += SetLeaveButton;
            _cameraService.ChangeView(virtualCamera);
            Invoke(nameof(ChangeEnable), 1);
        }

        private void ChangeEnable()
        {
            enabled = true;
            if(_isTutorial) _craneGame.StartGame(ThrowBall, _tutorialService);
            _craneGame.StartGame(ThrowBall);
        }

        private void LeaveVehicle()
        {
            _playerController.gameObject.SetActive(true);
            _playerController.SetTask();
            _cameraService.PlayerFocus();
            _controlsInput.HideButton();
            TaskTracker.DisableTracker();
            ball.SetActive(false);
            _craneGame.StopGame();
            Usable = false;
            Destroy(this);
        }

        private void ThrowBall(float ballPosition, float force)
        {
            var mass = _ballMass;
            mass *= force;
            _estimatedTime = mass > 100 ? 2 : 0.5f;
            ballPhysics.mass = mass;
            var position = _ballPosition;
            var targetPosition = Lenght * ballPosition / 800 + _offsetX;
            position = new Vector3(position.x, position.y, targetPosition);
            ball.transform.position = position;

            _startAmplitude = 70 * force;
            _timeCount = 0;
            _isFalling = true;
        }

        private void FixedUpdate()
        {
            if (!_isFalling) return;
            _timeCount += Time.fixedDeltaTime;
            var sin = Mathf.Sin(_timeCount) - 0.5f;
            amplitude = Mathf.Lerp(_startAmplitude, 0, _timeCount / 10f);
            var angle = (-sin * amplitude).FixAngle();
            ball.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (!(_timeCount > _estimatedTime)) return;
            _isFalling = false;
            ball.transform.DOMoveY(30, 1).OnComplete(() => { _craneGame.StartGame(ThrowBall); });
        }

        private void SetLeaveButton()
        {
            Invoke(nameof(LeaveVehicle), 2);
        }

        private void Start()
        {
            _offsetX = 11.8f + transform.position.z;
            _ballPosition = ball.transform.position;
            logicCollision.Initialize(TaskTracker);
            _ballMass = ballPhysics.mass;
        }

        public bool Usable { get; set; } = true;

        [field: SerializeField] public float Radius { get; private set; }

        public TaskTracker TaskTracker { get; private set; }
        public override EPointerType GetName => EPointerType.Crane;
        public override bool IsTaskDone => TaskTracker.IsTaskDone;
    }
}