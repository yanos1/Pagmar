using System.Collections;
using System.Xml.Schema;
using Enemies;
using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
using Player;
using SpongeScene;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Terrain.Environment
{
    public class MovingPlatform : MonoBehaviour, IResettable,IBreakable
    {
        [Header("Movement Settings")] 
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] protected Vector3 triggerDirection;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private float secondsBeforeReturn;
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private AnimationCurve returnMovementCurve;
        [SerializeField] private bool returnWhenDone;
        [SerializeField] private UnityEvent nextPlatformMove;
        [SerializeField] private UnityEvent nextPlatformMoveGently;

        [Header("Gentle Push Settings")]
        [SerializeField] private float gentleMoveDistance = 0.17f;
        [SerializeField] private float gentleMoveDuration = 0.3f;
        [SerializeField] private AnimationCurve gentleMoveCurve;

        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private EventReference moveSound;
        [SerializeField] private EventReference returnSound;
        [SerializeField] private BoxCollider2D col;
        
        
        private Vector3 startPos;
        private Vector3 targetPos;
        protected bool isMoving = false;
        protected bool isReturning = false;
        private bool isGentlyMoving = false;
        private bool isLooping = false;
        private Coroutine moveslightlyCor;
        private Coroutine moveCoroutine;
        private EventInstance moveInstance;
        private EventInstance returnInstance;

        private void Start()
        {
            startPos = transform.position;
            targetPos = startPos + targetOffset;
            
        }

        public void MovePlatformExternally()
        {
           moveCoroutine =  StartCoroutine(MovePlatform());
        }

        public void MovePlatformGentlyExternally()
        {
            moveslightlyCor  =  StartCoroutine(MoveGentlyUp());
        }
        private IEnumerator MovePlatform()
        {
            print("move platform!");
            
            if (isMoving || isReturning)  yield break;
            isMoving = true;
            print("yes move");
            if (moveslightlyCor != null)
            {
                StopCoroutine(moveslightlyCor);
            }
            float timer = 0f;
            PlayMoveSound();
            while (timer < moveDuration)
            {
                float t = timer / moveDuration;
                float easedT = movementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, targetPos, easedT);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.position = targetPos;
            StopMoveSound();
            yield return new WaitForSeconds(secondsBeforeReturn);

            print($"stop moving after {moveDuration}");
            isMoving = false;
            if (returnWhenDone)
            {
                print("return platform");
                yield return StartCoroutine(ReturnPlatform());
            }
           
        }
        
        private IEnumerator MovePlatformHalfwayFast()
        {
            if (moveslightlyCor != null)
                StopCoroutine(moveslightlyCor);
            float fastDuration = moveDuration / 1.5f;
            Vector3 halfTarget = transform.position + targetOffset * 0.5f;

            float timer = 0f;
            while (timer < fastDuration)
            {
                float t = timer / fastDuration;
                float easedT = movementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(transform.position, halfTarget, easedT);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.position = halfTarget;
            isMoving = false;
        }

        private void PlayMoveSound()
        {
            moveInstance = CoreManager.Instance.AudioManager.CreateEventInstance(moveSound);
            moveInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            moveInstance.start();
        }


        private void StopMoveSound()
        {
            moveInstance.stop(STOP_MODE.IMMEDIATE);
            moveInstance.release();
        }
        
        private void PlayReturnSound()
        {
            returnInstance = CoreManager.Instance.AudioManager.CreateEventInstance(moveSound);
            returnInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            returnInstance.start();
        }


        private void StopReturnSound()
        {
            returnInstance.stop(STOP_MODE.IMMEDIATE);
            returnInstance.release();
        }

        
        
        private IEnumerator ReturnPlatform()
        {
            isReturning = true;
            float timer = 0f;
            PlayReturnSound();
            while (timer < moveDuration)
            {
                float t = timer / moveDuration;
                float easedT = returnMovementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(targetPos, startPos, easedT);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            StopReturnSound();
            transform.position = startPos;
            isReturning = false;
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            
            FallingStone rock = collision.collider.GetComponent<FallingStone>();
            if (rock != null)
            {
                this.StopAndStartCoroutine(ref moveCoroutine, MovePlatformHalfwayFast());
                return;
            }

            if (collision.collider.gameObject.GetComponent<ChargingEnemy>() is not null)
            {
                print("found chargig enemy!!");
                collision.collider.transform.SetParent(transform);
            }
            
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player is not null)
            {
                print("player hit platform");
                if (CoreManager.Instance.Player.playerStage == PlayerStage.FinalForm && player.IsDashing)  // this is a stupid work around since onhit doesnt work and i dont know why.
                {
                    print("break");
                    OnBreak();
                    return;
                }
                print($"p{player.GroundCheckPos.y +0.2f} e {col.bounds.max.y}");
                if (player.transform.position.y +0.2f> col.bounds.max.y)
                {
                    print("setting parent elevator");
                    collision.collider.transform.SetParent(transform);
                }
                
                // If not dashing, move gently
                if (!player.IsDashing && moveCoroutine is null)
                {
                    if (!isGentlyMoving)
                    {
                        moveslightlyCor = StartCoroutine(MoveGentlyDown());
                    }

                    return;
                }

                if (isMoving || !player.IsDashing) return;

                // Check dash direction
                Vector3 dashDir = player.DashDirection;
                Vector3 playerPos = player.transform.position;

                if (Vector3.Dot(dashDir.normalized, triggerDirection.normalized) < 0.9f)
                {
                    print("direction is wrong!");
                    return;
                }

                Vector3 fromDirection = (transform.position - playerPos).normalized;
                // if (Vector3.Dot(fromDirection, triggerDirection.normalized) < 0.5f)
                //     return;
                print("player passed checks");
                if (moveslightlyCor is not null) StopCoroutine(moveslightlyCor);
                moveCoroutine = StartCoroutine(MovePlatform());
                nextPlatformMove?.Invoke();
            }
        }

        private IEnumerator MoveGentlyDown()
        {
            isGentlyMoving = true;
            nextPlatformMoveGently?.Invoke();
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
        
        private IEnumerator MoveGentlyUp()
        {
            if (moveCoroutine is not null) yield break;

            isGentlyMoving = true;
            
            Vector3 originalPos = transform.position;
            Vector3 destination = originalPos + Vector3.up * gentleMoveDistance;
            PlayMoveSound();
            float timer = 0f;
            while (timer < gentleMoveDuration)
            {
                float t = timer / gentleMoveDuration;
                float easedT = gentleMoveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(originalPos, destination, easedT);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = destination;

            // Return
            timer = 0f;
            while (timer < gentleMoveDuration)
            {
                float t = timer / gentleMoveDuration;
                float easedT = returnMovementCurve.Evaluate(t);
                transform.position = Vector3.Lerp(destination, originalPos, easedT);
                timer += Time.deltaTime;
                yield return null;
            }
            StopMoveSound();

            transform.position = originalPos;
            isGentlyMoving = false;
        }
        
        

        public virtual void OnCollisionExit2D(Collision2D collision)
        {
            PlayerManager player = collision.collider.GetComponent<PlayerManager>();
            if (player)
            {
                collision.collider.transform.SetParent(null);
            }
            
            if (collision.collider.gameObject.GetComponent<ChargingEnemy>() is not null)
            {
                collision.collider.transform.SetParent(null);
            }
        }

        public virtual void ResetToInitialState()
        {
            StopMoveSound();
            StopAllCoroutines();
            moveCoroutine = null;
            moveslightlyCor = null;
            isMoving = false;
            transform.position = startPos;
            gameObject.SetActive(true);
            col.enabled = true;
        }

        public void OnBreak()
        {
            StopMoveSound();
            col.enabled = false;
            e.explode();
            f.doExplosion(f.transform.position);
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)  // player stge is max since this aperas only in game end
        {
            print("player hit moving platform!");
            if (stage == PlayerStage.FinalForm && e is not null && f is not null)
            {
                col.enabled = false;
                print(" moving platform break!");
                OnBreak();
            }
        }
    }
}
