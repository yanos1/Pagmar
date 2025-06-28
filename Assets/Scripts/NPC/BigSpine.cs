using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
public class BigSpine : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    
    private string currentActionAnimation = "";

    private const string _blink = "blink";
    private const string _crouch = "crouch";
    private const string _gettingUp = "getting up";
    private const string _dash = "dashv2";
    private const string _idle = "idlev2";
    private const string _jump = "jumpv3";
    private const string _jumpAir = "jump-air";
    private const string _jumpLand = "jump-land";
    private const string _lookDownBack = "look down back";
    private const string _run = "run";
    private const string _smile = "smile";
    private const string _walk = "walk2";

    public enum SpineAnim
    {
        Idle, Walk, Run, Jump, JumpAir, JumpLand, Crouch, Dash, Blink, GettingUp, LookDownBack, Smile,
        Sleeping
    }

    public string GetAnimName(SpineAnim anim)
    {
        return anim switch
        {
            SpineAnim.Idle => _idle,
            SpineAnim.Walk => _walk,
            SpineAnim.Run => _run,
            SpineAnim.Jump => _jump,
            SpineAnim.JumpAir => _jumpAir,
            SpineAnim.JumpLand => _jumpLand,
            SpineAnim.Crouch => _crouch,
            SpineAnim.Dash => _dash,
            SpineAnim.Blink => _blink,
            SpineAnim.GettingUp => _gettingUp,
            SpineAnim.LookDownBack => _lookDownBack,
            SpineAnim.Smile => _smile,
            _ => _idle
        };
    }
    public void Awake()
    {
        PlayBlinkLoop();
    }
    private void PlayBlinkLoop()
    {
        var blinkEntry = skeletonAnimation.AnimationState.SetAnimation(0, _blink, true);
        blinkEntry.MixDuration = 0.2f; // Smooth blend
    }

    public void PlayAnimation(string animationName, bool loop = false, string fallbackAnimation = _idle, bool force = false, Action onComplete = null)
    {
        Debug.Log(animationName);
        if (string.IsNullOrEmpty(animationName)) return;

        if (!force && currentActionAnimation == animationName)
        {
            print("returned witout animation");
            return;

        }

        currentActionAnimation = animationName;

        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);
        if (!loop)
        {
            entry.Complete += _ =>
            {
                currentActionAnimation = "";
                print($"finished {animationName}");
                if (!string.IsNullOrEmpty(fallbackAnimation))
                {
                    skeletonAnimation.AnimationState.AddAnimation(2, fallbackAnimation, true, 0f);
                }
                onComplete?.Invoke();
            };
        }
    }

    public void DoSmile(float seconds)
    {
        print("big smile called");
        StartCoroutine(SmileForSeconds(seconds));
    }

    private IEnumerator SmileForSeconds(float duration)
    {
        // Play the smile animation on track 1
        skeletonAnimation.AnimationState.SetAnimation(1, _smile, true); // looped to hold smile pose

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Go back to blinking or clear the smile
        skeletonAnimation.AnimationState.SetAnimation(1, _blink, true);
    }




    
    public void PlayAnimationOnBaseTrack(string animationName, bool loop = false, string fallback = _idle) {
        
        
        if (string.IsNullOrEmpty(animationName)) return;
    
        var entry = skeletonAnimation.AnimationState.SetAnimation(2, animationName, loop);
        if (!loop) {
            entry.Complete += _ => {
                skeletonAnimation.AnimationState.SetAnimation(2, fallback, true);
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
    
    
    public bool IsAnyNonLoopingAnimationPlaying()
    {
        var currentEntry = skeletonAnimation.AnimationState.GetCurrent(2);
        return currentEntry != null && !currentEntry.Loop && !currentEntry.IsComplete;
    }
}
