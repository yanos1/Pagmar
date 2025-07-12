using System;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

public class MirrorTrigger : MonoBehaviour
{

    [SerializeField] private MMF_Player enterFeedback;
    private bool hasActivated = false;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerManager>())
        {
            if (hasActivated) return; 
            enterFeedback?.PlayFeedbacks();
            hasActivated = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
