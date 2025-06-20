using Interfaces;
using Managers;
using Player;

namespace Terrain.Environment
{
    using UnityEngine;
    using DG.Tweening;
    using System;

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class HealRune : MonoBehaviour, IPickable, IResettable
    {
        [SerializeField] protected int healAmount;
        [SerializeField] private float floatHeight = 0.2f;
        [SerializeField] private float floatDuration = 1f;
        [SerializeField] protected EventNames onPickup;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.1f;

        private Vector3 _startPos;
        protected Rigidbody2D _rb;
        protected Tween _floatTween;
        private bool _hasLanded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _startPos = transform.position;
            ResetToInitialState();
        }

        public virtual void Update()
        {
            if (_hasLanded) return;

            if (IsGrounded())
            {
                Land();
            }
            else
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        }

        private void Land()
        {
            _hasLanded = true;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;

            _floatTween = transform.DOMoveY(transform.position.y + floatHeight, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() != null)
            {
                OnPick();
            }
        }

        public void OnPick()
        {
            _floatTween?.Kill();
            print($"invoked {onPickup} 12");
            CoreManager.Instance.EventManager.InvokeEvent(onPickup, healAmount);
            gameObject.SetActive(false);
        }

        public virtual void ResetToInitialState()
        {
            transform.position = _startPos;
            gameObject.SetActive(true);
            _hasLanded = false;
            _floatTween?.Kill();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize ground check ray
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}
