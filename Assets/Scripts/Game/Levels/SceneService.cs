using Game.Levels.Interfaces;
using Game.Player;
using Game.Tasks.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

namespace Game.Levels
{
    public class SceneService : ISceneService
    {
        public SceneService(SerializableLevels levels, ITaskUI taskUI, IWinService winService,
            PlayerController playerController)
        {
            _levels = levels;
            var id = CurrentLevel;
            var instance = levels.Levels[id];
            instance.gameObject.SetActive(true);
            instance.Initialize(taskUI);
            taskUI.OnMissionComplete += winService.SetActive;
            var buttons = levels.LevelButtons;
            for (int i = 0; i < buttons.Length; i++)
            {
                var index = i;
                buttons[i].onClick.AddListener(() => LoadLevel(index));
            }

            _fictionId = PlayerPrefs.GetInt("FictionID", 1);

            MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Start, _fictionId);
            winService.OnClick += () =>
            {
                MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Complete, _fictionId);
                _fictionId += 1;
                PlayerPrefs.SetInt("FictionID", _fictionId);
                LoadLevel();
            };
            playerController.NavMeshAgent.enabled = false;
            SetPosition(playerController, instance.SpawnPosition);
        }

        private readonly SerializableLevels _levels;
        private int _fictionId;

        private async void SetPosition(PlayerController player, Vector3 position)
        {
            await Task.Delay(100);
            player.transform.position = position;
            player.NavMeshAgent.enabled = true;
        }

        public void LoadLevel()
        {
            int id;
            if (_fictionId > 10)
            {
                id = CurrentLevel + Random.Range(1, 3);
            }
            else
            {
                id = CurrentLevel + 1;
            }

            var count = _levels.Levels.Count;
            if (id > count - 1)
            {
                id %= count;
            }

            id = PlayerPrefs.HasKey("TutorialComplete") ? Mathf.Clamp(id, 5, count - 1) : Mathf.Clamp(id, 0, 4);

            PlayerPrefs.SetInt("LevelID", id);
            SceneManager.LoadSceneAsync(2);
        }

        private void LoadLevel(int i)
        {
            PlayerPrefs.SetInt("LevelID", i);
            SceneManager.LoadSceneAsync(2);
        }

        public int CurrentLevel => PlayerPrefs.GetInt("LevelID", 0);
    }
}