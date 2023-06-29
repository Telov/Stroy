using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.QuestPointer
{
    public class PointerManager : IPointerManager, IDisposable
    {
        public PointerManager(SerializablePointerSettings pointerSettings)
        {
            _parentTransform = pointerSettings.ParentTransform;
            _monoPointerInstance = pointerSettings.PointerInstance;
        }

        private readonly List<MonoPointer> _monoPointers = new();
        private readonly MonoPointer _monoPointerInstance;
        private readonly Transform _parentTransform;

        private List<Transform> _transforms;
        private Transform _transform;
        private bool _isActive;
        private bool _iDispose;

        public void SetPointers(Transform transform, List<Transform> transforms, float offsetDistance = 3)
        {
            if (_iDispose) return;
            
            _transform = transform;
            _transforms = transforms;

            var upperFloor = Mathf.Clamp(transforms.Count, 0, 3);
            if (transforms.Count > _monoPointers.Count)
            {
                while (_monoPointers.Count < upperFloor)
                {
                    var instance = Object.Instantiate(_monoPointerInstance, _parentTransform);
                    _monoPointers.Add(instance);
                }
            }

            var list = transforms.OrderBy(x => Vector3.Distance(transform.position, x.position)).Take(5).ToList();

            var i = 0;
            if (_monoPointers.Count(pointer => pointer.IsActive) != upperFloor || transforms.Count > 3)
            {
                foreach (var instance in _monoPointers)
                {
                    if (i < upperFloor)
                    {
                        instance.SetPoint(transform, list[i].position, offsetDistance);
                    }
                    else
                    {
                        instance.Disable();
                    }

                    i += 1;
                }
            }

            if (upperFloor == 0)
            {
                _isActive = false;
            }

            if (upperFloor > 0 && !_isActive)
            {
                _isActive = true;
                Update(offsetDistance);
            }
        }

        public void Disable()
        {
            _isActive = false;

            foreach (var instance in _monoPointers)
            {
                instance.Disable();
            }
        }

        private async void Update(float offsetDistance = 3)
        {
            while (_isActive)
            {
                SetPointers(_transform, _transforms, offsetDistance);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public IPointer SpawnPointer()
        {
            return Object.Instantiate(_monoPointerInstance, _parentTransform);
        }

        public void Dispose()
        {
            _iDispose = true;
        }
    }
}