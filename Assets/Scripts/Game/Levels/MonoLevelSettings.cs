using System.Collections.Generic;
using Game.Tasks.Interfaces;
using UnityEngine;
using Game.Tasks;

namespace Game.Levels
{
    public class MonoLevelSettings : MonoBehaviour
    {
        [SerializeField, NonReorderable] private List<SerializableTask> tasks;

        private ITaskUI _taskUI;

        public void Initialize(ITaskUI taskUI)
        {
            taskUI.Initialize(tasks);
        }
        
        [field: SerializeField, Space(50)]
        public Vector3 SpawnPosition
        {
            get;
            private set;
        }
    }
}
