using System;
using System.Linq;
using MoreMountains.Feedbacks;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private MMF_Player fadeInFeedbacks;
        [SerializeField] private MMF_Player fadeOutFeedbacks;
        [SerializeField] private MMF_Player openComicsFeedbacks;
        [SerializeField] private MMF_Player EnterCutSceneFeedbacks;
        [SerializeField] private MMF_Player ExitCutSceneFeedbacks;
        [SerializeField] private PauseMenu pauseMenu;


        private void OnEnable()
        {
            print("registered ui events");
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, OnEnterCutScene);
            CoreManager.Instance.EventManager.AddListener(EventNames.EndCutScene, OnEndCutScene);
            // CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, OnEndCutScene);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, OnEndCutScene);
            // CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, OnEndCutScene);

        }

        private void OnEnterCutScene(object obj)
        {
            print("open stripes");
            EnterCutSceneFeedbacks?.PlayFeedbacks();
        }

        
        public void OnEndCutScene(object obj)
        {
            print("close stripes");
            ExitCutSceneFeedbacks?.PlayFeedbacks();
        }
        
        public void ShowLoadingScreen(float duration = 0.4f)
        {
            SetCanvasGroupDuration(fadeInFeedbacks, duration);
            fadeInFeedbacks?.PlayFeedbacks();
        }

        public void HideLoadingScreen(float duration = 0.8f)
        {
            SetCanvasGroupDuration(fadeOutFeedbacks, duration);
            fadeOutFeedbacks?.PlayFeedbacks();
        }

        public bool IsFadeInFinished()
        {
            return !fadeInFeedbacks.IsPlaying;
        }

        private void SetCanvasGroupDuration(MMF_Player feedbacks, float duration)
        {
            if (feedbacks == null) return;

            foreach (var feedback in feedbacks.Feedbacks)
            {
                if (feedback.GetComponent<MMF_CanvasGroup>() is {} canvasGroupFeedback)
                {
                    print("found canvas group");
                    canvasGroupFeedback.Duration = duration;
                }
            }
        }

        public void OpenComics()
        {
            openComicsFeedbacks?.PlayFeedbacks();
        }


        public void OpenPauseMenu()
        {
            if (Time.timeScale < 0.9) return; // dont pause on slow motion
            pauseMenu.EnableMenu();
        }

        public void NavigatePauseMenu(InputAction.CallbackContext c)
        {
            pauseMenu.Navigate(c);
        }

        public void SelectButtonInPauseMenu(InputAction.CallbackContext c)
        {
            pauseMenu.Submit(c);
        }
    }
}