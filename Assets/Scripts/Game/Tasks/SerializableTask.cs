using System.Threading.Tasks;
using Game.Tasks.Interfaces;
using Game.Vehicle;
using UnityEngine;
using System;

namespace Game.Tasks
{
    [Serializable]
    public class SerializableTask
    {
        [SerializeField] private int quantity;
        [SerializeField] private Sprite sprite;

        public async void Initialize(SerializableToggler settings)
        {
            var taskConsumer = Controllable.GetComponent<ITaskConsumer>();
            if (taskConsumer == null) throw new Exception("Wrong instance placed");
            settings.Toggler.gameObject.SetActive(true);
            while (taskConsumer.TaskTracker == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(.1f));
            }

            var taskTracker = taskConsumer.TaskTracker;
            settings.TMPro.text = $"0/{quantity}";
            settings.Image.sprite = sprite;
            taskTracker.SetTask(quantity);
            taskTracker.OnComplete += () =>
            {
                settings.TMPro.text = string.Empty;
                settings.Toggler.isOn = true;
            };
            taskTracker.OnIncrease += ChangeText;
            
            void ChangeText() => settings.TMPro.text = $"{taskTracker.CurrentCount}/{quantity}";
        }

        [field: SerializeField] public MonoControllable Controllable { get; private set; }
    }
}