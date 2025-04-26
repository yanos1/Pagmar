using Interfaces;
using Managers;

namespace Terrain.Environment
{
    using UnityEngine;
    using DG.Tweening;
    using System;

    public class FakeRune : MonoBehaviour, IPickable
    {

        [SerializeField] private float floatHeight = 0.2f;
        [SerializeField] private float floatDuration = 1f;

        private Vector3 _startPos;
        

        private void Start()
        {
            _startPos = transform.position;

            // Floating animation using DOTween
            transform.DOMoveY(_startPos.y + floatHeight, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnPick();
        }

        public void OnPick()
        {
            // Disable the object
            gameObject.SetActive(false);

            // Fire the PickFakeRune event
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.PickUpFakeRune, null);
        }
    }
    
}