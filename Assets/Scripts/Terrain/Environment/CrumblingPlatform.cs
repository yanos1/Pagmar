using Interfaces;

namespace Terrain.Environment
{
    using UnityEngine;
    using DG.Tweening; // Add DoTween namespace

    public class CrumblingPlatform : MonoBehaviour, IResettable
    {
        public float crumbleDelay = 2f;
        public bool destroyAfterCrumble = false;
        public float crumbleDuration = 0.7f; // Duration of the crumble effect
        public Animator animator; // Optional for crumble animation

        private bool hasCrumbled = false;
        private Collider2D col;
        private SpriteRenderer sr;
        private Vector3 initialPosition;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();

            initialPosition = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasCrumbled) return;
            if (collision.gameObject.transform.position.y > transform.position.y)
            {
                hasCrumbled = true;
                Invoke(nameof(Crumble), crumbleDelay);
            }

        }

        private void Crumble()
        {
            // if (animator != null)
            //     animator.SetTrigger("Crumble");

            transform.DOMoveY(transform.position.y - 2f, crumbleDuration) 
                .SetEase(Ease.InQuad);
            sr.DOFade(0f, crumbleDuration).OnComplete(DisablePlatform);
            col.enabled = false;
            Invoke(nameof(ResetToInitialState),6);
            
        }

        private void DisablePlatform()
        {
            gameObject.SetActive(false); // Or any other way to handle it
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            hasCrumbled = false;
            transform.position = initialPosition;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            col.enabled = true;
        }
    }
}