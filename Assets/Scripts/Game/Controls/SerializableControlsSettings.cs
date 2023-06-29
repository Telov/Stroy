using UnityEngine.UI;
using UnityEngine;
using System;

namespace Game.Controls
{
    [Serializable]
    public class SerializableControlsSettings
    {
        [field: SerializeField]
        public Joystick Joystick
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Button InteractButton
        {
            get;
            private set;
        }
    }
}
