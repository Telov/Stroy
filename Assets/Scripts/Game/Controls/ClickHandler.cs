using System;
using Game.Controls.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Controls
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler, IClickHandler
    {
        public event Action<RaycastHit> OnGetHit = _ => { };
        
        private LayerMask _layerMask;
        private Camera _mainCamera;

        public void OnPointerClick(PointerEventData eventData)
        {
            GetPoint(eventData.position);
        }

        private void GetPoint(Vector2 position)
        {
            var ray = _mainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, _layerMask))
            {
                OnGetHit.Invoke(raycastHit);
            }
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetLayerMask(int layerMask)
        {
            _layerMask = layerMask;
        }
    }
}