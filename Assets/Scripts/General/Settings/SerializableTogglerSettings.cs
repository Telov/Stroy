using UnityEngine.UI;
using UnityEngine;
using System;

namespace General.Settings
{
    [Serializable]
    public class SerializableTogglerSettings
    {
        [field: SerializeField]
        public AudioListener AudioListener
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Button TogglerButton
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Sprite IsOn
        {
            get;
            private set;
        }
        
        [field: SerializeField]
        public Sprite IsOff
        {
            get;
            private set;
        }
    }
}
