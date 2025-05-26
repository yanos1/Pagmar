using System;
using System.Collections;
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

    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = "idle", bool force = false)
    {
        if (string.IsNullOrEmpty(animationName)) return;
        // Prevent idle from overriding "jump-land" if it's playing
        if (!force && currentActionAnimation == "jump-land" && animationName == "idle"&& skeletonAnimation.AnimationState.GetCurrent(2) != null)
            return;
        // Prevent same animation from restarting
        if (!force && currentActionAnimation == animationName)
            return;


        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";
                if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(2, fallbackAnimation, true, 0f);
                }
            };

        }

    }


    
    public void PlayAnimationOnBaseTrack(string animationName, bool loop = false, string fallback = "idle") {
        if (string.IsNullOrEmpty(animationName)) return;
    
        var entry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        if (!loop) {
            entry.Complete += _ => {
                // when hit is done, go back to idle on track 0
                skeletonAnimation.AnimationState.SetAnimation(0, fallback, true);
            };
        }
    }


    public void ClearActionAnimation()
    {
        currentActionAnimation = "";
        skeletonAnimation.AnimationState.ClearTrack(2);
    }
}