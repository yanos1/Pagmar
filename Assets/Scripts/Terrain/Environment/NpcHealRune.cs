using DG.Tweening;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class NpcHealRune : HealRune
    {
        private bool isCarried = false;
        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is { } carrier)
            {
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
            }
        }

        public override void Update()
        {
            if (!isCarried)
            {
                base.Update(); // floats if not carried
            }
        }
    }
}