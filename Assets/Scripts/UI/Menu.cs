using System;
using System.Collections.Generic;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventReference = FMODUnity.EventReference;

namespace UI
{
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] private EventReference selectedSound;
        [SerializeField] private EventReference pressSound;

        [SerializeField] private List<ButtonToAction> buttonsToActions;
        [SerializeField] private UISelectionFeedback selectButtonFeedbacks;
        [SerializeField] private UISelectionFeedback deselectButtonFeedbacks;

        private int selectedButtonIndex = 0;

        private float lastInputTime;
        private float lastSubmitTime;

        public float inputDelay;
        public float submitDelay;
        public float deadZone;

        private void Start()
        {
            if (buttonsToActions == null || buttonsToActions.Count == 0)
            {
                Debug.LogWarning("Menu has no buttons assigned!");
                return;
            }

            Debug.Log($"Menu button count: {buttonsToActions.Count}");

            selectedButtonIndex = 0;
            buttonsToActions[0].button.Select();
            selectButtonFeedbacks.PlayFeedbackForTarget(buttonsToActions[0].button.transform);
        }

      

        public void Navigate(InputAction.CallbackContext context)
        {
            if(!gameObject.activeInHierarchy) return;
            var direction = context.ReadValue<Vector2>();
            print($"navigating with {direction}");

            if (Time.unscaledTime - lastInputTime < inputDelay)
            {

                return;
            }


            if (Mathf.Abs(direction.x) > deadZone || Mathf.Abs(direction.y) > deadZone)
            {
                lastInputTime = Time.unscaledTime;
                if (direction.x > 0 || direction.y < 0)
                    ChangeSelection(1);
                else if (direction.x < 0 || direction.y > 0)
                    ChangeSelection(-1);
            }
        }

        public void Submit(InputAction.CallbackContext context)
        {
            if(!gameObject.activeInHierarchy) return;

            if (Time.unscaledTime - lastSubmitTime < submitDelay)
                return;

            lastSubmitTime = Time.unscaledTime;

            CoreManager.Instance.AudioManager.PlayOneShot(pressSound, transform.position);
            buttonsToActions[selectedButtonIndex].action.Invoke();
            print("pressing button!");

        }

        private void ChangeSelection(int direction)
        {
            if (buttonsToActions == null || buttonsToActions.Count == 0)
                return;
            print("selecting new button!");
            CoreManager.Instance.AudioManager.PlayOneShot(selectedSound, transform.position);
            deselectButtonFeedbacks.PlayFeedbackForTarget(buttonsToActions[selectedButtonIndex].button.transform);

            selectedButtonIndex += direction;

            if (selectedButtonIndex < 0)
            {
                selectedButtonIndex = (selectedButtonIndex % buttonsToActions.Count + buttonsToActions.Count) % buttonsToActions.Count;
            }
            else
            {
                selectedButtonIndex %= buttonsToActions.Count;
            }

            buttonsToActions[selectedButtonIndex].button.Select();
            selectButtonFeedbacks.PlayFeedbackForTarget(buttonsToActions[selectedButtonIndex].button.transform);
        }
    }

    [Serializable]
    public class ButtonToAction
    {
        public Button button;
        public UnityEvent action;
    }
}
