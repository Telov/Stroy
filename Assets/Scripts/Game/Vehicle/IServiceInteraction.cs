using UnityEngine;
using System;

namespace Game
{
    public interface IServiceInteraction
    {
        event Action OnComplete;

        void StartInteract(Action callback);
        void InterruptInteraction();
    }
}