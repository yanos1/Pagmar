using Interfaces;

namespace Obstacles
{
    using UnityEngine;

    public class AlternatingLavaBeam : MonoBehaviour, IKillPlayer
    {
        [SerializeField] private SpriteRenderer beamSprite;
        
        [SerializeField] private float interval = 3f;
        private Collider2D col;
        private void Start()
        {
            if (beamSprite == null)
                beamSprite = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();

            StartCoroutine(ToggleBeam());
        }

        private System.Collections.IEnumerator ToggleBeam()
        {
            while (true)
            {
                beamSprite.enabled = !beamSprite.enabled;
                col.enabled = !col.enabled;
                yield return new WaitForSeconds(interval);
            }
        }

        public bool IsDeadly()
        {
            return true;
        }
    }

}