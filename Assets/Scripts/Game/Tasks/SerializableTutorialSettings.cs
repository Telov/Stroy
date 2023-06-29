using System;
using TMPro;
using UnityEngine;

namespace Game.Tasks
{
    [Serializable]
    public class SerializableTutorialSettings
    {
        [field: SerializeField] public TextMeshProUGUI UpperText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI LowerText { get; private set; }
    }
}