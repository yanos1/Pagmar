using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unity2DDestruction.Assets._2D_Destruction.Scripts
{
    public class NoncollidingExplodable : ExplodableAddon
    {
        private List<GameObject> fragments;
        private bool stop = false;

        public override void OnFragmentsGenerated(List<GameObject> fragments)
        {
            // Optional: use if needed
        }

        public override void OnFragmentsExploded(List<GameObject> fragments)
        {
            this.fragments = fragments;
            StartCoroutine(CheckFragmentsRoutine());
        }

        private IEnumerator CheckFragmentsRoutine()
        {
            yield return new WaitForSeconds(1.2f); // Initial delay

            // Create a LayerMask for the ground (make sure "Ground" layer exists)
            int groundLayerMask = LayerMask.GetMask("Ground");

            while (!stop)
            {
                var anyActive = false;

                foreach (var frag in fragments)
                {
                    if (frag == null) continue;

                    var rb = frag.GetComponent<Rigidbody2D>();
                    var col = frag.GetComponent<Collider2D>();

                    // Check if it's already stopped
                   
                        // Raycast down from the fragment's position
                    RaycastHit2D hit = Physics2D.Raycast(frag.transform.position, Vector2.down, 0.1f, groundLayerMask);


                    // Only disable collider and make static if touching the ground
                    if (hit.collider != null && hit.collider.GetComponent<Rigidbody2D>() is null)
                    {
                        col.enabled = false;
                        rb.bodyType = RigidbodyType2D.Static;
                    }
                    else
                    {
                        anyActive = true;
                    }
              
                }

                if (!anyActive)
                {
                    stop = true;
                    gameObject.SetActive(false);
                    yield break;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

    }
}