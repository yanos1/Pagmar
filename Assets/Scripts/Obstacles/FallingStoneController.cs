using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Terrain.Environment;
using Unity.Cinemachine;

namespace Obstacles
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FallingStoneController : MonoBehaviour, IResettable, IBreakable
    {
        [SerializeField] private List<FallingStone> stones;
        [SerializeField] private Vector2 fallForce;
        [SerializeField] private EventReference hitSound;
        [SerializeField] private MMF_Player fb;

        private int currentIndex = 0;
        public void ResetToInitialState()
        {
            currentIndex = 0;
        }

        public void OnBreak()
        {
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            if(currentIndex >= stones.Count) {return;}
            CoreManager.Instance.AudioManager.PlayOneShot(hitSound,transform.position);
            fb?.PlayFeedbacks();
            FallingStone stone = stones[currentIndex];
            stone.Activate();
            stone.GetComponent<Rigidbody2D>().AddForce(fallForce, ForceMode2D.Impulse);
            currentIndex++;
        }
    }

}