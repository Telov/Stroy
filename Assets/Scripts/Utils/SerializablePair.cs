using UnityEngine;
using System;

namespace Utils
{
    [Serializable]
    public class SerializablePair<TKey,TValue>
    {
        [field: SerializeField]
        public TKey Key
        {
            get;
            private set;
        }

        [field: SerializeField]
        public TValue Value
        {
            get;
            private set;
        }
    }
}