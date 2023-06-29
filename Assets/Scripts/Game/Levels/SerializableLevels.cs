using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels
{
    [Serializable]
    public class SerializableLevels
    {
        [field: SerializeField]
        public List<MonoLevelSettings> Levels
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Button[] LevelButtons
        {
            get;
            private set;
        }
    }
}
