using System.Collections;
using Interfaces;
using Triggers;
using UnityEngine;

namespace Terrain.Environment
{
    public class OneTimeBeam : MonoBehaviour, IResettable
    {
        [SerializeField] private Trigger trigger;
        private Collider2D _collider;
        private SpriteRenderer _sr;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            _collider.enabled = false;
            StartCoroutine(WaitForTriggerThenActivate());
        }

        private IEnumerator WaitForTriggerThenActivate()
        {
            // Wait until trigger is activated
            yield return new WaitUntil(() => trigger.IsTriggered);

            // Set sprite visible
            Color color = _sr.color;
            color.a = 1f;
            _sr.color = color;

            // Enable collider
            _collider.enabled = true;

            // Wait 1 second, then reset
            yield return new WaitForSeconds(1f);
            ResetToInitialState();
        }

        public void ResetToInitialState()
        {
            // Hide sprite
            Color color = _sr.color;
            color.a = 0f;
            _sr.color = color;

            // Disable collider
            _collider.enabled = false;

            // Restart listening for trigger
            StartCoroutine(WaitForTriggerThenActivate());
        }
    }
}