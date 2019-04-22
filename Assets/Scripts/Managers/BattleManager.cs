using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
    }

    public void SwitchToFightScene()
    {
        SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        StartCoroutine(WaitSceneIsLoaded());
    }

    IEnumerator WaitSceneIsLoaded()
    {
        //Start a loading screen and wait until the fight scene is loaded
        HUDUIManager.Instance.StartLoadScreen();
        Scene scene = SceneManager.GetSceneByName("Fight");
        yield return new WaitUntil(() => scene.isLoaded);

        //Create and set player fight from player main, also destroy player main
        GameObject player = GameManager.Instance.m_PlayerManagerMain.gameObject;
        var fight = player.AddComponent<PlayerManagerFight>();
        PlayerManager.PlayerMainToPlayerFight(GameManager.Instance.m_PlayerManagerMain, ref fight);
        GameManager.Instance.m_playerManagerFight = fight;
        DestroyImmediate(player.GetComponent<PlayerManagerMain>());

        //Stop load screen and unload main scene
        HUDUIManager.Instance.StopLoadScreen();
        SceneManager.UnloadSceneAsync("Main");
    }

}
