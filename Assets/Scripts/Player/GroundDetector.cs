using System;
using UnityEngine;

namespace Player
{
    public class GroundDetector : MonoBehaviour
    {
        private LayerMask groundLayer;
        private void Start()
        {
            groundLayer = LayerMask.NameToLayer("Ground");
        }

        public int? GetGroundMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
            if (hit.collider is not null)
            {
                return GetMaterialIndex(hit.collider);
            }
            return null;
        }
        
        private int GetMaterialIndex(Collider2D collider)
        {
            if (collider.CompareTag("LeavesAndDirt")) return 0;
            if (collider.CompareTag("Rock")) return 1;
            if (collider.CompareTag("WeakRock")) return 2;
            if (collider.CompareTag("Metal")) return 3;
            if (collider.CompareTag("Bone")) return 4;
            if (collider.CompareTag("TreeBranches")) return 5;
            if (collider.CompareTag("Wood")) return 6;
            return -1;
        }
        
        
    }
}