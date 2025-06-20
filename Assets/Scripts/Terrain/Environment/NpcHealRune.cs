using DG.Tweening;
using Managers;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class NpcHealRune : HealRune
    {
        private bool isCarried = false;
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
                CoreManager.Instance.EventManager.InvokeEvent(onPickup, healAmount);

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