using System;
using Interfaces;
using Managers;
using SpongeScene;
using UnityEngine;
using Utility;

namespace Atmosphere.TileExplostion
{
    public class ExplodableTile : Poolable, IResettable
    {
        [SerializeField] private Explodable e;
        private Rigidbody2D rb;
        private Vector3 startingPos;
        private bool addedAsRessetable = false;

        private void OnEnable()
        {
            if(addedAsRessetable) return;
            addedAsRessetable = true;
            CoreManager.Instance.ResetManager.AddResettable(this);
            CoreManager.Instance.ResetManager.AddResettable(gameObject.GetComponent<Explodable>());
        }
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
            print("reset explodable tile");
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = startingPos;
            gameObject.SetActive(false);
        }
    }
}