using System;
using Managers;
using UnityEngine;

namespace Triggers
{
    public class DangerLayerTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                CoreManager.Instance.AudioManager.SetGlobalParameter("Danger", 1);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                CoreManager.Instance.AudioManager.SetGlobalParameter("Danger", 0);
            }
        }
    }
}