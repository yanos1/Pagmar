namespace MoreMountains.Feedbacks
{
    using MoreMountains.Feedbacks;
    using TMPro;
    using UnityEngine;

    [AddComponentMenu("")]
    public class MMF_TMPShaderSwitch : MMF_Feedback
    {
        public override float FeedbackDuration => 0f;

        [Tooltip("The target TextMeshPro object")]
        public TextMeshProUGUI TargetTMP;

        [Tooltip("The new shader to apply at runtime")]
        public Shader NewShader;

        protected Material _runtimeMaterial;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || TargetTMP == null || NewShader == null) return;

            // Clone material to avoid modifying shared material
            _runtimeMaterial = new Material(TargetTMP.fontMaterial);
            _runtimeMaterial.shader = NewShader;
            TargetTMP.fontMaterial = _runtimeMaterial;
        }
    }

}