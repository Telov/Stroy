using System;

namespace Game.Levels.Interfaces
{
    public interface IWinService
    {
        event Action OnClick;
        void SetActive();
    }
}
