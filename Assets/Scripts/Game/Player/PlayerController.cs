using System.Collections.Generic;
using System.Linq;
using Game.Controls.Interfaces;
using Game.Levels.Interfaces;
using Game.QuestPointer;
using Game.Vehicle;
using Game.Vehicle.Interfaces;
using UnityEngine.AI;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject[] skins;
        [SerializeField] private FillingRing interactionService;

        private bool _isFilling;
        private bool _isRunning;
        private IWinService _winPanel;
        private IControlsInput _controlsInput;
        private List<MonoControllable> _transforms;
        private readonly List<IPointer> _pointers = new();
        private IPointerManager _pointerManager;
        private static readonly int Running = Animator.StringToHash("Running");

        [Inject]
        private void Construct(IControlsInput controlsInput, IWinService winService, IPointerManager pointerManager)
        {
            _winPanel = winService;
            _controlsInput = controlsInput;
            _pointerManager = pointerManager;
        }

        public void SetTasks(IEnumerable<MonoControllable> transforms)
        {
            _transforms = transforms.ToList();
            foreach (var task in _transforms)
            {
                var pointer = _pointerManager.SpawnPointer();
                pointer.SetPoint(transform, task.transform.position, task.GetName);
                _pointers.Add(pointer);
            }
        }

        public void SetTask()
        {
            for (int i = 0; i < _pointers.Count; i++)
            {
                if (!_transforms[i].IsTaskDone)
                {
                    _pointers[i].SetPoint(transform, _transforms[i].transform.position, _transforms[i].GetName);
                }
            }
        }

        private void Update()
        {
            var direction = _controlsInput.Direction;
            NavMeshAgent.velocity = new Vector3(direction.x, 0, direction.y) * (speed * Time.fixedDeltaTime);

            animator.SetBool(Running, direction.magnitude > 0.1f);
            _isRunning = direction.magnitude > 0;
            if (_isFilling && _isRunning)
            {
                interactionService.InterruptInteraction();
                _isFilling = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _winPanel.SetActive();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isFilling) return;
            if (other.TryGetComponent(out IControllable vehicle) && vehicle.Usable)
            {
                interactionService.StartInteract(() => vehicle.TakeControl(this));
                _isFilling = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IControllable vehicle))
            {
                interactionService.InterruptInteraction();
                vehicle.Usable = true;
                _isFilling = false;
            }
        }

        private void Start()
        {
            var skinId = PlayerPrefs.GetInt("SkinID", 0);
            var skinInstance = skins[skinId];
            skinInstance.SetActive(true);
            animator = skinInstance.transform.GetComponentInChildren<Animator>();
            _controlsInput.HideButton();
        }

        private void OnDisable()
        {
            _pointers.ForEach(pointer => pointer.Disable());
        }
        
        [field:SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
    }
}
