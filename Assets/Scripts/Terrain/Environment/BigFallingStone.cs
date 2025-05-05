namespace Terrain.Environment
{
 using System;
 using Interfaces;
 using MoreMountains.Feedbacks;
 using Player;
 using UnityEngine;
 
 namespace Terrain.Environment
 {
     public class BigFallingStone : FallingStone
     {
         [SerializeField] private Vector2 rollForce;
         
         private bool isDeadly = true;
         public override void  OnCollisionEnter2D(Collision2D other)
         {
             if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
             {
                 // landFeedBacks?.PlayFeedbacks();
                 isDeadly = false;
                 rb.linearVelocity = Vector2.right * rollForce;

             }

             if (other.gameObject.GetComponent<IBreakable>() is { } breakable)
             {
                 breakable.OnHit(Vector2.right);
                 rb.linearVelocity = Vector2.zero;
                 print(rb.linearVelocity + " linear vel 447");
                 rb.angularVelocity = 0f;
                 rb.AddForce(rollForce*7);

             }
         }
 
         public void HitPlayer()
         {
             // landFeedBacks?.PlayFeedbacks();
         }
 
         public override bool IsDeadly()
         {
             print($"445 kill player : {isDeadly== true} magnitude {rb.linearVelocity.magnitude}");
             return isDeadly || rb.linearVelocity.magnitude > 1;
         }

         public override void ResetToInitialState()
         {
             base.ResetToInitialState();
             rb.angularVelocity = 0f;
             rb.linearVelocity = Vector2.zero;

             print("reset big stone");
         }
     }
 }
}