using Interfaces;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class WoodPlank : MonoBehaviour, IBreakable
    {
        [SerializeField] private WoodPlankHinged _woodPlank;
        [SerializeField] private HingeJoint2D hinge;
        [SerializeField] private Rigidbody2D rb;
        public void OnBreak()
        {
          // gameObject.SetActive(false);
        }

        public void OnHit(Vector2 hitDir, PlayerManager.PlayerStage stage)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            hinge.enabled = true;
            _woodPlank.ActivateHinge();
        }
    }
}