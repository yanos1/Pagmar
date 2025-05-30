using System;
using UnityEngine;

namespace Player
{
    public class PlayerTimelineActions : MonoBehaviour
    {
        private int force = 100;
        private int forceAddition = 50;
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void BumpUp()
        {
            rb.AddForce(Vector2.up * force);
            force += forceAddition;
        }
    }
}