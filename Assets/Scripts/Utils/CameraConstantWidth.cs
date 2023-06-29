using Cinemachine;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraConstantWidth : MonoBehaviour
    {
        public Vector2Int defaultResolution = new (720, 1280);
    
        private CinemachineVirtualCamera _camera;
        private float _initialSize;
        private float _targetAspect;
        private float _initialFov;
        private float _horizontalFov;
        
        private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            var hFovInRads = hFovInDeg * Mathf.Deg2Rad;

            var vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
        
        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();

            _targetAspect = (float) defaultResolution.x / defaultResolution.y;

            _initialFov = _camera.m_Lens.FieldOfView;
            _horizontalFov = CalcVerticalFov(_initialFov, 1f / _targetAspect);

            _camera.m_Lens.FieldOfView = CalcVerticalFov(_horizontalFov, _camera.m_Lens.Aspect);
        }
    }
}