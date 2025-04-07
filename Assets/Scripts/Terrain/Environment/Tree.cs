using System;
using NPC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Terrain.Environment
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private float power = 25;
        private int hitcount;
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            
            if (other.GetComponent<Npc>() is not null)
            {
                if (++hitcount == 2)
                {
                    Fall();
                }
            }
        }

        private void Fall()
        {
            rb.AddForce(Vector2.left * power);
        }
    }
}