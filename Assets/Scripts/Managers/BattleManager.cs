using System.Collections;
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
        StartCoroutine(WaitSceneIsLoadedFight(groupNI));
    }

    IEnumerator WaitSceneIsLoadedFight(NetworkIdentity groupNI)
    {
        //Get EnnemyGroupMain
        var groupMain = groupNI.GetComponent<EnnemyGroup>();

        //Start a loading screen and wait until the fight scene is loaded
        m_HUDManager.StartLoadScreen();
        Scene scene = SceneManager.GetSceneByName("Fight");
        yield return new WaitUntil(() => scene.isLoaded);

        SceneManager.SetActiveScene(scene);

        //Set Player to fight mode
        GameManager.PlayerManager.ChangeStrategy(PlayerManager.PlayerStartegy.Fight);
        GameManager.PlayerManager.SubscribeToNewTurnEvents(m_HUDManager.SwitchableMana.SwitchableF.MakeLeftTopIconAppear);
        GameManager.PlayerManager.GetPlayerFight().SubscribeToSpellCooldownEvents(m_HUDManager.SwitchableMana.SwitchableF.SpellAndControlsUI.HandleSpellCooldown);


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
        m_HUDManager.SwitchableMana.SwitchToFight(GameManager.PlayerManager, groupFight);


        //Stop load screen and unload main scene
        m_HUDManager.StopLoadScreen();
        SceneManager.UnloadSceneAsync("Main");
    }

    public void SwitchToMain(Vector2 posMainPlayer)
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        StartCoroutine(WaitSceneIsLoadedMain(posMainPlayer));
    }

    IEnumerator WaitSceneIsLoadedMain(Vector2 posMainPlayer)
    {
        //Start a loading screen and wait until the fight scene is loaded
        m_HUDManager.StartLoadScreen();
        Scene sceneMain = SceneManager.GetSceneByName("Main");
        yield return new WaitUntil(() => sceneMain.isLoaded);

        SceneManager.SetActiveScene(sceneMain);

        //Unsubscribe to events
        GameManager.PlayerManager.UnsubscribeToNewTurnEvents(m_HUDManager.SwitchableMana.SwitchableF.MakeLeftTopIconAppear);
        GameManager.PlayerManager.GetPlayerFight().UnsubscribeToSpellCooldownEvents(m_HUDManager.SwitchableMana.SwitchableF.SpellAndControlsUI.HandleSpellCooldown);

        //Activate HUD for battle
        m_HUDManager.SwitchableMana.SwitchToMain();


        //Stop load screen and unload main scene
        m_HUDManager.StopLoadScreen();
        Scene sceneFight = SceneManager.GetSceneByName("Fight");
        SceneManager.UnloadSceneAsync(sceneFight);

        yield return new WaitUntil(() => !sceneFight.isLoaded);

        //Set Player to main mode
        GameManager.PlayerManager.ChangeStrategy(PlayerManager.PlayerStartegy.Main);
    }
}
