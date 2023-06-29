using System;
using Menu.Loading.Interfaces;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using TMPro;

namespace Menu.Buttons
{
    public class MonoMenuService : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button levelsMenu;
        [SerializeField] private GameObject levelsUI;
        [SerializeField] private Button[] levelsButton;

        private ILoadingScreen _loadingScreen;

        [Inject]
        private void Construct(ILoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public void LoadLevel(int id)
        {
            PlayerPrefs.SetInt("LevelID", id);
            _loadingScreen.LoadScene(2);
            levelsUI.SetActive(false);
            gameObject.SetActive(false);
        }

        private void Start()
        {
            levelText.text = PlayerPrefs.GetInt("FictionID", 1) + " Level";
            for (var i = 0; i < 5; i++)
            {
                var id = i;
                levelsButton[id].gameObject.SetActive(true);
                levelsButton[id].onClick.AddListener(() => LoadLevel(id));
            }

            startButton.onClick.AddListener(() =>
            {
                _loadingScreen.LoadScene(2);
                gameObject.SetActive(false);
            });

            levelsMenu.onClick.AddListener(() =>
            {
                levelsUI.SetActive(true);
                gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                levelsUI.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}