using Cinemachine;

namespace Game.Cameras.Interfaces
{
    public interface ICameraService
    {
        void ChangeView(CinemachineVirtualCamera virtualCamera);

        void PlayerFocus();

        CinemachineVirtualCamera PlayerCamera
        {
            get;
        }
    }
}
