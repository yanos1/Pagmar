using UnityEngine;

namespace Terrain
{
    public interface IBreakable
    {
        public void OnBreak();
        public void OnHit(Vector2 hitDir);
    }
}