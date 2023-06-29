using System;

namespace Game.Tasks.Interfaces
{
    public interface ITaskTracker
    {
        event Action OnComplete;
        void IncreaseValue();
        void SetActive(int semiProgress);
        void SetActive(int semiProgress, int unavailable);
        void DisableTracker();
    }
}
