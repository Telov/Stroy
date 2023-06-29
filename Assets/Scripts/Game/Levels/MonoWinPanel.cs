using Game.Levels.Interfaces;
using UnityEngine;
using DG.Tweening;
using System;

namespace Game.Levels
{
    public class MonoWinPanel : MonoBehaviour, IWinService
    {
        public event Action OnClick = () => { };

        [SerializeField] private GameObject gameUi;
        [SerializeField] private GameObject confetti;
        [SerializeField] private GameObject[] skins;

        private bool _isStarted;
        private bool _isUsed;

        public void ClickHandler()
        {
            if (!_isUsed)
            {
                _isUsed = true;
                OnClick.Invoke();
            }
        }

        public void SetActive()
        {
            SelectSkin();

            if (_isStarted) return;

            _isStarted = true;
            gameObject.SetActive(true);
            gameUi.SetActive(false);
            confetti.SetActive(true);
            transform.DOScale(Vector3.zero, 1).OnComplete(() => transform.DOScale(Vector3.one, 0.2f));
        }

        private void SelectSkin()
        {
            var id = PlayerPrefs.GetInt("SkinID", 0);
            skins[id].SetActive(true);
        }
    }
}