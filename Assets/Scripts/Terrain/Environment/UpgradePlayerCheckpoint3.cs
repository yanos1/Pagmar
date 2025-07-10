using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class UpgradePlayerCheckpoint3 : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerMovement>() is { } player)
            {
                player.enableDash = true;
            }
        }
    }
}