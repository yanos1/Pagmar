using Managers;
using UnityEngine;

namespace Triggers
{
    public class PassageOpener : MonoBehaviour
    {
        private int orbPickedUp = 0;
        [SerializeField] private Collider2D secondOrbBlock;
        [SerializeField] private Collider2D firstOrbBlock;

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
            orbPickedUp++;
            if (orbPickedUp == 1)
            {
                firstOrbBlock.enabled = true; // blocks the way to the orb after player picked it up
            }

            else if (orbPickedUp == 2) // there are 2 orbs in uppergroud to pick up, so we picked them all. we let the player through
            {
                secondOrbBlock.enabled = false;
            }
        }
    }
}