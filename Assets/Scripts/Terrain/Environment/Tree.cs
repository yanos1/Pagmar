using System;
using System;
using Interfaces;
using NPC;
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
            if (trigger.IsTriggered)
            {
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(1f, () =>
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

        public void OnHit(Vector2 hitDir)
        {
            return;
        }
    }
}