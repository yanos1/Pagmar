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
    
    public SkeletonAnimation skeletonAnimation;
    private string currentActionAnimation = "";
    
    [Tooltip("If true, all animations are locked to idle + blink.")]
    [SerializeField] private bool _lockIdleState = false;

    private void Awake()
    {
        skeletonAnimation = skeletonAnimationYoung;

        // Set idle and blink on their own tracks
        skeletonAnimation.AnimationState.SetAnimation(0, "blink", true);
        skeletonAnimation.AnimationState.SetAnimation(1, "idlev2", true);

        // Setup animation blends (mixing)
        SetupMixes();
    }

    private void SetupMixes()
    {
        var stateData = skeletonAnimation.AnimationState.Data;

        stateData.SetMix("sleep", "wake-up", 1f);
        stateData.SetMix("wake-up", "wake-up-jump", 0f);
        stateData.SetMix("wake-up-jump", "idlev2", 0.2f);
    }


    public void PlayAnimation(string animationName,  int channel, bool loop = false, string fallbackAnimation = "idlev2",
        bool force = false, Action onComplete = null)
    {
        print($"attempt to play {animationName}");
        if (_lockIdleState) return;
        print($"attempt to play {animationName}1");

        if (string.IsNullOrEmpty(animationName)) return;
        print($"attempt to play {animationName}2");


        if (!force && currentActionAnimation == "jump-land" && animationName == "idlev2" && skeletonAnimation.AnimationState.GetCurrent(channel) != null)
            return;
        print($"attempt to play {animationName}3");

        if (!force && currentActionAnimation == animationName)
            return;      
        
        print($"trying to play {animationName}");

        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";

                if (animationName == "walljump-jump")
                {
                    skeletonAnimation.AnimationState.AddAnimation(channel, "walljump-air", true, 0f);
                }
                else if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(channel, fallbackAnimation, true, 0f);
                }

                onComplete?.Invoke();
            };
        }
    }
    
    public void QueueAnimation(string animationName, int track = 2, bool loop = false, float delay = 0f, string fallbackAnimation = "idlev2", Action onComplete = null)
    {
        if (_lockIdleState || string.IsNullOrEmpty(animationName)) return;

        var entry = skeletonAnimation.AnimationState.AddAnimation(track, animationName, loop, delay);

        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";

                if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(track, fallbackAnimation, true, 0f);
                }

                onComplete?.Invoke();
            };
        }
    }


    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = "idlev2", bool force = false, Action onComplete = null)
    {
        print($"attempt to play {animationName}");
        if (_lockIdleState) return;
        print($"attempt to play {animationName}1");

        if (string.IsNullOrEmpty(animationName)) return;
        print($"attempt to play {animationName}2");


        if (!force && currentActionAnimation == "jump-land" && animationName == "idlev2" && skeletonAnimation.AnimationState.GetCurrent(2) != null)
            return;
        print($"attempt to play {animationName}3");

        if (!force && currentActionAnimation == animationName)
            return;      
        
        print($"trying to play {animationName}");

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
        
        if (_lockIdleState) return;
        
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
        if(skeletonAnimation == null ) return;
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
    public void SetIdleLock(bool value)
    {
        _lockIdleState = value;

        if (value)
        {
            skeletonAnimation.AnimationState.ClearTrack(2);
            currentActionAnimation = "";
            skeletonAnimation.AnimationState.SetAnimation(0, "blink", true);
            skeletonAnimation.AnimationState.SetAnimation(1, "idlev2", true);
        }
    }

}