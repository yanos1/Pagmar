using Interfaces;
using Terrain.Environment;
using Unity.Cinemachine;

namespace Obstacles
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FallingStoneController : MonoBehaviour, IResettable, IBreakable
    {
        [SerializeField] private List<FallingStone> stones;
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
          
        }

        public void ResetToInitialState()
        {
            currentIndex = 0;
        }

        public void OnBreak()
        {
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            FallingStone stone = stones[currentIndex];
            stone.Activate();
            stone.GetComponent<Rigidbody2D>().AddForce(fallForce, ForceMode2D.Impulse);
            currentIndex++;
        }
    }

}