using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class Wall : MonoBehaviour, IBreakable, IResettable
    {
        public void OnBreak()
        {
            gameObject.SetActive(false);
        }

        public void OnHit(Vector2 hitDir)
        {
            OnBreak();
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
        }
    }
}