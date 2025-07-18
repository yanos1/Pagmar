using System;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GlobalActions : MonoBehaviour
{
    [SerializeField] private MMF_Player slowMotionFeedbacks;
    [SerializeField] private MMF_Player longslowMotionFeedbacks;
    [SerializeField] private UndergroundCanvas undergroundCanvas;
    [SerializeField] private MMF_Player endGameUiFeedbacks;
    [SerializeField] private MMF_Player creditsFeedbacks;

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.EnterSlowMotion, EnterSlowMotion);
        CoreManager.Instance.EventManager.AddListener(EventNames.PlayerMeetSmall, EnterEndGamePanel);
    }


    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterSlowMotion, EnterSlowMotion);
        CoreManager.Instance.EventManager.RemoveListener(EventNames.PlayerMeetSmall, EnterEndGamePanel);
    }

    public void EnterEndGamePanel(object obj)
    {
        endGameUiFeedbacks?.PlayFeedbacks();
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

    private void EnterSlowMotion(object o)
    {
        print("enter slow motion");
        slowMotionFeedbacks?.PlayFeedbacks();
        EnableAttackEnemyText();
        CoreManager.Instance.Player.EnableInput();
    }

    public void EnterLongSlowMotion()
    {
        longslowMotionFeedbacks?.PlayFeedbacks();
    }

    public void EnableAttackEnemyText()
    {
        undergroundCanvas.TurnOnTextForDuration();
    }

    public void EndCutScene(bool enableInput = true)
    {
        print("called end cutscne");
        CoreManager.Instance.EventManager.InvokeEvent(EventNames.EndCutScene, enableInput);
    }

    public void EnterCutScene()
    {
        CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, null);
    }

    public void FadeInScreen(float duration = 0.4f)
    {
        CoreManager.Instance.UiManager.ShowLoadingScreen(duration);
    }

    public void FadeOutScreen(float duration = 0.8f)
    {
        CoreManager.Instance.UiManager.HideLoadingScreen(duration);
    }

    public void OpenPauseMenu()
    {
        if(CoreManager.Instance.GameManager.InCutScene) return; // cant pause during cur scenes.
        CoreManager.Instance.UiManager.OpenPauseMenu();
    }

    public void NavigatePauseMenu(InputAction.CallbackContext c)
    {
        CoreManager.Instance.UiManager.NavigatePauseMenu(c);
        print("navigating pause menu");
    }

    public void SelectButtonInPauseMenu(InputAction.CallbackContext c)
    {
        CoreManager.Instance.UiManager.SelectButtonInPauseMenu(c);
        print("selecting pause menu");
    }

    public void ShowCredits()
    {
        print("show credits!");
        creditsFeedbacks?.PlayFeedbacks();
    }

    public void EnterNightAmbience()
    {
        CoreManager.Instance.AudioManager.SetGlobalParameter("Time", 1); // this changes ambience to night ambience
    }

    public void EnterEchoEarthQuakeAmbience()
    {
        CoreManager.Instance.AudioManager.SetGlobalParameter("CaveExited", 1); // this changes ambience to night ambience
    }
    
    public void StartForestMusic() {
        print("start forest music");
        CoreManager.Instance.AudioManager.SetMusicLocalParameter("giantarriving", 1);

    }
}