using UnityEngine.SceneManagement;
using Game.Pause.Interfaces;
using UnityEngine.UI;
using System;
using Game.Levels;
using Game.Levels.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Pause
{
    public class PauseService : IPauseService
    {
        public PauseService(SerializablePauseSettings settings)
        {
            var soundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("Volume", 1));

            settings.SoundButton.onClick.AddListener(() =>
            {
                soundEnabled = !soundEnabled;
            });
            settings.MenuButton.onClick.AddListener(() => SceneManager.LoadScene(1));
            settings.ResumeButton.onClick.AddListener(() =>
            {
                OnGamePaused.Invoke(false);
                settings.GameUI.SetActive(true);
                settings.PauseUI.SetActive(false);
            });
            settings.RestartButton.onClick.AddListener(() => { SceneManager.LoadSceneAsync(2); });
            settings.PauseButton.onClick.AddListener(() =>
            {
                _isPaused = !_isPaused;
                OnGamePaused.Invoke(_isPaused);
                settings.PauseUI.SetActive(_isPaused);
                settings.GameUI.SetActive(!_isPaused);
                settings.PauseButton.image.sprite = _isPaused ? settings.PauseOn : settings.PauseOff;
            });
        }
                
        public event Action<bool> OnGamePaused = _ => { };

        private bool _isPaused;
    }
}