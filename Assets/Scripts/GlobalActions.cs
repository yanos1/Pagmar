using System;
using Managers;
using MoreMountains.Feedbacks;
using UI;
using UnityEngine;

public class GlobalActions : MonoBehaviour
{

    [SerializeField] private MMF_Player slowMotionFeedbacks;
    [SerializeField] private UndergroundCanvas _canvas;

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.EnterSlowMotion, EnterSlowMotion);
    }

    
    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterSlowMotion, EnterSlowMotion);
    }
    public void AllowInput()
    {
        print("allow input !");
        CoreManager.Instance.GameManager.AlowCutSceneInput(); 
    }

    public void ShowFriendshipComics()
    {
        CoreManager.Instance.UiManager.OpenComics();
    }

    public void LoadNextScene()
    {
        ScenesManager.Instance.LoadNextScene();
    }

    public void ChangeAmbiance()
    {
        print("change ambience !");
        CoreManager.Instance.AudioManager.OnChangeAmbience(AmbienceType.Upperground);
    }

    public void EnterSlowMotion(object o)
    {
        print("enter slow motion");
        slowMotionFeedbacks?.PlayFeedbacks();
        EnableAttackEnemyText();
        AllowInput();
    }

    public void EnableAttackEnemyText()
    {
        _canvas.TurnOnTextForDuration();
    }
}