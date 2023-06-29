using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class OutlineManager : IOutlineManager, IDisposable
    {
        public OutlineManager(SerializableOutlineSettings outlineSettings)
        {
            _animationCurve = outlineSettings.AnimationCurve;
            _gradient = outlineSettings.Gradient;
        }

        private bool _isShine;
        private float _currentTime, _totalTime;
        private List<Outline> _outlines;
        
        private readonly Gradient _gradient;
        private readonly AnimationCurve _animationCurve;
    
        public void SetShines(List<Outline> outlines)
        {
            _isShine = true;
            _currentTime = 0;
            _outlines = outlines;
            _totalTime = _animationCurve.keys[^1].time;
            Shine();
        }

        public void Disable()
        {
            _isShine = false;
            _outlines = new List<Outline>();
        }

        private async void Shine()
        {
            while (_isShine)
            {
                var width = _animationCurve.Evaluate(_currentTime);
                var color = _gradient.Evaluate(_currentTime);
                _outlines.ForEach(outline =>
                {
                    outline.OutlineWidth = width;
                    outline.OutlineColor = color;
                });
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                _currentTime += 0.1f;
                if (_currentTime > _totalTime)
                {
                    _currentTime = 0;
                }
            }
        }

        public void Dispose()
        {
            _isShine = false;
        }
    }
}