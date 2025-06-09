using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Managers;
using ScripableObjects;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Player
{
    public class PlayerSoundHandler : MonoBehaviour
    {
        private Dictionary<int, EventInstance> materialToSoundMap = new();
        [SerializeField] private PlayerSounds sounds;
        [SerializeField] private Transform groundCheck;


        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.Die, StopGroundSound);
        }

        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.Die, StopGroundSound);
        }

        private int currentGround = -1;
        private Coroutine soundCoroutine;
        private bool isPlaying = false;

        private void Start()
        {
            CreateEventInstancesForGroundMaterials();
        }

        public void HandleGroundSound()
        {
            int newGround = GetGroundMaterial() ?? -1;

            if (newGround == -1)
            {
                StopGroundSound();
                return;
            }

            bool isSameGround = (newGround == currentGround);
            bool shouldRestart = !isSameGround || soundCoroutine == null;

            if (shouldRestart)
            {

                StopGroundSound(); // Ensure previous sound stops
                currentGround = newGround;
                soundCoroutine = StartCoroutine(PlayGroundSoundRepeatedly(currentGround));
                isPlaying = true;
            }
            else
            {
            }
        }

        public void StopGroundSound(object o = null)
        {

            if (soundCoroutine != null)
            {
                StopCoroutine(soundCoroutine);
                soundCoroutine = null;
            }
            else
            {
            }

            isPlaying = false;
        }

        private IEnumerator PlayGroundSoundRepeatedly(int ground)
        {

            while (true)
            {
                if (!materialToSoundMap.ContainsKey(ground))
                {
                    yield break;
                }

                var instance = materialToSoundMap[ground];
                instance.start();

                yield return new WaitForSeconds(0.23f);
            }
        }

        private void CreateEventInstancesForGroundMaterials()
        {
            for (int i = 0; i <= 6; ++i)
            {
                materialToSoundMap[i] = CoreManager.Instance.AudioManager.CreateEveneInstance(sounds.walkSound, "Material", i);
            }
        }
        
        private int? GetGroundMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f);
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.1f, hit.collider ? Color.green : Color.red);

            if (hit.collider != null)
            {
                int materialIndex = GetMaterialIndex(hit.collider);
                return materialIndex;
            }

            return null;
        }

        private int GetMaterialIndex(Collider2D collider)
        {
            return collider.tag switch
            {
                "LeavesAndDirt" => 0,
                "Rock" => 1,
                "WeakRock" => 2,
                "Metal" => 3,
                "Bone" => 4,
                "TreeBranches" => 5,
                "Wood" => 6,
                _ => -1
            };
        }
    }
}
