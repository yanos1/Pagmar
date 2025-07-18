using System;
using System.Collections;
using FMODUnity;
using Managers;
using SpongeScene;
using UnityEngine;

namespace Player
{
    public class PlayerTimelineActions : MonoBehaviour
    {
        private int force = 120;
        private int forceAddition = 60;
        private Rigidbody2D rb;
        private PlayerMovement _playerMovement;
        private int currentRotation = 0;
        [SerializeField] private GlobalActions _globalActions;
        [SerializeField] private GameObject explosionPrefab; 
        [SerializeField] private EventReference explodeSound;
        [SerializeField] private EventReference gaspSound;
        private int bumpCount;
        

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void BumpUp()
        {
            if(_playerMovement.jumpIsPressed) return;
            if (++bumpCount == 2 || bumpCount == 4)
            {
                print("be scared");
                GetComponent<SpineControl>().PlayAnimation("scared", force:true);
            }
            rb.AddForce(Vector2.up * force);
            force += forceAddition;
        }

        public void EndGameAnimationSequence() // unused
        {
            Rotate();
            // play  look down
            // play look up
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(4.5f, () => _globalActions.ShowCredits()));
        }
        public void Rotate()
        {
            var rotator = new Vector3(transform.rotation.x, currentRotation == 0 ? 180: 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

        public void Explode()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(explodeSound, transform.position);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        public void Gasp()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(gaspSound, transform.position);
        }

        public void TransitionComicsSounds()
        {
            StartCoroutine(PlayTransitionSounds());
        }

        private IEnumerator PlayTransitionSounds()
        {
            yield return new WaitForSecondsRealtime(2);
        }
    }
}