using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unity2DDestruction.Assets._2D_Destruction.Scripts
{
    public class BecomeDebreeAddon : ExplodableAddon
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
            foreach (var frag in fragments)
            {
                frag.layer = LayerMask.NameToLayer("Debree");
            }
        }
    }
}