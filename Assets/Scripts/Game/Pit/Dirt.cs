using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Pit
{
    public class Dirt : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private GameObject crackPrefab;
        [SerializeField] private GameObject fakeView;
        [SerializeField] private Rigidbody[] chunks;
        [SerializeField] private Color color;

        private Color _defaultColor;
        private Material _material;

        public void TakeDamage(Action callBack)
        {
            if (!enabled) return;

            health -= 1;
            if (health < 1)
            {
                fakeView.SetActive(false);

                foreach (var chunk in chunks)
                {
                    chunk.gameObject.SetActive(true);
                    chunk.isKinematic = false;
                    chunk.AddForce(new Vector3(Random.Range(-5, 5), -1, Random.Range(-5, 5)), ForceMode.Impulse);
                    chunk.transform.DOScale(Vector3.zero, 2).OnComplete(() => { Destroy(chunk.gameObject); });
                }

                callBack.Invoke();
                GetComponent<BoxCollider>().enabled = false;
                enabled = false;
            }
            else
            {
                transform.DOPunchPosition(Vector3.up / 4, .2f, 12);
                //transform.DOShakePosition(.2f, .1f);
                _material.DOColor(color, .2f).OnComplete(() =>
                {
                    _material.DOColor(_defaultColor, .2f);
                });
            }
        }

        private void Start()
        {
            _material = fakeView.GetComponent<MeshRenderer>().materials[0];
            _defaultColor = _material.color;
        }
    }
}