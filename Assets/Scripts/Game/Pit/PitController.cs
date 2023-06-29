using UnityEngine;
using DG.Tweening;
using System;
using Cinemachine;
using Game.Cameras.Interfaces;
using Zenject;

namespace Game.Pit
{
    public class PitController : MonoBehaviour
    {
        public bool isClear;

        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private Transform cementQuad;

        private ICameraService _cameraService;

        [Inject]
        private void Construct(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }
        
        public void FillCement(Action callback)
        {
            _cameraService.ChangeView(virtualCamera);
            cementQuad.DOLocalMoveY(1, 5).OnComplete(() =>
            {
                boxCollider.enabled = false;
                isClear = false;
                callback.Invoke();
            });
        }
    }
}