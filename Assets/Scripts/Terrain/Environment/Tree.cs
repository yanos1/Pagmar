using System;
using System;
using FMODUnity;
using Interfaces;
using Managers;
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
        [SerializeField] private GameObject impass;
        [SerializeField] private EventReference treeHit;
        [SerializeField] private EventReference treeImpact;
        private Rigidbody2D rb;
        private bool hasFallen = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnMouseDown()
        {
            OnBreak();
        }

        private void Update()
        {
            if (!hasFallen && trigger.IsTriggered && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                hasFallen = true;
                CoreManager.Instance.AudioManager.PlayOneShot(treeImpact, transform.position);
                
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.3f, () =>
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                    gameObject.layer = 3; // ground layer
                    impass.SetActive(false);
                }));
            }
        }

        public void OnBreak()
        {
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.5f,() => CoreManager.Instance.AudioManager.PlayOneShot(treeHit, transform.position)));
           
            rb.AddForce(Vector2.left * power);
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            print($"hit dir is {hitDir}");
            if (stage == PlayerStage.Adult && hitDir.x < 0 ) // we are to the right of the tree
            {
                OnBreak();
            }
        }
        
    }
}