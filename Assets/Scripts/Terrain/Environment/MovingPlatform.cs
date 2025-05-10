using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Terrain.Environment
{
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private Vector3 triggerDirection;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private float secondsBeforeReturn;
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private AnimationCurve returnMovementCurve;
        [SerializeField] private bool returnWhenDone;
        [SerializeField] private UnityEvent nextPlatform;

        [Header("Gentle Push Settings")]
        [SerializeField] private float gentleMoveDistance = 0.17f;
        [SerializeField] private float gentleMoveDuration = 0.3f;
        [SerializeField] private AnimationCurve gentleMoveCurve;

        private Vector3 startPos;
        private Vector3 targetPos;
        private bool hasMoved = false;
        private bool isGentlyMoving = false;
        private bool isLooping = false;

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

            yield return new WaitForSeconds(secondsBeforeReturn);

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
            hasMoved = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player is not null)
            {

                // Attach player to platform
                if (player.GroundCheckPos.y > transform.position.y)
                {
                    collision.collider.transform.SetParent(transform);
                }

                // If not dashing, move gently
                if (!player.IsDashing)
                {
                    if (!isGentlyMoving)
                    {
                        StartCoroutine(MoveDownGently());
                    }

                    return;
                }

                if (hasMoved) return;

                // Check dash direction
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

            Box box = collision.collider.GetComponent<Box>();

            if (box != null)
            {
                collision.collider.transform.SetParent(transform);
            }

      
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(isLooping) return;
            
            StartCoroutine(MoveEternally());
            isLooping = true;
        }

        private IEnumerator MoveEternally()
        {
            while (true)
            {
                yield return MovePlatform();
            }
        }

        private IEnumerator MoveDownGently()
        {
            isGentlyMoving = true;

            Vector3 originalPos = transform.position;
            Vector3 downPos = originalPos + Vector3.down * gentleMoveDistance;

            float timer = 0f;
            while (timer < gentleMoveDuration)
            {
                float t = timer / gentleMoveDuration;
                float easedT = gentleMoveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(originalPos, downPos, easedT);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = downPos;

            // Return
            timer = 0f;
            while (timer < gentleMoveDuration)
            {
                float t = timer / gentleMoveDuration;
                float easedT = returnMovementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(downPos, originalPos, easedT);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPos;
            isGentlyMoving = false;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            PlayerManager player = collision.collider.GetComponent<PlayerManager>();
            if (player)
            {
                collision.collider.transform.SetParent(null);
            }
            
            Box box = collision.collider.GetComponent<Box>();
            if (box)
            {
                collision.collider.transform.SetParent(null);
            }
        }
    }
}
