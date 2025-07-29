using System;
using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using UnityEngine;

namespace Terrain.House
{
    public class House : MonoBehaviour, IResettable
    {
        private Vector3 startingPos;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private MMF_Player shakeFeedbacks;
        [SerializeField] private EventReference earthQuake;

        private void Start()
        {
            startingPos = transform.position;
            rb = GetComponent<Rigidbody2D>();
            shakeFeedbacks?.PlayFeedbacks();
        }

        public void ResetToInitialState()
        {
            shakeFeedbacks?.StopFeedbacks();
            transform.position = startingPos;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Mathf.Abs(CoreManager.Instance.Player.transform.position.x - transform.position.x) <
                40) // we are near house. 
            {
                print("play earthquake again!");
                shakeFeedbacks?.PlayFeedbacks();
                var sound = CoreManager.Instance.AudioManager.AddTemporalAmbience(AmbienceType.EarthQuake, earthQuake); // play earthquake sound
                CoreManager.Instance.AudioManager.RegisterSoundToStopWhenGoingToMainMenu(sound);

            }
            else
            {
                print("deistance too large dont player ambience agian");
            }
        }

        public void OnEndShake()
        {
            if (CoreManager.Instance.Player.IsDead) return; // avoids a bug
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}