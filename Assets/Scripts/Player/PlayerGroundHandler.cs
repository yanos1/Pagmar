using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Managers;
using ScripableObjects;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Player
{
    public class PlayerGroundHandler : MonoBehaviour
    {
        private LayerMask groundLayer;
        private Dictionary<int, EventInstance> materialToSoundMap = new Dictionary<int, EventInstance>();
        [SerializeField] private PlayerSounds sounds;
        [SerializeField] private Transform groundCheck;
        private int currentGround = -1;
        private bool noSoundPlaying;
        
        private void Start()
        {
            groundLayer = LayerMask.NameToLayer("Ground");
            CreateEventInstancesForGroudMaterials();
        }
        
        public void HandleGroundSound()
        {
            var newGround = GetGroundMaterial();
            print($"new ground mat is {newGround}");
            if (currentGround != newGround || noSoundPlaying)
            { 
                if (currentGround != -1)
                {
                    print("stop sound but why");
                    materialToSoundMap[currentGround].stop(STOP_MODE.IMMEDIATE);
                }
                if (newGround is { } ground)
                {
                    currentGround = ground;
                    print($"current ground is {ground}");
                    materialToSoundMap[ground].start();
                    noSoundPlaying = false;
                    print($"start playinh sound of {currentGround}");
                }
            }
            else
            {
                print("do nothing");
            }
        }

        public void StopGroundSound()
        {
            print("stop sound?");
            noSoundPlaying = true;
            if(currentGround == -1) return;
            materialToSoundMap[currentGround].getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                print("stop sounds!");
                materialToSoundMap[currentGround].stop(STOP_MODE.IMMEDIATE);
            }
        }

        private void CreateEventInstancesForGroudMaterials()
        {
            for (int i=0; i <= 6 ;++i)
            {
                var eventInstance = CoreManager.Instance.AudioManager.CreateEveneInstance(sounds.walkSound, "Material", i);
                materialToSoundMap[i] = eventInstance;
            }
        }

        private int? GetGroundMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f);
            Debug.DrawRay(transform.position, Vector2.down * 0.1f, hit.collider? Color.green : Color.red);
            if (hit.collider is not null)
            {
                return GetMaterialIndex(hit.collider);
            }
            return null;
        }
        
        private int GetMaterialIndex(Collider2D collider)
        {
            if (collider.CompareTag("LeavesAndDirt")) return 0;
            if (collider.CompareTag("Rock")) return 1;
            if (collider.CompareTag("WeakRock")) return 2;
            if (collider.CompareTag("Metal")) return 3;
            if (collider.CompareTag("Bone")) return 4;
            if (collider.CompareTag("TreeBranches")) return 5;
            if (collider.CompareTag("Wood")) return 6;
            return -1;
        }
        
        
    }
}