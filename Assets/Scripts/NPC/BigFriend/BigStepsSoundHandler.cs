using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Managers;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace NPC.BigFriend
{
    public class BigStepsSoundHandler : MonoBehaviour
    {
        [SerializeField] private EventReference stepSound;
        [SerializeField] private Transform groundCheck;

        private Dictionary<int, EventInstance> materialSoundMap = new();
        private Coroutine stepCoroutine;
        private int currentMaterial = -1;
        private bool isPlaying = false;

        private void Start()
        {
            CreateStepSoundInstances();
        }

        private void OnDestroy()
        {
            foreach (var instance in materialSoundMap.Values)
            {
                instance.stop(STOP_MODE.IMMEDIATE);
                instance.release();
            }
        }

        public void HandleStepSounds()
        {
            int? material = GetCurrentGroundMaterial();
            print($"found material {material}");
            if (material == null || material == -1)
            {
                StopStepSounds();
                return;
            }

            if (!isPlaying || material != currentMaterial)
            {
                StopStepSounds();
                currentMaterial = material.Value;
                stepCoroutine = StartCoroutine(PlayStepsRepeatedly(currentMaterial));
                isPlaying = true;
            }
        }

        public void StopStepSounds()
        {
            if (stepCoroutine != null)
            {
                StopCoroutine(stepCoroutine);
                stepCoroutine = null;
            }

            isPlaying = false;
        }

        private IEnumerator PlayStepsRepeatedly(int material)
        {
            while (true)
            {
                // Wait until 0.633s to play first step
                yield return new WaitForSeconds(0.633f);
                PlayStep(material);
                
                // Wait the remaining time until end of animation for the second step
                yield return new WaitForSeconds(1.05f - 0.633f); // 0.417s
                PlayStep(material);

                // Loop again for the next cycle of animation
            }
        }

        private void PlayStep(int material)
        {
            print("play big step");
            EventInstance instance = CoreManager.Instance.AudioManager.CreateEventInstance(stepSound, "Material", material);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            instance.start();
            instance.release(); // let FMOD clean it up after it finishes
        }



        private void CreateStepSoundInstances()
        {
            for (int i = 0; i <= 1; i++)
            {
                EventInstance instance = CoreManager.Instance.AudioManager.CreateEventInstance(stepSound, "Material", i);

                instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

                materialSoundMap[i] = instance;
            }
        }

        private int? GetCurrentGroundMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position + Vector3.up*0.3f, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(groundCheck.position + Vector3.up * 0.3f, Vector2.down * 0.5f, hit.collider ? Color.magenta : Color.cyan, 3f);

            if (hit.collider == null)
            {
                print("found no ground");
                return null;

            }

            return hit.collider.tag switch
            {
                "LeavesAndDirt" => 0,
                "Wood" => 1,
                _ => -1
            };
        }
    }
}
