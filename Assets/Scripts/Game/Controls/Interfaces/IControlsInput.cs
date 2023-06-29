using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Controls.Interfaces
{
    public interface IControlsInput
    {
        Vector2 Direction
        {
            get;
        }
        
        void SetButton(UnityAction action);
        void HideButton();

        void PlayAnimation();
    }
}