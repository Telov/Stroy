using System.Collections.Generic;
using Game.Vehicle.Enum;
using UnityEngine;
using Utils;

namespace Game.QuestPointer
{
    public class MonoPointer : MonoBehaviour, IPointer
    {
        [SerializeField] private Camera uiCamera;
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private List<SerializablePair<EPointerType, GameObject>> pointerIcons;

        private float _offsetDistance;
        private Vector3 _targetPosition;
        private Transform _icon;
        private Transform _characterTransform;
        private GameObject _pointerGameObject;
        private RectTransform _rectTransform;

        public void SetPoint(Transform from, Vector3 targetPosition, float offsetDistance = 3)
        {
            if (Vector3.Distance(from.position, targetPosition) < offsetDistance)
            {
                _pointerGameObject.SetActive(false);
                return;
            }

            IsActive = true;
            _icon = transform.GetChild(0).GetChild(0);
            _characterTransform = from;
            _offsetDistance = offsetDistance;
            _targetPosition = targetPosition;
            _targetPosition.y = 0;
        }

        public void SetPoint(Transform from, Vector3 targetPosition, EPointerType pointerType, float offsetDistance = 5)
        {
            _pointerGameObject.SetActive(false);

            IsActive = true;
            _characterTransform = from;
            _offsetDistance = offsetDistance;
            _targetPosition = targetPosition;
            _targetPosition.y = 0;

            pointerIcons.ForEach(x => x.Value.SetActive(x.Key == pointerType));
            _icon = transform.GetChild(0).GetComponentsInChildren<Transform>()[1];
        }

        public void Disable()
        {
            IsActive = false;
            _pointerGameObject.SetActive(false);
        }

        private static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            var n = Mathf.Atan2(-dir.x, dir.z) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        private void Update()
        {
            if (!IsActive) return;
            var from = _characterTransform.position;
            var dir = _targetPosition - from;
            dir.y = 0;
            var angle = GetAngleFromVectorFloat(dir);
            pointerRect.localEulerAngles = new Vector3(0, 0, angle);

            var isOffScreen = dir.magnitude > _offsetDistance;
            switch (isOffScreen)
            {
                case true when !_pointerGameObject.activeSelf:
                    _pointerGameObject.SetActive(true);
                    break;
                case false when _pointerGameObject.activeSelf:
                    _pointerGameObject.SetActive(false);
                    return;
            }

            if (!isOffScreen) return;

            var position = Quaternion.Euler(pointerRect.localEulerAngles) * Vector3.up * 5000;
            if (position.x <= 100) position.x = 100;
            if (position.y <= 100) position.y = 100;
            if (position.x >= Screen.width - 100) position.x = Screen.width - 100;
            if (position.y >= Screen.height - 100) position.y = Screen.height - 100;

            pointerRect.position = uiCamera.ScreenToWorldPoint(position);
            _icon.eulerAngles = Vector3.zero;
            var localPosition = pointerRect.localPosition;
            localPosition.z = 0;

            localPosition = new Vector2(-Mathf.Sin(angle * Mathf.Deg2Rad),
                Mathf.Cos(angle * Mathf.Deg2Rad)) * 300;
            pointerRect.localPosition = localPosition;
        }

        private void Awake()
        {
            _pointerGameObject = pointerRect.gameObject;
        }

        public bool IsActive { get; private set; }
    }
}