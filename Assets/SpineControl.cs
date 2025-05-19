using System;
using UnityEngine;
using Spine.Unity;

public class SpineControl : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    
    private string currentActionAnimation = "";

    private void Awake()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "blink", true);
        
        skeletonAnimation.AnimationState.SetAnimation(1, "idle", true);
    }

    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = "idle")
    {
        if (string.IsNullOrEmpty(animationName)) return;
        if (currentActionAnimation == animationName) return;

        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";
                if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.SetEmptyAnimation(2, 0.1f); // Smooth blend-out
                }
            };
        }
    }


    public void ClearActionAnimation()
    {
        currentActionAnimation = "";
        skeletonAnimation.AnimationState.ClearTrack(2);
    }
}