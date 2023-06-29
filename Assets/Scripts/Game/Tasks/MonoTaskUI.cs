using System;
using System.Collections.Generic;
using System.Linq;
using Game.Tasks.Interfaces;
using UnityEngine.UI;
using DG.Tweening;
using Game.Player;
using TMPro;
using UnityEngine;

namespace Game.Tasks
{
    public class MonoTaskUI : MonoBehaviour, ITaskUI
    {
        public event Action OnMissionComplete = () => { };

        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject angryParticle;
        [SerializeField] private GameObject happyParticle;
        [SerializeField] private GameObject coolParticle;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private SerializableToggler[] togglers;
        [SerializeField] private PlayerController playerController;

        private List<SerializableTask> _tasks;
        private int _completedCount;
        private bool _isOpen;

        public void Initialize(List<SerializableTask> taskList)
        {
            _tasks = taskList;
            closeButton.onClick.AddListener(ChangeState);
            for (int i = 0; i < _tasks.Count; i++)
            {
                togglers[i].Toggler.onValueChanged.AddListener(ChangeEvent);
                _tasks[i].Initialize(togglers[i]);
            }

            var tasks = _tasks.Select(x => x.Controllable);
            playerController.SetTasks(tasks);
        }

        private void ChangeEvent(bool value)
        {
            if (!value) return;
            _completedCount += 1;
            text.gameObject.SetActive(true);
            if (_completedCount % 2 == 0)
            {
                happyParticle.SetActive(true);
            }
            else
            {
                coolParticle.SetActive(true);
            }

            Invoke(nameof(CloseParticles), 2);
            if (_completedCount == _tasks.Count)
            {
                OnMissionComplete.Invoke();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                angryParticle.SetActive(true);
                Invoke(nameof(CloseParticles), 2);
            }
        }

        private void CloseParticles()
        {
            text.gameObject.SetActive(false);
            angryParticle.SetActive(false);
            happyParticle.SetActive(false);
            coolParticle.SetActive(false);
        }


        private void ChangeState()
        {
            closeButton.transform.localScale *= -1;
            _isOpen = !_isOpen;
            if (!_isOpen)
            {
                transform.DOLocalMoveX(-690, 1);
            }
            else
            {
                transform.DOLocalMoveX(-375, 1);
            }
        }
    }
}