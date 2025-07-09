using DG.Tweening;
using FMODUnity;
using Managers;
using NPC;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class NpcHealRune : HealRune
    {
        private bool isCarried = false;
        [SerializeField] private EventReference healSound;
        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is { } carrier && isCarried == false)
            {
                print("carry!");
                Collider2D carrierCollider = carrier.GetComponent<Collider2D>();
                Bounds bounds = carrierCollider.bounds;

                Vector3 newPosition = new Vector3(
                    bounds.center.x,
                    bounds.max.y + 0.2f,
                    transform.position.z
                );

                transform.position = newPosition;
                _floatTween.Kill();
                isCarried = true;
                transform.SetParent(carrier.transform);
                _rb.bodyType = RigidbodyType2D.Kinematic;
                CoreManager.Instance.AudioManager.PlayOneShot(pickUpHealSound, transform.position);
                CoreManager.Instance.EventManager.InvokeEvent(onPickup, healAmount);

            }
            
            if (other.GetComponent<Npc>() is { } npc)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(healSound, transform.position);
                npc.Heal();
                gameObject.SetActive(false);
            }
        }

        public override void Update()
        {
            if (!isCarried)
            {
                base.Update(); // floats if not carried
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            isCarried = false;
            transform.SetParent(null);
        }
    }
}