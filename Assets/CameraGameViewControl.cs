using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Utils {
    
    [ExecuteInEditMode]
    public class CameraGameViewControl : MonoBehaviour{

        [SerializeField] private CinemachineVirtualCamera cam;
        [SerializeField] private float f = 1;
        
        [SerializeField] private float zoomF = 1;
        [SerializeField] private float minOrtho = 3;
        [SerializeField] private float maxOrtho = 4.5f;
        private float z = -10;
        
        private Vector3 originalPosition;
        private Vector2 originalMousePosition;
        private float originalOrtho;
        
        private void Awake(){
            if (Application.isPlaying){
                gameObject.SetActive(false);
            }
        }
        
        #if UNITY_EDITOR
        
        private void OnGUI() {
            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint) {
                UnityEditor.EditorUtility.SetDirty(this);
            } else if (Event.current.type == EventType.MouseDown) {
                DoMouseDown();
            }else if (Event.current.type == EventType.MouseDrag) {
                DoMouseDrag();
            }
        }

        private void DoScrollWheel() {
            cam.m_Lens.OrthographicSize += Event.current.delta.y * f;
        }

        private void DoMouseDrag() {
            if (Event.current.control) {
                cam.m_Lens.OrthographicSize += Event.current.delta.y * zoomF;
                cam.m_Lens.OrthographicSize = Mathf.Clamp(cam.m_Lens.OrthographicSize, minOrtho, maxOrtho);
                return;
            }
            
            var mousePos = Event.current.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            var delta = mousePos - originalMousePosition;
            cam.transform.position = originalPosition + new Vector3(-delta.x, -delta.y, 0) * (Event.current.shift ? f*2 : f);
        }

        private void DoMouseDown() {
            originalPosition = cam.transform.position;
            originalPosition.z = z;
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            originalMousePosition = mousePos;
        }
        
        
#endif
    }
}