using System;
using UnityEngine;

namespace Game.Vehicle
{
    public class FillingRing : MonoBehaviour, IServiceInteraction
    {
        public event Action OnComplete = () => { };
        
        [SerializeField] private float angle = 1;
        [SerializeField] private TrailRenderer trailRenderer;
        
        private float _angleSum;
        private bool _isRotating;
        private Vector3 _center;
        private Transform _transform;
        private Vector3 _localPosition;
        
        public void StartInteract(Action callback)
        {
            OnComplete = callback;
            trailRenderer.Clear();
            _transform.parent = null;
            _center = transform.position;
            trailRenderer.emitting = true;
            _isRotating = true;
        }

        public void InterruptInteraction()
        {
            OnComplete = null;
            
            _angleSum = 0;
            _isRotating = false;
            _transform.SetParent(transform, true);
            _transform.localPosition = _localPosition;
            trailRenderer.Clear();
            trailRenderer.emitting = false;
        }

        private void FixedUpdate()
        {
            if (_isRotating)
            {
                _angleSum += angle;
                _transform.RotateAround(_center, Vector3.up, angle);
            }

            if (!(_angleSum > 359)) return;
            _angleSum = 0;
            _isRotating = false;
            trailRenderer.Clear();
            OnComplete.Invoke();
        }

        private void Start()
        {
            _transform = trailRenderer.transform;
            _transform.SetParent(transform, true);
            _localPosition = _transform.localPosition;
        }
    }
}
