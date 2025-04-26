using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC.BigFriend
{
    public class BigActions : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bigSpriteRenderer;
        [SerializeField] private SpriteRenderer questionMarkRenderer;
        [SerializeField] private GameObject questionMark;
    
        public void ShowQuestionMarkForSeconds(float duration = 3f)
        {
            questionMark.SetActive(true);

            Sequence seq = DOTween.Sequence();

            seq.Append(questionMarkRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutSine)) // Fade in
                .AppendInterval(duration)                                            // Wait
                .Append(questionMarkRenderer.DOFade(0f, 0.3f).SetEase(Ease.InSine))  // Fade out
                .OnComplete(() => questionMark.SetActive(false));                    // Deactivate
        }

        public void ShowBig()
        {
            print("SHOW BIG111");
            var color = bigSpriteRenderer.color;
            color.a = 1f;
            bigSpriteRenderer.color = color;        
        }

        public void EnableHeadCollider()
        {
            GetComponent<CircleCollider2D>().isTrigger = false;
        }

        public void DisableHeadCollideR()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }
    }
}
