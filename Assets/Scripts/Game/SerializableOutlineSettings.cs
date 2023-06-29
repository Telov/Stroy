using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class SerializableOutlineSettings
    {
        [field: SerializeField] public Gradient Gradient { get; private set; }
        [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; }
    }
}