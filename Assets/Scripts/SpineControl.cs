using System;
using System.Collections;
using Player;
using UnityEngine;
using Spine.Unity;

public class SpineControl : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimationYoung;
    [SerializeField] private SkeletonAnimation skeletonAnimationTeen;
    [SerializeField] private SkeletonAnimation skeletonAnimationAdult;
    private SkeletonAnimation skeletonAnimation;
    
    private string currentActionAnimation = "";

    private void Awake()
    {
        skeletonAnimation = skeletonAnimationYoung;
        
        skeletonAnimation.AnimationState.SetAnimation(0, "blink", true);
        
        skeletonAnimation.AnimationState.SetAnimation(1, "idlev2", true);
    }

    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = "idlev2", bool force = false, Action onComplete = null)
    {
        if (string.IsNullOrEmpty(animationName)) return;

        if (!force && currentActionAnimation == "jump-land" && animationName == "idlev2" && skeletonAnimation.AnimationState.GetCurrent(2) != null)
            return;

        if (!force && currentActionAnimation == animationName)
            return;

        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";

                if (animationName == "walljump-jump")
                {
                    skeletonAnimation.AnimationState.AddAnimation(2, "walljump-air", true, 0f);
                }
                else if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(2, fallbackAnimation, true, 0f);
                }

                onComplete?.Invoke();
            };
        }
    }



    
    public void PlayAnimationOnBaseTrack(string animationName, bool loop = false, string fallback = "idlev2") {
        if (string.IsNullOrEmpty(animationName)) return;
    
        var entry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        if (!loop) {
            entry.Complete += _ => {
                // when hit is done, go back to idle on track 0
                skeletonAnimation.AnimationState.SetAnimation(0, fallback, true);
            };
        }
    }
    
    public bool IsCurrentActionAnimationPlaying(string animationName)
    {
        var currentEntry = skeletonAnimation.AnimationState.GetCurrent(2);
        return currentActionAnimation == animationName && currentEntry != null && !currentEntry.IsComplete;
    }


    public void ClearActionAnimation()
    {
        currentActionAnimation = "";
        skeletonAnimation.AnimationState.ClearTrack(2);
    }
    
    public void changeSkelatonAnimation(PlayerStage playerStage)
    {
        Debug.Log($"Changing skeleton animation to {playerStage}");
        skeletonAnimation.gameObject.SetActive(false);
        switch (playerStage)
        {
            case PlayerStage.Young:
                skeletonAnimationYoung.gameObject.SetActive(true);
                skeletonAnimation = skeletonAnimationYoung;
                break;
            case PlayerStage.Teen:
                skeletonAnimationTeen.gameObject.SetActive(true);
                skeletonAnimation = skeletonAnimationTeen;
                break;
            case PlayerStage.Adult:
                skeletonAnimationAdult.gameObject.SetActive(true);
                skeletonAnimation = skeletonAnimationAdult;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(playerStage), playerStage, null);
        }
        
        skeletonAnimation.AnimationState.SetAnimation(0, "blink", true);
        skeletonAnimation.AnimationState.SetAnimation(1, "idlev2", true);
    }
}