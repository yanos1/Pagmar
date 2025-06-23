using System;
using MoreMountains.Feedbacks;
using UnityEngine;

public class MirrorTrigger : MonoBehaviour
{

    [SerializeField] private MMF_Player enterFeedback;
    private bool hasActivated = false;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated) return; 
        enterFeedback?.PlayFeedbacks();
        hasActivated = true;

    }

    public void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
