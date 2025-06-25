namespace UI
{
    using MoreMountains.Feedbacks;
    using UnityEngine;

    using MoreMountains.Feedbacks;
    using UnityEngine;

    public class UISelectionFeedback : MonoBehaviour
    {
        [SerializeField] private MMF_Player feedbackPlayer;

        public void PlayFeedbackForTarget(Transform newTarget)
        {
            foreach (var feedback in feedbackPlayer.FeedbacksList)
            {
                if (!feedback.Active)
                {
                    continue;
                }

                // Try updating feedbacks with a known target field
                // Most animated feedbacks use a TargetTransform field
                var feedbackType = feedback.GetType();
                var targetField = feedbackType.GetField("AnimateScaleTarget");
                
                if (targetField != null)
                {
                    targetField.SetValue(feedback, newTarget);
                }

                // Add special cases for known feedbacks if needed:
                // e.g., if the field has a different name (like AnimatePositionTarget)
            }

            feedbackPlayer.PlayFeedbacks();
        }
    }


}