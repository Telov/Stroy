using System;
using UnityEngine;

namespace Game.Vehicle
{
    [Serializable]
    public struct Wheel
    {
        public Wheel(Transform wheel)
        {
            Circumference = 2 * Mathf.PI / 4;
            WheelTransform = wheel;
        }
        
        public Transform WheelTransform { get; }
        public float Circumference { get; }
    }
}