using Game.Vehicle.Enum;
using UnityEngine;

namespace Game.QuestPointer
{
    public interface IPointer
    {
        void SetPoint(Transform from, Vector3 targetPosition, EPointerType pointerType, float offsetDistance = 5);
        void SetPoint(Transform from, Vector3 targetPosition, float distance);
        void Disable();
    }
}