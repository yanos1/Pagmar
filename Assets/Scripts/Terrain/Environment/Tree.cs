using System;
using System;
using Interfaces;
using NPC;
using Player;
using SpongeScene;
using Triggers;
using UnityEngine;

namespace Terrain.Environment
{
    public class Tree : MonoBehaviour, IBreakable
    {
        [SerializeField] private float power;
        [SerializeField] private Trigger trigger;
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (trigger.IsTriggered && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.3f, () =>
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                }));
            }
        }

        public void OnBreak()
        {
            rb.AddForce(Vector2.left * power);
        }

        public void OnHit(Vector2 hitDir, PlayerManager.PlayerStage stage)
        {
            return;
        }
        
    }
}