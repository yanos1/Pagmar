using UnityEngine;

namespace Interfaces
{
    public interface IBreakable
    {
        public void OnBreak();
        public void OnHit(Vector2 hitDir);
    }
}