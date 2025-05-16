using System;
using Interfaces;
using Terrain.Environment;

namespace Obstacles
{
    using UnityEngine;

    public class AlternatingLavaBeam : MonoBehaviour, IKillPlayer
    {
        [SerializeField] private SpriteRenderer beamSprite;
        [SerializeField] private float delayBeforeFirstBeam;
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
            yield return new WaitForSeconds(delayBeforeFirstBeam);
            while (true)
            {                
                yield return new WaitForSeconds(interval);
                beamSprite.enabled = !beamSprite.enabled;
                col.enabled = !col.enabled;
            }
        }

        public bool IsDeadly()
        {
            return true;
        }
        
    }

}