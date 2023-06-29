using UnityEngine.SceneManagement;
using Menu.Loading.Interfaces;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Menu.Loading
{
    public class LoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private Slider progressBar;
        
        public void LoadScene(int id)
        {
            gameObject.SetActive(true);
            StartCoroutine(nameof(LoadAsync), id);
        }

        private IEnumerator LoadAsync(int id)
        {
            var operation = SceneManager.LoadSceneAsync(id);

            while (!operation.isDone)
            {
                var progress = Mathf.Clamp01(operation.progress / .9f);
                progressBar.value = progress;
                yield return null;
            }
        }
    }
}
