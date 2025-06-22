using System;
using UnityEngine;

namespace Triggers
{
    public class OneWayPassage : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.position.x > transform.position.x)
            {
                GetComponent<Collider2D>().isTrigger = false;
            }
        }
    }
}