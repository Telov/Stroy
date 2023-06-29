using System;
using General.Audio.Interfaces;
using General.Constants;
using UnityEngine;
using Zenject;
using Task = System.Threading.Tasks.Task;

namespace Game.Garbage
{
    public class MonoGarbage : MonoBehaviour
    {
        [SerializeField] private GameObject garbageModel;
        [SerializeField] private GameObject treeModel;
        [SerializeField] private AnimationCurve appearCurve;
        [SerializeField] private AnimationCurve grabCurve;

        private float _currentTime;
        private bool _isGrabPlaying, _isAppearPlaying;
        private Transform _target;
        private Vector3 _destination;
        private IAudioService _audioService;
        private Action<MonoGarbage> _onDestroy;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void SetActiveModel(bool isTree)
        {
            IsTree = isTree;
            if (isTree)
            {
                treeModel.SetActive(true);
            }
            else
            {
                garbageModel.SetActive(true);
            }
        }

        public void DisableTreeModel()
        {
            garbageModel.SetActive(true);
            treeModel.SetActive(false);
        }

        public void PlayGrabAnimation(Transform destination, Action<MonoGarbage> disappearCallback)
        {
            if (_isGrabPlaying) return;
            gameObject.SetActive(true);
            gameObject.transform.parent = null;
            SphereCollider.enabled = false;
            _onDestroy ??= disappearCallback;
            _isGrabPlaying = true;
            _target = destination;
            _currentTime = 0;
            _audioService.PlayOneShot(AudioConstants.Collect);
        }

        public void PlayAppearAnimation(Vector3 destination)
        {
            if (_isAppearPlaying) return;
            SphereCollider.isTrigger = true;
            _isAppearPlaying = true;
            _currentTime = 0;
            _destination = destination;
            _destination.y = appearCurve.Evaluate(appearCurve.keys[^1].time);
            _audioService.PlayOneShot(AudioConstants.Collect);
        }

        private void GrabAnimation()
        {
            _currentTime += Time.deltaTime;

            var targetPosition = _target.position;
            targetPosition = new Vector3(targetPosition.x, grabCurve.Evaluate(grabCurve.keys[^1].time),
                targetPosition.z);

            var position = transform.position;
            position = new Vector3(position.x, grabCurve.Evaluate(_currentTime), position.z);

            position = Vector3.Lerp(position, targetPosition, _currentTime / 5f);
            transform.position = position;

            if (Vector3.Distance(position, targetPosition) > 0.5f) return;
            transform.SetParent(_target, true);
            transform.position = _target.position;
            _isGrabPlaying = false;
            gameObject.SetActive(false);
            SphereCollider.enabled = true;
            _onDestroy.Invoke(this);
            _onDestroy = null;
        }

        private void AppearAnimation()
        {
            Rigidbody.Sleep();
            _currentTime += Time.deltaTime;

            var position = transform.position;

            position = Vector3.Lerp(position, _destination, _currentTime / 5f);

            if (Vector3.Distance(position, _destination) < 0.1f)
            {
                _isAppearPlaying = false;
                SphereCollider.isTrigger = false;
                Rigidbody.WakeUp();
                Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
                ConstraintsOff();
                return;
            }

            transform.position = position;
        }

        private async void ConstraintsOff()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Rigidbody.constraints = RigidbodyConstraints.None;
        }

        private void Update()
        {
            if (_isGrabPlaying) GrabAnimation();

            if (_isAppearPlaying) AppearAnimation();
        }

        [field: SerializeField] public SphereCollider SphereCollider { get; private set; }

        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

        public bool IsTree { get; private set; }
    }
}