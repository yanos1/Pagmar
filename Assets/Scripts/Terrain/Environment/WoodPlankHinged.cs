using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class WoodPlankHinged : MonoBehaviour
    {
        [SerializeField] private HingeJoint2D hinge;
        [SerializeField] private Rigidbody2D rb;
        
        public void ActivateHinge()
        {
            if (hinge != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                hinge.enabled = true;
            }
        }
    }
}