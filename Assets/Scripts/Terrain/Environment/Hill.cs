using Interfaces;
using SpongeScene;

namespace Terrain.Environment
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine;
    using System.Collections;

    public class Hill : MonoBehaviour, IBreakable
    {
        private Rigidbody2D _rb;
        private bool isShaking = false;
        [SerializeField] private Rigidbody2D objectOnTop;
        [SerializeField] private float force;
        [SerializeField] private float shakeMagnitude;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>(); 
        }

        public void OnBreak()
        {
           return;
        }

        public void OnHit(Vector2 hitDir)
        {
            StartCoroutine(UtilityFunctions.ShakeObject(_rb,0.12f,shakeMagnitude,true,false));
            if (objectOnTop is not null)
            {
                print($"adding force {force}");
                objectOnTop.AddForce(Vector2.left * force);
            }
        }
    }
}