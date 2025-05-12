using System;
using Interfaces;
using SpongeScene;
using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain.Environment
{
    public class ShakyTree : MonoBehaviour, IResettable
    {

        private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D branchCollider;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                StartCoroutine(UtilityFunctions.ShakeObject(rb, 0.1f, 0.1f,true,false));
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.5f, () => branchCollider.enabled = false));
            }
        }

        public void ResetToInitialState()
        {
            branchCollider.enabled = true;
        }
    }
}