using Interfaces;
using Managers;
using UnityEngine;
using Utility;

namespace Atmosphere.TileExplostion
{
    public class ExplodableTile : Poolable, IResettable
    {
        [SerializeField] private Explodable e;
        private Rigidbody2D rb;
        private Vector3 startingPos;
        public void Start()
        {
            startingPos = transform.position;
            Type = PoolEnum.ExplodableTile;
            rb = GetComponent<Rigidbody2D>();
        }

        public void Explode()
        {
            e.explode();
        }

        public void ResetToInitialState()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = startingPos;
            gameObject.SetActive(false);
        }
    }
}