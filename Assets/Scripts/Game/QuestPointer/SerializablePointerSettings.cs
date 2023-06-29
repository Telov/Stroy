using System;
using UnityEngine;

namespace Game.QuestPointer
{
    [Serializable]
    public class SerializablePointerSettings
    {
        [field: SerializeField]
        public Transform ParentTransform
        {
            get;
            private set;
        }

        [field: SerializeField]
        public MonoPointer PointerInstance
        {
            get;
            private set;
        }
    }
}
