using Game.Player;

namespace Game.Vehicle.Interfaces
{
    public interface IControllable
    {
        void TakeControl(PlayerController playerController);

        float Radius
        {
            get;
        }

        bool Usable
        {
            get;
            set;
        }
    }
}
