using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UndergroundCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI attackEnemyText;
        [SerializeField] private MMF_Player attackEnemyTextFeedbacks;
        public void TurnOnTextForDuration()
        {
            print("enter text ");

            attackEnemyTextFeedbacks?.PlayFeedbacks();
        }
}
}