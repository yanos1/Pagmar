using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
using Terrain.Environment;
using Unity.Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CollapsingRoomManager : MonoBehaviour, IResettable
    {
        [SerializeField] private List<MMF_Player> collapsefeebacks;
        [SerializeField] private List<FallingStone> stones;
        private int currentIndex = 0;
        private List<FallingStone> remainingStones;

        public void InvokeNextFeedbacks()
        {
            collapsefeebacks[currentIndex]?.StopFeedbacks();
            collapsefeebacks[currentIndex++]?.PlayFeedbacks();
            if (currentIndex >= collapsefeebacks.Count)
            {
                print("activate stones");
                remainingStones = new List<FallingStone>(stones);
                StartCoroutine(ActivateRandomStones());
            }
        }

        private IEnumerator ActivateRandomStones()
        {
            while (remainingStones.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, remainingStones.Count);
                FallingStone selectedStone = remainingStones[randomIndex];

                selectedStone.Activate();
                remainingStones.RemoveAt(randomIndex);

                yield return new WaitForSeconds(1.2f);
            }
        }

        private void OnDestroy()
        {
            CinemachineImpulseManager.Instance.Clear(); // stop shaking of camera.
        }

        public void ResetToInitialState()
        {
            // Stop and reset all feedbacks
            foreach (var feedback in collapsefeebacks)
            {
                if (feedback != null)
                {
                    feedback.StopFeedbacks();
                    feedback.RestoreInitialValues(); // Resets feedbacks to initial state
                }
            }

            // Stop coroutine if it's running
            StopAllCoroutines();

            // Reset index
            currentIndex = 0;

            remainingStones = new List<FallingStone>(stones);

            // Optional: clear any camera impulses left hanging
            CinemachineImpulseManager.Instance.Clear();
        }

    }
}