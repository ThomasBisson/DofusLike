using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTemporary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GoToMainScene()
    {
        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        StartCoroutine(WaitSceneIsLoaded());
    }

    IEnumerator WaitSceneIsLoaded()
    {
        Scene sceneHUD = SceneManager.GetSceneByName("HUD");
        Scene sceneMain = SceneManager.GetSceneByName("Main");
        yield return new WaitUntil(() => sceneHUD.isLoaded && sceneMain.isLoaded);

        SceneManager.SetActiveScene(sceneMain);

        SceneManager.UnloadSceneAsync("Menu");
    }
}
