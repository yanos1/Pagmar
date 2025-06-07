
using System;
using UnityEngine;
using Spine.Unity;

public class EnemySpineControl : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    
    private string currentActionAnimation = "";
    
    [Tooltip("If true, all animations are locked to idle + blink.")]
    [SerializeField] private bool _lockIdleState = false;

    private void Awake()
    {
        // skeletonAnimation.AnimationState.SetAnimation(1, "Idle", true);
    }

    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = "Idle", bool force = false, Action onComplete = null)
    {
        Debug.Log($"Playing animation: {animationName}, Loop: {loop}, Force: {force}, Fallback: {fallbackAnimation}");
        if (_lockIdleState) return;
        
        if (string.IsNullOrEmpty(animationName)) return;

        if (!force && currentActionAnimation == animationName)
            return;

        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";

                if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(0, fallbackAnimation, true, 0f);
                }

                onComplete?.Invoke();
            };
        }
    }



    
    public void PlayAnimationOnBaseTrack(string animationName, bool loop = false, string fallback = "Idle") {
        
        if (_lockIdleState) return;
        
        if (string.IsNullOrEmpty(animationName)) return;
    
        var entry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        if (!loop) {
            entry.Complete += _ => {
                skeletonAnimation.AnimationState.SetAnimation(0, fallback, true);
            };
        }
    }
    
    public bool IsCurrentActionAnimationPlaying(string animationName)
    {
        var currentEntry = skeletonAnimation.AnimationState.GetCurrent(0);
        return currentActionAnimation == animationName && currentEntry != null && !currentEntry.IsComplete;
    }


    public void ClearActionAnimation()
    {
        currentActionAnimation = "";
        skeletonAnimation.AnimationState.ClearTrack(2);
    }
    
    public void SetIdleLock(bool value)
    {
        _lockIdleState = value;

        if (value)
        {
            skeletonAnimation.AnimationState.ClearTrack(0);
            currentActionAnimation = "";
            skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        }
    }
    public bool IsAnyNonLoopingAnimationPlaying()
    {
        var currentEntry = skeletonAnimation.AnimationState.GetCurrent(0);
        return currentEntry != null && !currentEntry.Loop && !currentEntry.IsComplete;
    }


}