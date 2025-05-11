using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility.תונוש
{
    public class DebugGameScene : MonoBehaviour
    {
        [SerializeField] private string persistentSceneName = "PersistentScene";

        private void Awake()
        {
            if (!IsSceneLoaded(persistentSceneName))
            {
                SceneManager.LoadScene(persistentSceneName, LoadSceneMode.Additive);
                StartCoroutine(A());
                Debug.Log($"[DebugGameScene] Loaded missing persistent scene: {persistentSceneName}");
            }
        }

        private IEnumerator A()
        {
            yield return new WaitForSeconds(1);
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.StartNewScene, null);
        }

        private bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}