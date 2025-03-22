using System;
using Player;
using UnityEngine;

namespace Terrain
{
    public class BreakableTerrain : MonoBehaviour, IBreakable
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<PlayerMovement>() is not null)
            {
                if (other.gameObject.GetComponent<PlayerMovement>().IsDashing)
                {
                    OnBreak();
                }
            }
        }

        public void OnBreak()
        {
            gameObject.SetActive(false);
            
        }
    }
}