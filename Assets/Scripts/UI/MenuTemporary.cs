using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTemporary : MonoBehaviour
{
    public static MenuTemporary Instance;

    [SerializeField]
    private TMPro.TMP_InputField m_login;
    [SerializeField]
    private TMPro.TMP_InputField m_password;

    //Need so that even ifthe player press the button multiple times it will only send one message
    private bool m_isAlreadyGoing = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleVisible = true;
        Debug.LogError("SHOW THE FUCKING COMMAND PROMPT");
    }

    public void SendLoginAndPassword()
    {
        if (m_isAlreadyGoing)
            return;

        m_isAlreadyGoing = true;

        string myjson = "{ \"login\" : \"" + m_login.text + "\", \"password\" : \"" + m_password.text + "\" }";
        NetworkClient.Instance.socketManagerRef.GetSocket().Emit("Loggin", myjson);
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

        NetworkClient.Instance.socketManagerRef.GetSocket().Emit("MainSceneLoaded");

        SceneManager.UnloadSceneAsync("Menu");
    }
}
