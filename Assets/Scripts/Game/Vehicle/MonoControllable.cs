using Game.Vehicle.Enum;
using UnityEngine;

namespace Game.Vehicle
{
    public abstract class MonoControllable : MonoBehaviour
    {
        public abstract EPointerType GetName { get; }
        
        public abstract bool IsTaskDone { get; }
    }
}
