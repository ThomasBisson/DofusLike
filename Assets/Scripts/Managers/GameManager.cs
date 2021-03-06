﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private static HUDUIManager m_HUDUIManager;

    public static PlayerManager PlayerManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #region Instantiations

    /// <summary>
    /// Set the Player and its datas
    /// </summary>
    /// <param name="ni"></param>
    /// <param name="data"></param>
    public static void SetAMainPlayer(NetworkIdentity ni, object[] data)
    {
        //Get needed values in data
        var dataPlayer = data[0] as Dictionary<string, object>;
        var dataCharacteristic = dataPlayer["baseCharacteristic"] as Dictionary<string, object>;
        //var spellsAsList = dataCharacteristic["myspells"] as List<object>;
        var spellsAsList = dataPlayer["spells"] as Dictionary<string, object>;


        //Set the PlayerManagerMain
        var player = ni.GetComponent<PlayerManager>();
        player.m_character = Characters.Character.PLAYER;
        if (ni.IsControlling())
        {
            player.gameObject.AddComponent<NetworkBattle>();
            player.FindNetworkBattle();
            PlayerManager = player;
        }
        player.m_HUDUIManager = m_HUDUIManager;
        player.ChangeStrategy(PlayerManager.PlayerStartegy.Main);

        //Subscribe to events
        //player.SubscribeToTimeEvents(m_HUDUIManager.SwitchableMana.SwitchableF.TurnFight.)

        //Set player Stats
        player.SetStats(
            dataCharacteristic["name"] as string,
            (int)(double)dataPlayer["health"],
            (int)(double)dataPlayer["actionPoints"],
            (int)(double)dataPlayer["movementPoints"]
        );
        //Subscribe observer
        player.m_stats.Subscribe(Stats.PossibleStats.CurrentHealth, m_HUDUIManager.HealthTextChara.UpdateMyValue);
        player.m_stats.Subscribe(Stats.PossibleStats.CurrentShield, m_HUDUIManager.ShieldTextChara.UpdateMyValue);
        player.m_stats.Subscribe(Stats.PossibleStats.CurrentPA, m_HUDUIManager.ActionPointsTextChara.UpdateMyValue);
        player.m_stats.Subscribe(Stats.PossibleStats.CurrentPM, m_HUDUIManager.MovementPointsTextChara.UpdateMyValue);

        //Find the icon in the resources
        player.FindIconInResources();

        //Set SpellTree
        //foreach (var obj in spellsAsList)
        //{
        //    string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
        //    Debug.Log(obj as string);
        //    //Debug.Log(JsonUtility.ToJson(obj as Dictionary<string, object>));
        //    player.SetSpellTree(spellJson);
        //}
        player.SetSpellTree(spellsAsList);

        //SetPosition
        var dataPos = dataPlayer["positionInWorld"] as Dictionary<string, object>;
        float x = (float)(double)dataPos["x"];
        float z = (float)(double)dataPos["z"];
        player.transform.position = new Vector3(x, player.transform.position.y, z);

        var dataPosMain = dataPlayer["positionArrayMain"] as Dictionary<string, object>;
        float xMain = (float)(double)dataPosMain["x"];
        float yMain = (float)(double)dataPosMain["y"];
        player.m_positionArrayMain = new Vector2(xMain, yMain);

        var dataPosFight = dataPlayer["positionArrayFight"] as Dictionary<string, object>;
        float xFight = (float)(double)dataPosFight["x"];
        float yFight = (float)(double)dataPosFight["y"];
        player.m_positionArrayFight = new Vector2(xFight, yFight);
    }

    public static void SetAnotherPlayer(NetworkIdentity ni, object[] data)
    {
        //Get needed values in data
        var dataPlayer = data[0] as Dictionary<string, object>;
        var dataCharacteristic = dataPlayer["baseCharacteristic"] as Dictionary<string, object>;
        //var spellsAsList = dataCharacteristic["myspells"] as List<object>;
        var spellsAsList = dataPlayer["spells"] as Dictionary<string, object>;

        //Set the PlayerManagerMain
        var player = ni.GetComponent<OtherPlayerManager>();
        player.m_character = Characters.Character.PLAYER;
        
        //Set player Stats
        player.SetStats(
            dataCharacteristic["name"] as string,
            (int)(double)dataPlayer["health"],
            (int)(double)dataPlayer["actionPoints"],
            (int)(double)dataPlayer["movementPoints"]
        );

        //Find the icon in the resources
        player.FindIconInResources();

        //Set SpellTree
        //foreach (var obj in spellsAsList)
        //{
        //    string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
        //    player.SetSpellTree(spellJson);
        //}
        player.SetSpellTree(spellsAsList);

        //SetPosition
        var dataPos = dataPlayer["positionInWorld"] as Dictionary<string, object>;
        float x = (float)(double)dataPos["x"];
        float z = (float)(double)dataPos["z"];
        player.transform.position = new Vector3(x, player.transform.position.y, z);

        var dataPosMain = dataPlayer["positionArrayMain"] as Dictionary<string, object>;
        float xMain = (float)(double)dataPosMain["x"];
        float yMain = (float)(double)dataPosMain["y"];
        player.m_positionArrayMain = new Vector2(xMain, yMain);

        var dataPosFight = dataPlayer["positionArrayFight"] as Dictionary<string, object>;
        float xFight = (float)(double)dataPosFight["x"];
        float yFight = (float)(double)dataPosFight["y"];
        player.m_positionArrayFight = new Vector2(xFight, yFight);
    }

    /// <summary>
    /// Set the enneny as Ennemygroup or Ennemy and set their datas
    /// </summary>
    /// <param name="niEnnemyGroup"></param>
    /// <param name="niEnnemies"></param>
    /// <param name="data">All the data gotten from the server</param>
    public static void SetAnEnnemy(NetworkIdentity niEnnemyGroup, List<NetworkIdentity> niEnnemies, object[] data)
    {
        //Get needed values in data
        var dataGroup = data[0] as Dictionary<string, object>;
        var dataEnnemiesAsList = dataGroup["monsters"] as Dictionary<string, object>;
        var dataPosGroup = dataGroup["position"] as Dictionary<string, object>;
        var dataEnnemies = new List<Dictionary<string, object>>();
        var dataEnnemiesCaracteristic = new List<Dictionary<string, object>>();

        foreach (var obj in dataEnnemiesAsList) {
            dataEnnemies.Add(obj.Value as Dictionary<string, object>);
            dataEnnemiesCaracteristic.Add(dataEnnemies[dataEnnemies.Count-1]["baseCharacteristic"] as Dictionary<string, object>);
        }

        //Destroy the component Ennemy and create a component EnnemyGroup
        DestroyImmediate(niEnnemyGroup.GetComponent<EnnemyManager>());
        EnnemyGroup ennemyGroup = niEnnemyGroup.gameObject.AddComponent<EnnemyGroup>();
        ennemyGroup.ActivateMainMode();

        //Create each EnnemyManager
        EnnemyManager ennemy;
        Dictionary<string, object> spellsAsList;
        Dictionary<string, object> dataPosFight;
        for (int i = 0; i < dataEnnemies.Count; i++)
        {
            //Add EnnemyManager
            ennemy = niEnnemies[i].gameObject.GetComponent<EnnemyManager>();
            ennemy.m_character = Characters.Character.ENNEMY;

            //Set Stats
            ennemy.SetStats(
                dataEnnemiesCaracteristic[i]["name"] as string,
                (int)(double)dataEnnemies[i]["health"],
                (int)(double)dataEnnemies[i]["actionPoints"],
                (int)(double)dataEnnemies[i]["movementPoints"]
            );

            //Find the icon in the resources
            ennemy.FindIconInResources();

            //Set SpellTree
            spellsAsList = dataEnnemies[i]["spells"] as Dictionary<string, object>;
            ennemy.SetSpellTree(spellsAsList);
            ennemyGroup.AddToEnnemyGroup(ennemy);
            ennemy.gameObject.SetActive(false);

            //Set the ennemy's position
            dataPosFight = dataEnnemies[i]["positionArrayFight"] as Dictionary<string, object>;
            float xEnnemy = (float)(double)dataPosFight["x"];
            float yEnnemy = (float)(double)dataPosFight["y"];
            ennemy.m_positionArrayFight = new Vector2(xEnnemy, yEnnemy);

            ennemy.ChangeStrategy(EnnemyManager.PossibleStrategy.Main);
        }

        //Set the EnnemyGroup position
        float x = (float)(double)dataPosGroup["x"];
        float z = (float)(double)dataPosGroup["z"];
        ennemyGroup.Position = new Vector2(x, z);
    }

    #endregion

    #region SceneManagement

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

        m_HUDUIManager = HUDUIManager.Instance;
    }

    #endregion

}
