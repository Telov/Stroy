using DG.Tweening;
using Game.Controls.Interfaces;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace Game.Controls
{
    public class ControlsInput : IControlsInput
    {
        public ControlsInput(SerializableControlsSettings controlsSettings)
        {
            _interactButton = controlsSettings.InteractButton;
            _joystick = controlsSettings.Joystick;
        }
        
        private readonly Button _interactButton;
        private readonly Joystick _joystick;

        public void SetButton(UnityAction action)
        {
            _interactButton.onClick.RemoveAllListeners();
            _interactButton.onClick.AddListener(action);
            _interactButton.onClick.AddListener(StopAnimation);
            _interactButton.gameObject.SetActive(true);
            _interactButton.transform.localScale = Vector3.one;
            
            void StopAnimation() => DOTween.Kill(_interactButton.transform);
        }

        public void HideButton()
        {
            _interactButton.onClick.RemoveAllListeners();
            _interactButton.gameObject.SetActive(false);
        }

        public void PlayAnimation()
        {
            _interactButton.transform.DOScale(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        public Vector2 Direction => _joystick.Direction;
    }
}