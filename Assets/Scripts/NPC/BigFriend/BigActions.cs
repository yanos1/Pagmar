using System.Numerics;
using DG.Tweening;
using FMODUnity;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

namespace NPC.BigFriend
{
    public class BigActions : MonoBehaviour
    {
        [SerializeField] private BigSpine bigSpine;
        [SerializeField] private GameObject bigVisuals;
        [SerializeField] private SpriteRenderer questionMarkRenderer;
        [SerializeField] private SpriteRenderer exclamationMarkRenderer;
        [SerializeField] private GameObject questionMark;
        [SerializeField] private GameObject exclamationMark;
        [SerializeField] private GameObject healText;
        [SerializeField] private GameObject headCol;
        [SerializeField] private MMF_Player healFeedbacks;
        [SerializeField] private MMF_Player sleepingHealRequestFeedbacks;
        [SerializeField] private MMF_Player landFeedbacks;
        [SerializeField] private EventReference approachSound;
        [SerializeField] private EventReference apearSound;
        [SerializeField] private EventReference ponderingSound;
        [SerializeField] private EventReference requestHealSound;

        public void ShowQuestionMarkForSeconds(float duration = 3f)
        {
            CoreManager.Instance.AudioManager.PlayOneShot(ponderingSound, transform.position + Vector3Int.right*8);
            bigSpine.PlayAnimation(bigSpine.GetAnimName(BigSpine.SpineAnim.HeadTilt));
            // questionMark.SetActive(true);
            
            // Sequence seq = DOTween.Sequence();
            //
            // seq.Append(questionMarkRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutSine)) // Fade in
            //     .AppendInterval(duration) // Wait
            //     .Append(questionMarkRenderer.DOFade(0f, 0.3f).SetEase(Ease.InSine)) // Fade out
            //     .OnComplete(() => questionMark.SetActive(false)); // Deactivate
        }
        
        public void ShowExclamationMarkForSeconds(float duration = 3f)
        {
            exclamationMark.SetActive(true);

            Sequence seq = DOTween.Sequence();

            seq.Append(exclamationMarkRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutSine)) // Fade in
                .AppendInterval(duration) // Wait
                .Append(exclamationMarkRenderer.DOFade(0f, 0.3f).SetEase(Ease.InSine)) // Fade out
                .OnComplete(() => questionMark.SetActive(false)); // Deactivate
        }

        public void ShowHealRequest()
        {
            // enter animation of tired.
            bigSpine.PlayAnimation(bigSpine.GetAnimName(BigSpine.SpineAnim.Tired), loop: true);
            CoreManager.Instance.AudioManager.PlayOneShot(requestHealSound, transform.position);
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(2, () => healText.SetActive(true)));
        }
        
        public void RemoveHealRequest()
        {
            // enter aniamtion of happy
            healFeedbacks?.PlayFeedbacks();
            healText.SetActive(false);
        }

        public void ShowBig()
        {
            print("show big!");
            bigVisuals.SetActive(true);
            Materialize();
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.3f, () =>
            {
                print("playing appear sound");
                CoreManager.Instance.AudioManager.PlayOneShot(apearSound, transform.position + Vector3Int.right*8);
            }));
        }
        
        public void DoSmileAnim(float seconds)
        {
            print("call do smile");
            bigSpine.DoSmile(seconds);
        }
        public void EnableHeadCollider()
        {
            headCol.SetActive(true);
        }

        public void DisableHeadCollideR()
        {
            headCol.SetActive(false);
            Unmeterialize();
        }

        public void Materialize()
        {
            GetComponent<CapsuleCollider2D>().isTrigger = false;
        }

        public void Unmeterialize()
        {
            GetComponent<CapsuleCollider2D>().isTrigger = true;
        }

        public void ShowSleepHeallRequest()
        {
            // CoreManager.Instance.AudioManager.PlayOneShot(requestHealSound, transform.position);
            sleepingHealRequestFeedbacks?.PlayFeedbacks();
        }

        public void PlayApproachSound()
        {
            print("playing Approach sound");

            CoreManager.Instance.AudioManager.PlayOneShot(approachSound, transform.position + Vector3Int.right*8);
        }

        public void PlayLandFeedbacks()
        {
            landFeedbacks?.PlayFeedbacks();
        }
    }
}