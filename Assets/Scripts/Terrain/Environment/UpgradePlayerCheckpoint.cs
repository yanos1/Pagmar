using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class UpgradePlayerCheckpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerMovement>() is { } player)
            {
                player.enableAdvancedDash = true;
            }
        }
    }
}