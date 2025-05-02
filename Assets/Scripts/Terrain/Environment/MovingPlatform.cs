using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Terrain.Environment
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private Vector3 triggerDirection;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private AnimationCurve returnMovementCurve;
        [SerializeField] private UnityEvent nextPlatform;
        [SerializeField] private bool returnWhenDone;
        private Vector3 startPos;
        private Vector3 targetPos;
        private bool hasMoved = false;
        

        private void Start()
        {
            startPos = transform.position;
            targetPos = startPos + targetOffset;
        }

        public void MovePlatformExternally()
        {
            StartCoroutine(MovePlatform());
        }
        private IEnumerator MovePlatform()
        {
            float timer = 0f;

            while (timer < moveDuration)
            {
                float t = timer / moveDuration;
                float easedT = movementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, targetPos, easedT);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.position = targetPos;

            // Wait before returning
            yield return new WaitForSeconds(2f);
            if (returnWhenDone)
            {
                yield return StartCoroutine(ReturnPlatform());
            }
        }

        private IEnumerator ReturnPlatform()
        {
            float timer = 0f;

            while (timer < moveDuration)
            {
                float t = timer / moveDuration;
                float easedT = returnMovementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(targetPos, startPos, easedT);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.position = startPos;
            hasMoved = false; // Allow the platform to be triggered again
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player == null) return;

            // Attach player to platform
            if (player.GroundCheckPos.y > transform.position.y)
            {
                collision.collider.transform.SetParent(transform);
            }
            // Check if dash conditions are met
            if (hasMoved || !player.IsDashing) return;

            Vector3 dashDir = player.DashDirection;
            Vector3 playerPos = player.transform.position;

            if (Vector3.Dot(dashDir.normalized, triggerDirection.normalized) < 0.9f)
                return;

            Vector3 fromDirection = (transform.position - playerPos).normalized;
            if (Vector3.Dot(fromDirection, triggerDirection.normalized) < 0.5f)
                return;

            StartCoroutine(MovePlatform());
            nextPlatform?.Invoke();
            hasMoved = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            PlayerManager player = collision.collider.GetComponent<PlayerManager>();
            if (player)
            {
                collision.collider.transform.SetParent(null);
            }
        }
    }
}
