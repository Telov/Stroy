using DG.Tweening;
using UnityEngine;

namespace Game.Building
{
    public class ChunkCollision : MonoBehaviour
    {
        // Called from SendMessage in fractured obj
        private void OnChunkDetach(FracturedChunk.CollisionInfo info)
        {
            if (info.used)
            {
                print("Recursive call");
            }
            // if (info.bCancelCollisionEvent) return;
            //
            // var chunk = info.chunk.transform;
            // chunk.DOScale(Vector3.zero, 5).OnComplete(() =>
            // {
            //     GameObject o;
            //     (o = chunk.gameObject).SetActive(false);
            //     Destroy(o, 10);
            // });
            //
            print("coolide");
            info.bCancelCollisionEvent = false;
        }
    }
}