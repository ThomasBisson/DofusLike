using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTemporary : MonoBehaviour
{
    private bool m_isAlreadyGoing = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleVisible = true;
        Debug.LogError("SHOW THE FUCKING COMMAND PROMPT");
    }

    public void GoToMainScene()
    {
        if (m_isAlreadyGoing)
            return;

        m_isAlreadyGoing = true;

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
