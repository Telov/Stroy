using General.Settings;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace General.Audio
{
    public class AudioToggler : IDisposable
    {
        public AudioToggler(SerializableTogglerSettings togglerSettings)
        {
            _isOn = togglerSettings.IsOn;
            _isOff = togglerSettings.IsOff;
            _audioListener = togglerSettings.AudioListener;
            _togglerButton = togglerSettings.TogglerButton;
            
            _audioEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("Volume", 1));
            _togglerButton.onClick.AddListener(ChangeState);
            _audioListener.enabled = _audioEnabled;
        }

        private bool _audioEnabled;
        private readonly Button _togglerButton;
        private readonly Sprite _isOn;
        private readonly Sprite _isOff;
        private readonly AudioListener _audioListener;
        
        private void ChangeState()
        {
            _audioEnabled = !_audioEnabled;
            _togglerButton.image.sprite = _audioEnabled ? _isOn : _isOff;
            var convertedValue = Convert.ToInt32(_audioEnabled);
            PlayerPrefs.SetInt("Volume", convertedValue);
            _audioListener.enabled = _audioEnabled;
        }

        public void Dispose()
        {
            _togglerButton.onClick.RemoveAllListeners();
        }

    }
}
