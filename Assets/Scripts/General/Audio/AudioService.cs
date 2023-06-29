using System.Collections.Generic;
using General.Audio.Interfaces;
using General.Settings;
using UnityEngine;

namespace General.Audio
{
    public class AudioService : IAudioService
    {
        public AudioService(SerializableAudioSettings audioSettings)
        {
            _audioSource = audioSettings.AudioSourcePrefab;
            _audioStorage = audioSettings.AudioStorage;
        }

        private readonly Dictionary<string, AudioClip> _audioStorage;
        private readonly AudioSource _audioSource;
        
        public void PlayOneShot(string clip)
        {
            _audioSource.PlayOneShot(_audioStorage[clip]);
        }
    }
}
