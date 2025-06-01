using Managers;
using UnityEngine;

public class GlobalActions : MonoBehaviour
{
    public void AllowInput()
    {
        CoreManager.Instance.GameManager.AlowCutSceneInput(); 
    }

    public void ShowFriendshipComics()
    {
        CoreManager.Instance.UiManager.OpenComics();
    }

    public void LoadNextScene()
    {
        ScenesManager.Instance.LoadNextScene();
    }

}