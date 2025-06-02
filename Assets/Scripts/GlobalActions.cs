using Managers;
using UnityEngine;

public class GlobalActions : MonoBehaviour
{
    public void AllowInput()
    {
        print("allow input !");
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

    public void ChangeAmbiance()
    {
        print("change ambience !");
        CoreManager.Instance.AudioManager.OnChangeAmbience(AmbienceType.Upperground);
    }

}