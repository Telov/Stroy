using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;

namespace General.Settings
{
    [Serializable]
    public class SerializableAudioSettings : IInitializable
    {
        [SerializeField] private List<SerializablePair<string, AudioClip>> audioStorage;

        public void Initialize()
        {
            if (AudioStorage.Count > 0) return;
            foreach (var audio in audioStorage)
            {
                AudioStorage.Add(audio.Key, audio.Value);
            }
        }

        public Dictionary<string, AudioClip> AudioStorage { get; private set; } = new();

        [field: SerializeField]
        public AudioSource AudioSourcePrefab
        {
            get;
            private set;
        }
    }
}