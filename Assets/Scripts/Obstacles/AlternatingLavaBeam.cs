using Interfaces;

namespace Obstacles
{
    using UnityEngine;

    public class AlternatingLavaBeam : MonoBehaviour, IKillPlayer
    {
        [SerializeField] private SpriteRenderer beamSprite;
        [SerializeField] private float interval = 3f;

        private void Start()
        {
            if (beamSprite == null)
                beamSprite = GetComponent<SpriteRenderer>();

            StartCoroutine(ToggleBeam());
        }

        private System.Collections.IEnumerator ToggleBeam()
        {
            while (true)
            {
                beamSprite.enabled = !beamSprite.enabled;
                yield return new WaitForSeconds(interval);
            }
        }

        public bool IsDeadly()
        {
            return true;
        }
    }

}