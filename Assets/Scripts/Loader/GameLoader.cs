using System.Collections;
using System.Collections.Generic;
using Camera;
using Managers;
using UnityEngine;

namespace Loader
{
    namespace SpongeScene.Loader
    {
        public class GameLoader : MonoBehaviour
        {
            [SerializeField] private GameLoaderUI loaderUI;
            [SerializeField] private ScenesManager sceneManager;

            private Animator animator;
            private float loadSpeed = 0.005f;

            private void Start()
            {
                // animator = GetComponent<Animator>();
                StartCoroutine(LoadGame());
                // animator.SetTrigger("Start");
            }

            private IEnumerator LoadGame()
            {
                yield return new WaitForSeconds(0.05f); // fixes rare bugs
                if (sceneManager is not null)
                {
                    sceneManager.LoadPersistentScene(() => loadSpeed /= 2f);
                }
                yield return StartCoroutine(StartLoading());
            }

            private IEnumerator StartLoading()
            {
                // animator.SetTrigger("Stop"); // Ensure you have a "Stop" trigger in your Animator

                while (loaderUI.IsNotFinished)
                {
                    loaderUI.AddProgress(1);
                    yield return new WaitForSeconds(loadSpeed);
                }

                sceneManager.LoadNextScene();
            }
        }
    }
}