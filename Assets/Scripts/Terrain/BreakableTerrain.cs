using System;
using UnityEngine;

namespace Terrain
{
    public class BreakableTerrain : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<PlayerMovement>() is not null)
            {
                if (other.gameObject.GetComponent<PlayerMovement>().IsDashing)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}