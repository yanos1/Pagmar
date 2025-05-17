using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Player;
using SpongeScene;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Obstacles.Shooters
{
    public class AlternatingLavaBeamcontroller : MonoBehaviour, IResettable
    {
        [SerializeField] private List<AlternatingLavaBeam> beams;

        [SerializeField] private float warningDuration = 2;

        [SerializeField] private SpriteRenderer[] warningRenderers;
        [SerializeField] private GameObject warningPrefab;
        private bool triggered = false;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is not null && !triggered)
            {
                StartBeams();
            }
        }

        private void StartBeams()
        {
            triggered = true;
            foreach (var beam in beams)
            {
                beam.StartBeam();
            }
        }

        public void ResetToInitialState()
        {
            triggered = false;
        }
    }
}