﻿using FMODUnity;
using Interfaces;
using Managers;
using Player;

namespace Terrain.Environment
{
    using UnityEngine;
    using DG.Tweening;
    using System;

    public class FullHealRune : MonoBehaviour, IPickable, IResettable
    {

        [SerializeField] private float floatHeight = 0.2f;
        [SerializeField] private float floatDuration = 1f;
        [SerializeField] private EventNames onPickup;
        [SerializeField] private EventReference healSound;
        private GameObject parent;

        private Vector3 _startPos;
        

        private void Start()
        {
            _startPos = transform.position;
            parent = transform.parent.gameObject;
            // Floating animation using DOTween
            transform.DOMoveY(_startPos.y + floatHeight, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is not null)
            {
                OnPick();
            }
        }

        public void OnPick()
        {
            // Disable the object
            gameObject.SetActive(false);
            CoreManager.Instance.AudioManager.PlayOneShot(healSound, transform.position);
            // Fire the PickFakeRune event
            CoreManager.Instance.EventManager.InvokeEvent(onPickup, 100);
        }

        public void GetDeletedByLavaBeam()
        {
            parent.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            if (parent != null)
            {
                parent.SetActive(true);
            }
        }
    }
    
}