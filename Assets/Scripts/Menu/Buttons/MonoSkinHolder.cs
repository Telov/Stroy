using UnityEngine;
using UnityEngine.UI;

namespace Menu.Buttons
{
    public class MonoSkinHolder : MonoBehaviour
    {
        [SerializeField] private GameObject[] skins;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;

        private int _lenght;
        private int _index;

        private void ChangeSkin(int addendum)
        {
            skins[_index].SetActive(false);
            
            _index += addendum;
            if (_index < 0) _index = _lenght - 1;
            if (_index > _lenght - 1) _index = 0;
            
            skins[_index].SetActive(true);
            PlayerPrefs.SetInt("SkinID", _index);
        }

        private void Start()
        {
            _index = PlayerPrefs.GetInt("SkinID");
            
            rightButton.onClick.AddListener(() => ChangeSkin(1));
            leftButton.onClick.AddListener(() => ChangeSkin(-1));

            _lenght = skins.Length;
            ChangeSkin(_index);
        }
    }
}
