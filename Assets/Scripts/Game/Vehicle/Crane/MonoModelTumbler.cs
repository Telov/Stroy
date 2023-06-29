using Game.Player;
using UnityEngine;

namespace Game.Vehicle.Crane
{
    public class MonoModelTumbler : MonoBehaviour
    {
        [SerializeField] private GameObject model;

        public void SetActive(bool value)
        {
            model.SetActive(value);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                SetActive(false);
            }

            if (other.GetComponent<MonoVehicle>())
            {
                SetActive(false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            SetActive(true);
        }
    }
}
