using Managers;
using UnityEngine;

namespace UI
{
    public class MainMenu : Menu
    {

        public void StartGame()
        {
            gameObject.SetActive(false);
            ScenesManager.Instance.LoadNextScene();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}