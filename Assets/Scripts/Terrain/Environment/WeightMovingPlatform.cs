using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class WeightMovingPlatform : MovingPlatform
    {
        
        private void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.GetComponent<PlayerMovement>() is not null)
            {
                if (!hasMoved)
                {
                    Invoke(nameof(MovePlatformExternally),2);
                }
            }
        }
    }
}