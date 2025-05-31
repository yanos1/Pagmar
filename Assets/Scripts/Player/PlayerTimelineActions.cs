using System;
using UnityEngine;

namespace Player
{
    public class PlayerTimelineActions : MonoBehaviour
    {
        private int force = 100;
        private int forceAddition = 50;
        private Rigidbody2D rb;
        private PlayerMovement _playerMovement;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void BumpUp()
        {
            if(_playerMovement.jumpIsPressed) return;
            rb.AddForce(Vector2.up * force);
            force += forceAddition;
        }
    }
}