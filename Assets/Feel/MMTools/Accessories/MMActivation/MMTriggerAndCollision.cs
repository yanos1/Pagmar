using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools
{
    [AddComponentMenu("More Mountains/Tools/Activation/MM Trigger And Collision")]
    public class MMTriggerAndCollision : MonoBehaviour
    {
        public LayerMask CollisionLayerMask;
        public UnityEvent OnCollisionEnterEvent;
        public UnityEvent OnCollisionExitEvent;
        public UnityEvent OnCollisionStayEvent;

        public LayerMask TriggerLayerMask;
        public UnityEvent OnTriggerEnterEvent;
        public UnityEvent OnTriggerExitEvent;
        public UnityEvent OnTriggerStayEvent;

        public LayerMask Collision2DLayerMask;
        public UnityEvent OnCollision2DEnterEvent;
        public UnityEvent OnCollision2DExitEvent;
        public UnityEvent OnCollision2DStayEvent;

        public LayerMask Trigger2DLayerMask;
        public UnityEvent OnTrigger2DEnterEvent;
        public UnityEvent OnTrigger2DExitEvent;
        public UnityEvent OnTrigger2DStayEvent;

        public int allowedTriggers = 1;
        private int _triggerCount = 0;

        // Collision 2D ------------------------------------------------------------------------------------

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Collision2DLayerMask.MMContains(collision.gameObject))
            {
                _triggerCount++;
                OnCollision2DEnterEvent.Invoke();
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Collision2DLayerMask.MMContains(collision.gameObject))
            {
                _triggerCount++;
                OnCollision2DExitEvent.Invoke();
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Collision2DLayerMask.MMContains(collision.gameObject))
            {
                _triggerCount++;
                OnCollision2DStayEvent.Invoke();
            }
        }

        // Trigger 2D ------------------------------------------------------------------------------------

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Trigger2DLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTrigger2DEnterEvent.Invoke();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Trigger2DLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTrigger2DExitEvent.Invoke();
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (Trigger2DLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTrigger2DStayEvent.Invoke();
            }
        }

        // Collision ------------------------------------------------------------------------------------

        protected virtual void OnCollisionEnter(Collision c)
        {
            if (_triggerCount >= allowedTriggers) return;
            if ((CollisionLayerMask.value & (1 << c.transform.gameObject.layer)) != 0)
            {
                _triggerCount++;
                OnCollisionEnterEvent.Invoke();
            }
        }

        protected virtual void OnCollisionExit(Collision c)
        {
            if (_triggerCount >= allowedTriggers) return;
            if ((CollisionLayerMask.value & (1 << c.transform.gameObject.layer)) != 0)
            {
                _triggerCount++;
                OnCollisionExitEvent.Invoke();
            }
        }

        protected virtual void OnCollisionStay(Collision c)
        {
            if (_triggerCount >= allowedTriggers) return;
            if ((CollisionLayerMask.value & (1 << c.transform.gameObject.layer)) != 0)
            {
                _triggerCount++;
                OnCollisionStayEvent.Invoke();
            }
        }

        // Trigger ------------------------------------------------------------------------------------

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (TriggerLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTriggerEnterEvent.Invoke();
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (TriggerLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTriggerExitEvent.Invoke();
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (_triggerCount >= allowedTriggers) return;
            if (TriggerLayerMask.MMContains(collider.gameObject))
            {
                _triggerCount++;
                OnTriggerStayEvent.Invoke();
            }
        }

        // Utility ---------------------------------------------------------------------------------------

        public void ResetTriggerCount()
        {
            _triggerCount = 0;
        }

        protected virtual void Reset()
        {
            Collision2DLayerMask = LayerMask.NameToLayer("Everything");
            CollisionLayerMask = LayerMask.NameToLayer("Everything");
        }
    }
}
