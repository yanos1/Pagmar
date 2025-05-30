﻿using Interfaces;
using Unity.Cinemachine;

namespace Obstacles
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FallingStoneController : MonoBehaviour, IResettable
    {
        [SerializeField] private List<Rigidbody2D> stones;
        [SerializeField] private Vector2 fallForce;

        private int currentIndex = 0;

        // Trigger this method to activate the next stone
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (stones == null || stones.Count == 0) return;
            
            if (currentIndex >= stones.Count)
            {
                Debug.Log("All stones have already fallen.");
                return;
            }

            if (col.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                Rigidbody2D stone = stones[currentIndex];
                stone.bodyType = RigidbodyType2D.Dynamic;
                stone.AddForce(fallForce, ForceMode2D.Impulse);
                currentIndex++;
            }
          
        }

        public void ResetToInitialState()
        {
            currentIndex = 0;
        }
    }

}