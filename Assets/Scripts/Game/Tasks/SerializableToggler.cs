using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Game.Tasks
{
    [Serializable]
    public class SerializableToggler
    {
        [field: SerializeField] public TextMeshProUGUI TMPro { get; private set; }
        
        [field: SerializeField] public Image Image { get; private set; }
        
        [field: SerializeField] public Toggle Toggler { get; private set; }
    }
}