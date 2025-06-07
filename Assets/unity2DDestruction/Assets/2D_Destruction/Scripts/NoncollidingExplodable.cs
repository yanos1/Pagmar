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
            yield return new WaitForSeconds(2f); // Initial delay

            while (!stop)
            {
                var anyActive = false;

                foreach (var frag in fragments)
                {
                    var rb = frag.GetComponent<Rigidbody2D>();
                    if (rb.linearVelocity.magnitude < 2)
                    {
                        frag.GetComponent<Collider2D>().enabled = false;
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
                    yield break; // Exit the coroutine
                }

                yield return new WaitForSeconds(0.5f); // Wait before checking again
            }
        }
    }
}