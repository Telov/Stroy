using System;
using System.Collections.Generic;

namespace Game.Tasks.Interfaces
{
    public interface ITaskUI
    {
        event Action OnMissionComplete;
        void Initialize(List<SerializableTask> taskList);
    }
}
