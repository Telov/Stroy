using System.Collections.Generic;
using Game.Particles.Enum;
using System.Collections;
using Game.Particles;
using Game.Garbage;
using UnityEngine;
using Zenject;
using System;
using DG.Tweening;
using TMPro;

namespace Game.Vehicle.Lorry
{
    public class LorryBoot : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private GameObject[] cabinPrefabs;
        [SerializeField] private GameObject[] garbagePrefabsFour;
        [SerializeField] private GameObject[] garbagePrefabsEight;
        [SerializeField] private GameObject[] garbagePrefabsTwelve;

        private int _maxCount;
        private int _currentCount;
        private int _activeCount;
        private Transform _textTransform;
        private readonly List<MonoGarbage> _boot = new();
        private Transform _targetTransform;
        private Camera _cameraMain;
        private IMemoryPool<MonoGarbage> _memoryPool;
        private IMemoryPool<MonoParticles> _particlesPool;

        [Inject]
        private void Construct(MemoryPool<MonoParticles> particles, MemoryPool<MonoGarbage> garbageMemoryPool)
        {
            _memoryPool = garbageMemoryPool;
            _particlesPool = particles;
        }

        public void SetActive(bool value)
        {
            if (value)
            {
                UpdateText();
            }

            textMesh.gameObject.SetActive(value);
        }

        public void Increase(MonoGarbage instance)
        {
            instance.transform.parent = transform;
            _boot.Add(instance);
            _currentCount += 1;
        }

        public void VisualIncrease()
        {
            _activeCount += 1;
            GameObject[] garbages;
            switch (_maxCount)
            {
                case 4:
                {
                    garbages = garbagePrefabsFour;
                    break;
                }
                case 8:
                {
                    garbages = garbagePrefabsEight;
                    break;
                }
                case 12:
                {
                    garbages = garbagePrefabsTwelve;
                    break;
                }
                default:
                    return;
            }
            if (_currentCount != _activeCount)
            {
                for (var id = 0; id < _currentCount; id++)
                {
                    garbages[id].SetActive(true);
                }

                _activeCount = _currentCount;
            }

            garbages[_currentCount - 1].SetActive(true);
            IsLoading = false;
        }

        private void Decrease()
        {
            GameObject[] garbages;
            switch (_maxCount)
            {
                case 4:
                {
                    garbages = garbagePrefabsFour;
                    break;
                }
                case 8:
                {
                    garbages = garbagePrefabsEight;
                    break;
                }
                case 12:
                {
                    garbages = garbagePrefabsTwelve;
                    break;
                }
                default:
                    return;
            }
            _currentCount -= 1;
            _activeCount -= 1;
            garbages[_currentCount].SetActive(false);

            UpdateText();
        }

        private void Despawn(MonoGarbage instance)
        {
            instance.gameObject.SetActive(false);
            _memoryPool.Despawn(instance);
        }

        public void Unload(Transform targetTransform, Action callback)
        {
            if (_currentCount <= 0) return;
            IsUnloading = true;
            _targetTransform = targetTransform;
            StartCoroutine(nameof(UnloadCoroutine), callback);
        }

        private IEnumerator UnloadCoroutine(Action callback)
        {
            for (var i = _boot.Count - 1; i >= 0; i--)
            {
                _boot[i].transform.parent = null;
                _boot[i].PlayGrabAnimation(_targetTransform, Despawn);

                var particleInstance = _particlesPool.Spawn();
                particleInstance.SetParticle(EParticles.StarPoof);
                particleInstance.SetActive(true);
                particleInstance.transform.position = _boot[i].transform.position;
                particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));
                _boot.RemoveAt(i);

                yield return new WaitForSeconds(0.2f);
                _targetTransform.DOPunchScale(Vector3.up, 0.17f, 5).SetEase(Ease.Flash);
                callback.Invoke();
                Decrease();
            }

            IsUnloading = false;
        }

        public void UpdateText()
        {
            textMesh.text = $"{_currentCount} / {MaxCount}";
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                MaxCount += 4;
                transform.parent.localScale = Vector3.one * 2 + Vector3.one * (MaxCount / 12f);
                var particleInstance = _particlesPool.Spawn();
                particleInstance.SetParticle(EParticles.WhitePoof);
                particleInstance.SetActive(true);
                particleInstance.transform.position = transform.parent.position;
                particleInstance.transform.position += Vector3.up * 2;
                particleInstance.transform.localScale = transform.parent.localScale;
                particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));
            }
        }

        private void LateUpdate()
        {
            _textTransform.LookAt(_cameraMain.transform.position);
        }

        private void Awake()
        {
            _cameraMain = Camera.main;
            _textTransform = textMesh.transform;
        }

        public int MaxCount
        {
            get => _maxCount;
            set
            {
                _maxCount = value;
                _maxCount = Mathf.Min(_maxCount, 12);
                _currentCount = 0;
                _boot.Clear();
                switch (_maxCount)
                {
                    case 4:
                    {
                        cabinPrefabs[0].SetActive(true);
                        break;
                    }
                    case 8:
                    {
                        foreach (var garbage in garbagePrefabsFour)
                        {
                            garbage.SetActive(false);
                        }
                        cabinPrefabs[0].SetActive(false);
                        cabinPrefabs[1].SetActive(true);
                        break;
                    }
                    case 12:
                    {
                        foreach (var garbage in garbagePrefabsEight)
                        {
                            garbage.SetActive(false);
                        }
                        cabinPrefabs[1].SetActive(false);
                        cabinPrefabs[2].SetActive(true);
                        break;
                    }
                }
                UpdateText();
            }
        }
        
        public bool IsLoading { get; set; }

        public bool IsUnloading { get; set; }

        public bool IsAble => MaxCount > _currentCount;

        public int Count => _currentCount;
    }
}