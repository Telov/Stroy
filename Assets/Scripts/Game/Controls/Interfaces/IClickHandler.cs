using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Controls.Interfaces
{
    public interface IClickHandler
    {
        event Action<RaycastHit> OnGetHit;

        void SetActive(bool value);
        void SetLayerMask(int layerMask);
    }
}
