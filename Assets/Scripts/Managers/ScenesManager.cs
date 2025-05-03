using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class ScenesManager : MonoBehaviour
    {
        [Tooltip("The name of the persistent scene.")]
        public static ScenesManager Instance { get; private set; }
        
        private string persistentSceneName = "PersistentScene";
        private string loaderSceneName = "Loader";

        private int numScenes;

        private int currentSceneIndex = 1; // Tracks the index of the currently active scene
    
        private void RestartScene(object obj)
        {
            StartCoroutine(SwitchScene(currentSceneIndex));
        }

        private void Start()
        {
            Instance ??= this;
            DontDestroyOnLoad(gameObject);
            numScenes = SceneManager.sceneCountInBuildSettings;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                LoadNextScene();
            }
        }

        public void LoadPersistentScene(Action onComplete)
        {
            StartCoroutine(LoadPersistentSceneCoroutine(onComplete));
        }

        private IEnumerator LoadPersistentSceneCoroutine(Action onComplete)
        {
            if (!SceneManager.GetSceneByName(persistentSceneName).isLoaded)
            {
                AsyncOperation loadOp = SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);

                // Wait until scene is fully loaded
                while (!loadOp.isDone)
                {
                    yield return null;
                }
                print($"operation load op is done:{loadOp.isDone}");
                onComplete?.Invoke();
            }
        }


        public int LoadNextScene()
            {
                if (currentSceneIndex + 1 < numScenes)
                {
                    StartCoroutine(SwitchScene(currentSceneIndex + 1));
                }
                else
                {
                    Debug.LogWarning("No more scenes to load. You are at the last scene.");
                }

                return currentSceneIndex + 1;
            }

            public int ReloadCurrentScene()
            {
                StartCoroutine(SwitchScene(currentSceneIndex));
                return currentSceneIndex;
            }


            public void ReloadMainMenu()
            {
                StartCoroutine(SwitchScene(0));
            }

            private IEnumerator SwitchScene(int newSceneIndex)
            {
                if (newSceneIndex >= 0 && newSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    print($"current scene is {currentSceneIndex} {SceneManager.GetActiveScene().name}");

                    CoreManager.Instance.UiManager.ShowLoadingScreen();
                    AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
                    while (!loadOperation.isDone)
                    {
                        yield return null;
                    }

                    // Set the new scene as active
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newSceneIndex));
                }
                else
                {
                    Debug.LogWarning("Invalid scene index: " + newSceneIndex);
                    yield break;
                }

                if (currentSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentSceneIndex);
                    while (!unloadOperation.isDone)
                    {
                        yield return null;
                    }

                    CoreManager.Instance.UiManager.HideLoadingScreen();
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.StartNewScene, currentSceneIndex + 1);

                }

                currentSceneIndex = newSceneIndex;
                print($"current scene is {currentSceneIndex} {SceneManager.GetActiveScene()}");

            }
        }

        public enum SceneType
        {
            None = 0,
            Cutscene = 1,
            Level = 2,
            Menu = 3,
        }
    }