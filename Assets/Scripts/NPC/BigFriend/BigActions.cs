using System.Numerics;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private Collider2D headCol;
        [SerializeField] private MMF_Player healFeedbacks;

        public void ShowQuestionMarkForSeconds(float duration = 3f)
        {
            questionMark.SetActive(true);

            Sequence seq = DOTween.Sequence();

            seq.Append(questionMarkRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutSine)) // Fade in
                .AppendInterval(duration) // Wait
                .Append(questionMarkRenderer.DOFade(0f, 0.3f).SetEase(Ease.InSine)) // Fade out
                .OnComplete(() => questionMark.SetActive(false)); // Deactivate
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
            healText.SetActive(true);
        }
        
        public void RemoveHealRequest()
        {
            // enter aniamtion of happy
            healFeedbacks?.PlayFeedbacks();
            healText.SetActive(false);
        }

        public void ShowBig()
        {
            bigVisuals.SetActive(true);
        }

        public void DoSmileAnim(float seconds)
        {
            print("call do smile");
            bigSpine.DoSmile(seconds);
        }

        public void DoPonderAnim()
        {
         // bigSpine.PlayAnimation(bigSpine.GetAnimName(BigSpine.SpineAnim.LookDownBack));   
        }

        public void EnableHeadCollider()
        {
            headCol.enabled = true;
        }

        public void DisableHeadCollideR()
        {
            headCol.enabled = false;
        }

        public void Materialize()
        {
            GetComponent<CapsuleCollider2D>().isTrigger = false;
        }

        public void Unmeterialize()
        {
            GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
    }
}