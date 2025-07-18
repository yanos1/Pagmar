using Managers;
using UnityEngine;

namespace Triggers
{
    public class PassageOpener : MonoBehaviour
    {
        private int orbPickedUp = 0;

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.BigPickUpHeal, OnPickUp);
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.BigPickUpHeal, OnPickUp);
        }

        private void OnPickUp(object obj)
        {
            if (++orbPickedUp == 2) // there are 2 orbs in uppergroud to pick up, so we picked them all
            {
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}