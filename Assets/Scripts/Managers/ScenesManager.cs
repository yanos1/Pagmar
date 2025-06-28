using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class ScenesManager : MonoBehaviour
    {
        [Tooltip("The name of the persistent scene.")]
        public static ScenesManager Instance { get; private set; }

        [SerializeField] private List<SceneData> scenes;

        private string persistentSceneName = "PersistentScene";
        private string loaderSceneName = "Loader";

        private int numScenes;

        private int currentSceneIndex = 1; // Tracks the index of the currently active scene

        private void RestartScene(object obj)
        {
            var doFade = IsNextSceneGameplayScene();
            StartCoroutine(SwitchScene(currentSceneIndex, doFade));
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
                CoreManager.Instance.UiManager.OnEndCutScene(null); // close cutscene stripes
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadCurrentScene();
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

        
        public void LoadMainMenu()
        {
            StartCoroutine(SwitchToMainMenu());
        }

        private IEnumerator SwitchToMainMenu()
        {
            const int mainMenuIndex = 2;
            
            CoreManager.Instance.UiManager.ShowLoadingScreen();
            while (!CoreManager.Instance.UiManager.IsFadeInFinished())
            {
                yield return null;
            }

            // Disable input before loading scene
            if (CoreManager.Instance.Player is not null)
            {
                CoreManager.Instance.Player.GetComponent<PlayerInput>()?.gameObject.SetActive(false);
            }
            else
            {
                FindAnyObjectByType<Canvas>()?.GetComponentInChildren<PlayerInput>()?.gameObject.SetActive(false);
            }
            

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(mainMenuIndex, LoadSceneMode.Additive);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(mainMenuIndex));

            if (currentSceneIndex >= 0 && currentSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneIndex);
                while (!unloadOp.isDone)
                {
                    yield return null;
                }
            }

            
            CoreManager.Instance.UiManager.HideLoadingScreen();
            
            currentSceneIndex = mainMenuIndex;
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.StartNewScene, mainMenuIndex);
        }

        public int LoadNextScene()
        {
            if (currentSceneIndex + 1 < numScenes)
            {
                var doFade = IsNextSceneGameplayScene();
                StartCoroutine(SwitchScene(currentSceneIndex + 1, doFade));
            }
            else
            {
                Debug.LogWarning("No more scenes to load. You are at the last scene.");
            }
        
            return currentSceneIndex + 1;
        }

        private bool IsNextSceneGameplayScene()
        {
            bool doFade = false;
            var sceneData = scenes.Find(scene => scene.sceneNumber == currentSceneIndex + 1);
            SceneType nextSceneType = sceneData != null ? sceneData.SceneType : SceneType.None;
            if (nextSceneType == SceneType.Gameplay)
            {
                doFade = true;
            }

            return doFade;
        }
        
        public int ReloadCurrentScene()
        {
            var doFade = IsNextSceneGameplayScene();

            StartCoroutine(SwitchScene(currentSceneIndex, doFade));
            return currentSceneIndex;
        }

        private IEnumerator SwitchScene(int newSceneIndex, bool doFade, float delayBetweenFades = 0)
        {
            if (newSceneIndex >= 0 && newSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
            
                print($"current scene is {currentSceneIndex} {SceneManager.GetActiveScene().name}");
                if (doFade)
                {
                    CoreManager.Instance.UiManager.ShowLoadingScreen();
                    while (!CoreManager.Instance.UiManager.IsFadeInFinished())
                    {
                        yield return null;
                    }
                    CoreManager.Instance.AudioManager.StopAllSounds();

                    if (CoreManager.Instance.Player != null)
                    {
                        CoreManager.Instance.Player.GetComponent<PlayerInput>()?.gameObject
                            .SetActive(false); // fixes a bug in in input system
                    }
                    else
                    {
                        FindAnyObjectByType<Canvas>().GetComponentInChildren<PlayerInput>()?.gameObject.SetActive(false);
                        // for a case there is no player in scene, and the input sits on a ui eleemnt,
        
                    }
                }
                
                CoreManager.Instance.GameManager.OnEndCutScene(null); // returning input if was disabled at the end of the scene.
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
                AsyncOperation unloadOperation;
                if (currentSceneIndex == 1)  // this avoids deleting the persistent scene.
                {
                    unloadOperation = SceneManager.UnloadSceneAsync(0);
                }
                else
                {
                    unloadOperation = SceneManager.UnloadSceneAsync(currentSceneIndex);

                }
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(delayBetweenFades);
                if (doFade)
                {
                    CoreManager.Instance.UiManager.HideLoadingScreen();

                }
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
        Gameplay = 2,
        Menu = 3,
    }

    [Serializable]
    class SceneData
    {
        public int sceneNumber;
        public SceneType SceneType;
    }
}