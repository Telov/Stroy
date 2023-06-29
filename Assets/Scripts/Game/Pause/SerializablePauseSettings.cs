using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Game.Pause
{
    [Serializable]
    public class SerializablePauseSettings
    {
        [field: SerializeField]
        public Button PauseButton
        {
            get;
            private set;
        } 
        
        [field: SerializeField]
        public GameObject PauseUI
        {
            get;
            private set;
        }
        
        [field: SerializeField]
        public GameObject GameUI
        {
            get;
            private set;
        }  
        
        [field: SerializeField]
        public Button ResumeButton
        {
            get;
            private set;
        }   
        
        [field: SerializeField]
        public Button RestartButton
        {
            get;
            private set;
        }  
        
        [field: SerializeField]
        public TextMeshProUGUI SoundText
        {
            get;
            private set;
        }  
        
        [field: SerializeField]
        public Button SoundButton
        {
            get;
            private set;
        }
        
        [field: SerializeField]
        public Sprite PauseOn
        {
            get;
            private set;
        }   
        
        [field: SerializeField]
        public Sprite PauseOff
        {
            get;
            private set;
        }    
        
        [field: SerializeField]
        public Button MenuButton
        {
            get;
            private set;
        }
    }
}
