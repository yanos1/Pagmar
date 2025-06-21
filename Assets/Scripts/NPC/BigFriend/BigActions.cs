using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC.BigFriend
{
    public class BigActions : MonoBehaviour
    {
        [SerializeField] private BigSpine bigSpine;
        [SerializeField] private GameObject bigVisuals;
        [SerializeField] private SpriteRenderer questionMarkRenderer;
        [SerializeField] private GameObject questionMark;
        [SerializeField] private GameObject healText;

        public void ShowQuestionMarkForSeconds(float duration = 3f)
        {
            questionMark.SetActive(true);

            Sequence seq = DOTween.Sequence();

            seq.Append(questionMarkRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutSine)) // Fade in
                .AppendInterval(duration) // Wait
                .Append(questionMarkRenderer.DOFade(0f, 0.3f).SetEase(Ease.InSine)) // Fade out
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
            healText.SetActive(false);
        }

        public void ShowBig()
        {
            bigVisuals.SetActive(true);
        }

        public void DoSmileAnim()
        {
            print("call do smile");
            bigSpine.DoSmile();
        }

        public void DoPonderAnim()
        {
         bigSpine.PlayAnimation(bigSpine.GetAnimName(BigSpine.SpineAnim.LookDownBack));   
        }

        public void EnableHeadCollider()
        {
            GetComponent<CircleCollider2D>().isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
        }

        public void DisableHeadCollideR()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
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