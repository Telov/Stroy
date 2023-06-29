using System;

namespace Game.Pause.Interfaces
{
    public interface IPauseService
    {
        event Action<bool> OnGamePaused;
    }
}
