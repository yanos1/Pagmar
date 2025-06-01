using MoreMountains.Feedbacks;
using UnityEngine;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private MMF_Player fadeInFeedbacks;
        [SerializeField] private MMF_Player fadeOutFeedbacks;
        [SerializeField] private MMF_Player openComicsFeedbacks;

        public void ShowLoadingScreen()
        {
            fadeInFeedbacks?.PlayFeedbacks();
        }

        public bool IsFadeInFinished()
        {
            return !fadeInFeedbacks.IsPlaying;
        }
        public void HideLoadingScreen()
        {            
            fadeOutFeedbacks?.PlayFeedbacks();
        }

        public void OpenComics()
        {
            openComicsFeedbacks?.PlayFeedbacks();
        }
        
        
    }
}