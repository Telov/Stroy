using UnityEngine.UI;
using UnityEngine;
using System;
using Zenject;

namespace Game.Tasks
{
    public class TaskTracker
    {
        public TaskTracker(Slider greenSlider)
        {
            _greenSlider = greenSlider;
            _slider = _greenSlider.transform.parent.gameObject;
            _orangeSlider = greenSlider.transform.GetChild(0).GetComponent<Image>();
        }

        public event Action OnComplete = () => { };
        public event Action OnIncrease = () => { };

        private readonly GameObject _slider;
        private readonly Image _orangeSlider;
        private readonly Slider _greenSlider;

        private int _currentCount;
        private int _maxCount;

        public void SetActive()
        {
            Initialize();
            _slider.SetActive(true);
            _orangeSlider.fillAmount = 1;
        }

        public void SetActive(int availableCount)
        {
            Initialize();
            var totalCount = availableCount + _currentCount;
            var availablePart = totalCount / (float)_maxCount;
            _orangeSlider.fillAmount = availablePart;

            _slider.SetActive(true);
            _greenSlider.value = _currentCount;
        }

        public void SetActive(int availableCount, int semiCount)
        {
            Initialize();
            var totalCount = availableCount + semiCount + _currentCount;
            var availablePart = totalCount / (float)_maxCount;
            _orangeSlider.fillAmount = availablePart;

            _slider.SetActive(true);
            _greenSlider.value = _currentCount;
        }

        public void SetTask(int targetCount)
        {
            _maxCount = targetCount;
            _greenSlider.maxValue = targetCount;
        }

        public void IncreaseValue()
        {
            _currentCount += 1;
            _greenSlider.value = _currentCount;
            OnIncrease.Invoke();
            if (Math.Abs(_greenSlider.value - _maxCount) < Mathf.Epsilon)
            {
                OnComplete.Invoke();
                IsTaskDone = true;
            }
        }

        private void Initialize()
        {
            _greenSlider.maxValue = _maxCount;
            _greenSlider.value = _currentCount;
        }

        public int CurrentCount => _currentCount;
        public int MaxValue => _maxCount;

        public void DisableTracker()
        {
            _slider.SetActive(false);
        }

        public bool IsTaskDone { get; private set; } = false;

        public class Factory : PlaceholderFactory<TaskTracker>
        {
        }
    }
}