using System;
using FMODUnity;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : Menu
    {
        [SerializeField] private EventReference pressSound;
        public void EnableMenu()
        {
            if (!Mathf.Approximately(Time.timeScale, 1f))
            {
                Debug.LogWarning("Cannot pause during slow motion.");
                return;
            }

            gameObject.SetActive(true);
            Time.timeScale = 0f;

            DisablePlayerInputMap();
            CoreManager.Instance.AudioManager.StopAllSounds();
        }

        public void Resume()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(pressSound, transform.position);
            gameObject.SetActive(false);
            Time.timeScale = 1f;

            // Disable UI input and re-enable gameplay input
            EnablePlayerInputMap();
        }


        public void ToMainMenu()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(pressSound, transform.position);

            Time.timeScale = 1f;
            gameObject.SetActive(false);
            ScenesManager.Instance.LoadMainMenu();
            
        }

        public void Restart()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(pressSound, transform.position);
            ScenesManager.Instance.ReloadCurrentScene();
            Resume();
        }
        
        
        private void EnablePlayerInputMap()
        {
           CoreManager.Instance.Player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
        private void DisablePlayerInputMap()
        {
            CoreManager.Instance.Player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

        }
    }
}
