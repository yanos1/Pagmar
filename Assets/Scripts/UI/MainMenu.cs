using FMODUnity;
using Managers;
using UnityEngine;

namespace UI
{
    public class MainMenu : Menu
    {
        [SerializeField] private EventReference pressSound;
        [SerializeField] private EventReference pressStartSound;
        public void StartGame()
        {
            gameObject.SetActive(false);
            CoreManager.Instance.AudioManager.PlayOneShot(pressStartSound, transform.position);
            ScenesManager.Instance.LoadNextScene(1.8f);
        }

        public void ExitGame()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(pressSound, transform.position);
            Application.Quit();
        }
    }
}