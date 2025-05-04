
using System;
using Camera;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    
    private Collider2D _collider2D;

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        Debug.Log("Enter");
        if(other.gameObject.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.GetInstance().PanCameraOnContact(customInspectorObjects.panDistance,customInspectorObjects.panTime,customInspectorObjects.panDirection,false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit");
        if(other.gameObject.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.GetInstance().PanCameraOnContact(customInspectorObjects.panDistance,customInspectorObjects.panTime,customInspectorObjects.panDirection,true);
            }
        }
    }
    
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineCamera cameraOnLeft;
    [HideInInspector] public CinemachineCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime     = 0.35f;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

[CustomEditor(typeof(CameraControlTrigger))]
[CanEditMultipleObjects] 
public class MyScriptEditors : Editor
{
    SerializedProperty swapCameras;
    SerializedProperty panCameraOnContact;
    SerializedProperty cameraOnLeft;
    SerializedProperty cameraOnRight;
    SerializedProperty panDirection;
    SerializedProperty panDistance;
    SerializedProperty panTime;

    private void OnEnable()
    {
        swapCameras         = serializedObject.FindProperty("customInspectorObjects.swapCameras");
        panCameraOnContact  = serializedObject.FindProperty("customInspectorObjects.panCameraOnContact");
        cameraOnLeft        = serializedObject.FindProperty("customInspectorObjects.cameraOnLeft");
        cameraOnRight       = serializedObject.FindProperty("customInspectorObjects.cameraOnRight");
        panDirection        = serializedObject.FindProperty("customInspectorObjects.panDirection");
        panDistance         = serializedObject.FindProperty("customInspectorObjects.panDistance");
        panTime             = serializedObject.FindProperty("customInspectorObjects.panTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(swapCameras);
        if (swapCameras.boolValue)
        {
            EditorGUILayout.PropertyField(cameraOnLeft, new GUIContent("Camera on Left"));
            EditorGUILayout.PropertyField(cameraOnRight, new GUIContent("Camera on Right"));
        }

        EditorGUILayout.PropertyField(panCameraOnContact);
        if (panCameraOnContact.boolValue)
        {
            EditorGUILayout.PropertyField(panDirection, new GUIContent("Camera Pan Direction"));
            EditorGUILayout.PropertyField(panDistance, new GUIContent("Pan Distance"));
            EditorGUILayout.PropertyField(panTime, new GUIContent("Pan Time"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
