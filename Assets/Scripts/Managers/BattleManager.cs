﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private HUDUIManager m_HUDManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        m_HUDManager = HUDUIManager.Instance;
    }

    public void SwitchToFightScene(NetworkIdentity groupNI)
    {
        SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        StartCoroutine(WaitSceneIsLoaded(groupNI));
    }

    IEnumerator WaitSceneIsLoaded(NetworkIdentity groupNI)
    {
        //Get EnnemyGroupMain
        var groupMain = groupNI.GetComponent<EnnemyGroup>();

        //Start a loading screen and wait until the fight scene is loaded
        m_HUDManager.StartLoadScreen();
        Scene scene = SceneManager.GetSceneByName("Fight");
        yield return new WaitUntil(() => scene.isLoaded);

        SceneManager.SetActiveScene(scene);

        //Set Player to fight mode
        GameManager.Instance.m_playerManager.ChangeStrategy(PlayerManager.PossibleStrategy.Fight);
        GameManager.Instance.m_playerManager.SubscribeToNewTurnEvents(m_HUDManager.SwitchableMana.SwitchableF.MakeLeftTopIconAppear);


        //Set ennemies
        var parent = new GameObject().transform;
        parent.name = groupNI.transform.name;
        EnnemyGroup groupFight = parent.gameObject.AddComponent<EnnemyGroup>();
        groupFight.Position = new Vector2(0,0);
        groupFight.ActivateFightMode();



        NetworkIdentity niGroup = parent.gameObject.AddComponent<NetworkIdentity>();
        niGroup.SetControllerID(groupNI.GetID());
        niGroup.SetSocketReference(groupNI.GetSocket());
        groupNI.GetSocket().ModifyNetworkIdentityOfNetworkObject(groupNI.GetID(), niGroup);


        foreach (var ennemy in groupMain.Ennemies)
        {
            ennemy.gameObject.SetActive(true);
            ennemy.transform.parent = parent;
            ennemy.ChangeStrategy(EnnemyManager.PossibleStrategy.Fight);
            ennemy.SubscribeToNewTurnEvents(m_HUDManager.SwitchableMana.SwitchableF.MakeLeftTopIconAppear);
            groupFight.AddToEnnemyGroup(ennemy);
        }


        //Activate HUD for battle
        m_HUDManager.SwitchableMana.SwitchToFight(GameManager.Instance.m_playerManager, groupFight);


        //Stop load screen and unload main scene
        m_HUDManager.StopLoadScreen();
        SceneManager.UnloadSceneAsync("Main");
    }

}
