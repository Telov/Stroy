using Cinemachine;
using Game.Cameras.Interfaces;

namespace Game.Cameras
{
    public class CameraService : ICameraService
    {
        public CameraService(CinemachineVirtualCamera playerCamera)
        {
            PlayerCamera = playerCamera;
            _activeCamera = PlayerCamera;
        }

        private CinemachineVirtualCamera _activeCamera;

        public void ChangeView(CinemachineVirtualCamera virtualCamera)
        {
            _activeCamera.gameObject.SetActive(false);
            virtualCamera.gameObject.SetActive(true);
            _activeCamera = virtualCamera;
        }

        public void PlayerFocus()
        {
            _activeCamera.gameObject.SetActive(false);
            PlayerCamera.gameObject.SetActive(true);
            _activeCamera = PlayerCamera;
        }

        public CinemachineVirtualCamera PlayerCamera { get; }
    }
}