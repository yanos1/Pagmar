using System.Collections.Generic;
using UnityEngine;

namespace Zipline
{
    public class ZiplineRenderer : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public List<Transform> cableSegments = new List<Transform>(); // Holds all cable points

        void Start()
        {
            lineRenderer.positionCount = cableSegments.Count;
        }

        void Update()
        {
            UpdateCableLine();
        }

        void UpdateCableLine()
        {
            for (int i = 0; i < cableSegments.Count; i++)
            {
                lineRenderer.SetPosition(i, cableSegments[i].position);
            }
        }
    }
}