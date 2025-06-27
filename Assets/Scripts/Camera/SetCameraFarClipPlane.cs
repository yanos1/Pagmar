using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Extensions/Custom Far Clip Plane Setter")]
    public class SetFarClipPlane : CinemachineExtension
    {
        public float farClip = 100f;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
            {
                var unityCam = UnityEngine.Camera.main;
                if (unityCam != null)
                    unityCam.farClipPlane = farClip;
            }
        }
    }
}