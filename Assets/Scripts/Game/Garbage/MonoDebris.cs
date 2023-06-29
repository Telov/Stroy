using UnityEngine;

namespace Game.Garbage
{
    public class MonoDebris : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsTree
        {
            get;
            private set;
        }
    }
}
