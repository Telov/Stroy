using System;
using System.Collections;
using System.Threading.Tasks;
using Game.Particles.Enum;
using UnityEngine;

namespace Game.Particles
{
    public class MonoParticles : MonoBehaviour
    {
        [SerializeField] private GameObject[] particles;

        private int _id;

        public void SetParticle(EParticles particleType)
        {
            _id = (int)particleType;
        }

        public void SetActive(bool value)
        {
            if (value)
            {
                ClearParticles();
            }
            particles[_id].SetActive(value);
        }

        public async void Disable(float time, Action callback)
        {
            await Task.Delay(TimeSpan.FromSeconds(time));
            //SetActive(false);
            callback.Invoke();
        }

        private void ClearParticles()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(false);
            }
        }
    }
}