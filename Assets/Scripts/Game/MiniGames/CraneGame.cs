using System;
using DG.Tweening;
using Game.MiniGames.Interfaces;
using Game.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.MiniGames
{
    public class CraneGame : MonoBehaviour, IPointerClickHandler, ICraneGame
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Slider dynamometer;

        private event Action<float, float> OnComplete = (x,y) => {};
        private bool _flag;
        private bool _isTutorial;
        private int _clickCount;
        private ITutorialService _tutorialService;
        private Sequence _sequence;

        public void OnPointerClick(PointerEventData eventData)
        {
            DOTween.Kill(rectTransform);
            DOTween.Kill(dynamometer);

            switch (_clickCount)
            {
                case 0:
                    if(_isTutorial) _tutorialService.OpenDown("Click to aim"); 
                    dynamometer.gameObject.SetActive(false);
                    rectTransform.gameObject.SetActive(true);
                    rectTransform.localPosition = new Vector3(-400, 0, 0);
                    rectTransform.DOLocalMoveX(400, 1.2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case 1:
                    if(_isTutorial) _tutorialService.OpenDown("Click to release"); 
                    rectTransform.gameObject.SetActive(false); 
                    dynamometer.gameObject.SetActive(true);
                    dynamometer.value = 0;
                    dynamometer.DOValue(1, 1.2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case 2:
                    var offset = Mathf.Abs(0.5f - dynamometer.value);
                    OnComplete.Invoke(rectTransform.localPosition.x + 400, 1 - offset);
                    break;
            }

            _clickCount += 1;
        }

        public void StopGame()
        {
            gameObject.SetActive(false);
            _flag = true;
        }

        public void StartGame(Action<float, float> callback, ITutorialService tutorialService)
        {
            if (_flag) return;
            _isTutorial = true;
            _clickCount = 0;
            gameObject.SetActive(true);
            _tutorialService = tutorialService;
            OnPointerClick(null);
            OnComplete = callback;
        }

        public void StartGame(Action<float, float> callback)
        {
            if (_flag) return;
            _clickCount = 0;
            gameObject.SetActive(true);
            OnPointerClick(null);
            OnComplete = callback;
        }
    }
}