using TMPro;
using UnityEngine;

namespace Game.Tasks
{
    public class TutorialService : ITutorialService
    {
        public TutorialService(SerializableTutorialSettings tutorialSettings)
        {
            _upperText = tutorialSettings.UpperText;
            _lowerText = tutorialSettings.LowerText;
        }

        private readonly TextMeshProUGUI _upperText;
        private readonly TextMeshProUGUI _lowerText;

        public void OpenUp(string value)
        {
            _upperText.enabled = true;
            _upperText.text = value;
        }

        public void OpenDown(string value)
        {
            _lowerText.enabled = true;
            _lowerText.text = value;
        }
        
        public void Close()
        {
            _upperText.enabled = false;
            _lowerText.enabled = false;
        }

        public void CloseDown()
        {
            _lowerText.enabled = false;
        }
    }
}