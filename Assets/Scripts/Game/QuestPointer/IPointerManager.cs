using System.Collections.Generic;
using UnityEngine;

namespace Game.QuestPointer
{
    public interface IPointerManager
    {
        void SetPointers(Transform transform, List<Transform> transforms, float offsetDistance = 3);

        void Disable();

        IPointer SpawnPointer();
    }
}