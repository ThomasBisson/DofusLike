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
        HUDUIManager.Instance.StartLoadScreen();
        Scene scene = SceneManager.GetSceneByName("Fight");
        yield return new WaitUntil(() => scene.isLoaded);
        GameObject player = GameManager.Instance.m_PlayerManagerMain.gameObject;
        var fight = player.AddComponent<PlayerManagerFight>();
        fight = PlayerManager.PlayerMainToPlayerFight(GameManager.Instance.m_PlayerManagerMain);
        GameManager.Instance.m_playerManagerFight = fight;
        DestroyImmediate(player.GetComponent<PlayerManagerMain>());
        HUDUIManager.Instance.StopLoadScreen();
        SceneManager.UnloadSceneAsync("Main");
    }

}
