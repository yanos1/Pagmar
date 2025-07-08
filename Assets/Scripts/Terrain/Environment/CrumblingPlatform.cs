using Interfaces;
using Managers;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Terrain.Environment
{
    public class CrumblingPlatform : MonoBehaviour, IResettable
    {
        public float crumbleDelay = 2f;
        public bool destroyAfterCrumble = false;
        public float crumbleDuration = 0.7f;
        public Animator animator;

        private bool hasCrumbled = false;
        private Collider2D col;
        private SpriteRenderer sr;
        private Vector3 initialPosition;

        [SerializeField] private bool reset = true;

        // Tween references
        private Tween moveTween;
        private Tween fadeTween;

        // Coroutine reference
        private Coroutine crumbleCoroutine;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();

            initialPosition = transform.position;
        }

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.Die, OnPlayerDied);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.Die, OnPlayerDied);
        }

        private void OnPlayerDied(object _)
        {
            Debug.Log("Player died — kill tweens");
            KillTweens();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasCrumbled) return;

            if (collision.gameObject.GetComponent<PlayerMovement>() is not null &&
                collision.gameObject.transform.position.y > transform.position.y)
            {
                hasCrumbled = true;
                crumbleCoroutine = StartCoroutine(CrumbleAfterDelay());
            }
        }

        private IEnumerator CrumbleAfterDelay()
        {
            yield return new WaitForSeconds(crumbleDelay);
            Crumble();
        }

        public void CrumbleQuick()
        {
            fadeTween = sr.DOFade(0f, crumbleDuration).OnComplete(DisablePlatform);
        }

        private void Crumble()
        {
            col.enabled = false;

            moveTween = transform.DOMoveY(transform.position.y - 2f, crumbleDuration)
                .SetEase(Ease.InQuad);

            fadeTween = sr.DOFade(0f, crumbleDuration).OnComplete(DisablePlatform);
        }

        private void DisablePlatform()
        {
            gameObject.SetActive(false);
        }

        public virtual void ResetToInitialState()
        {
            if (!reset) return;

            // Kill active tweens
            KillTweens();

            // Stop crumble coroutine if it's still waiting
            if (crumbleCoroutine != null)
            {
                StopCoroutine(crumbleCoroutine);
                crumbleCoroutine = null;
            }

            gameObject.SetActive(true);
            hasCrumbled = false;
            transform.position = initialPosition;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            col.enabled = true;
        }

        private void KillTweens()
        {
            moveTween?.Kill();
            fadeTween?.Kill();
            moveTween = null;
            fadeTween = null;
        }
    }
}
