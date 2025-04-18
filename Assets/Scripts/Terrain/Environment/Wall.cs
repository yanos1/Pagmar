using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class Wall : MonoBehaviour, IBreakable
    {
        public void OnBreak()
        {
            gameObject.SetActive(false);
        }

        public void OnHit(Vector2 hitDir)
        {
            
        }
    }
}