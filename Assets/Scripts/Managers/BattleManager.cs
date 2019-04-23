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

    public void SwitchToFightScene(NetworkIdentity groupNI)
    {
        SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        StartCoroutine(WaitSceneIsLoaded(groupNI));
    }

    IEnumerator WaitSceneIsLoaded(NetworkIdentity groupNI)
    {
        //Get EnnemyGroupMain
        var groupMain = groupNI.GetComponent<EnnemyGroupMain>();

        //Start a loading screen and wait until the fight scene is loaded
        HUDUIManager.Instance.StartLoadScreen();
        Scene scene = SceneManager.GetSceneByName("Fight");
        yield return new WaitUntil(() => scene.isLoaded);

        SceneManager.SetActiveScene(scene);

        //Create and set player fight from player main, also destroy player main
        GameObject player = GameManager.Instance.m_PlayerManagerMain.gameObject;
        var fight = player.AddComponent<PlayerManagerFight>();
        PlayerManager.PlayerMainToPlayerFight(GameManager.Instance.m_PlayerManagerMain, ref fight);
        GameManager.Instance.m_playerManagerFight = fight;
        DestroyImmediate(player.GetComponent<PlayerManagerMain>());
        player.GetComponent<NetworkTransform>().m_playerManager = fight;


        //Set ennemies
        var parent = new GameObject().transform;
        parent.name = groupNI.transform.name;
        EnnemyGroupFight groupFight = parent.gameObject.AddComponent<EnnemyGroupFight>();
        groupFight.m_HUDUIManager = groupMain.m_HUDUIManager;
        NetworkIdentity niGroup = parent.gameObject.AddComponent<NetworkIdentity>();
        niGroup.SetControllerID(groupNI.GetID());
        niGroup.SetSocketReference(groupNI.GetSocket());
        groupNI.GetSocket().ModifyNetworkIdentityOfNetworkObject(groupNI.GetID(), niGroup);
        groupFight.m_networkIdentity = niGroup;


        foreach (var ennemy in groupMain.m_ennemiesMain)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + ennemy.m_ennemyStats.m_name, typeof(GameObject)), parent);
            go.name = ennemy.transform.name;
            EnnemyManagerFight ennemyFight = go.AddComponent<EnnemyManagerFight>();

            //Network
            NetworkIdentity newNI = go.GetComponent<NetworkIdentity>();
            NetworkIdentity oldNI = ennemy.GetComponent<NetworkIdentity>();
            newNI.SetControllerID(oldNI.GetID());
            newNI.SetSocketReference(oldNI.GetSocket());
            newNI.GetSocket().ModifyNetworkIdentityOfNetworkObject(oldNI.GetID(), oldNI);

            //EnnemyManagerFight
            EnnemyManager.EnnemyMainToEnnemyFight(ennemy, ref ennemyFight);

            groupFight.AddToEnnemyGroup(ennemyFight);
        }


        //Stop load screen and unload main scene
        HUDUIManager.Instance.StopLoadScreen();
        SceneManager.UnloadSceneAsync("Main");
    }

}
