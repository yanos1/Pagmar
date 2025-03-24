using System.Collections.Generic;
using UnityEngine;

namespace Zipline
{
    public class ZiplineHandle : MonoBehaviour
    {
        public List<Transform> cableSegments; // List of cable segment positions
        public float baseSpeed = 3f; // Base movement speed

        private int currentSegmentIndex = 0;
        private bool isSliding = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isSliding)
            {
                isSliding = true;
            }

            if (isSliding)
            {
                MoveAlongCable();
            }
        }

        void MoveAlongCable()
        {
            if (currentSegmentIndex >= cableSegments.Count - 1)
            {
                isSliding = false; // Stop when reaching the end
                return;
            }

            Transform currentSegment = cableSegments[currentSegmentIndex];
            Transform nextSegment = cableSegments[currentSegmentIndex + 1];

            // Calculate direction & distance
            Vector2 direction = (nextSegment.position - currentSegment.position).normalized;
            float distance = Vector2.Distance(transform.position, nextSegment.position);

            // Adjust speed based on slope (steeper slopes = faster)
            float slope = Mathf.Abs(direction.y);
            float adjustedSpeed = baseSpeed + (slope * 5f); // Adjust speed based on gradient

            // Move handle
            transform.position = Vector2.MoveTowards(transform.position, nextSegment.position, adjustedSpeed * Time.deltaTime);

            // Check if we reached the next segment
            if (Vector2.Distance(transform.position, nextSegment.position) < 0.1f)
            {
                currentSegmentIndex++;
            }
        }
    }
}