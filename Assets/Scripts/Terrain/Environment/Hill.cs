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
            StartCoroutine(UtilityFunctions.ShakeObject(_rb,0.1f,0.05f));
        }
    }
}