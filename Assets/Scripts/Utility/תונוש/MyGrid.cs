using UnityEngine;

namespace Utility.תונוש
{
    public class MyGrid : MonoBehaviour
    {
        [ExecuteAlways] public Color gridColor = Color.cyan;
        public int gridSizeX = 20;
        public int gridSizeY = 20;
        public float cellSize = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = gridColor;

            for (int x = 0; x <= gridSizeX; x++)
            {
                Vector3 start = transform.position + new Vector3(x * cellSize, 0, 0);
                Vector3 end = transform.position + new Vector3(x * cellSize, gridSizeY * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }

            for (int y = 0; y <= gridSizeY; y++)
            {
                Vector3 start = transform.position + new Vector3(0, y * cellSize, 0);
                Vector3 end = transform.position + new Vector3(gridSizeX * cellSize, y * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}