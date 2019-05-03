using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HUDUIManager m_HUDUIManager;

    private PlayerManagerMain m_playerManagerMain;
    public PlayerManagerMain m_PlayerManagerMain
    {
        get { return m_playerManagerMain; }
        set
        {
            m_playerManagerMain = value;
            Camera.main.GetComponent<FollowTarget>().Target = m_playerManagerMain.transform;
        }
    }

    public PlayerManagerFight m_playerManagerFight;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        m_HUDUIManager = HUDUIManager.Instance;
    }

    public void SetAPlayer(NetworkIdentity ni, object[] data) {
        //Get needed values in data
        var dataPlayer = data[0] as Dictionary<string, object>;
        var dataCharacteristic = dataPlayer["characteristic"] as Dictionary<string, object>;
        var spellsAsList = dataCharacteristic["myspells"] as List<object>;


        //Set the PlayerManagerMain
        var player = ni.GetComponent<PlayerManagerMain>();
        player.m_character = Characters.Character.PLAYER;
        if (ni.IsControlling())
        {
            player.gameObject.AddComponent<NetworkBattle>();
            player.FindNetworkBattle();
            m_PlayerManagerMain = player;
        }
        player.m_HUDUIManager = m_HUDUIManager;
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
        foreach (var obj in spellsAsList)
        {
            string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
            player.SetSpellTree(spellJson);
        }

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

    public void SetAnEnnemy(NetworkIdentity niEnnemyGroup, List<NetworkIdentity> niEnnemies, object[] data)
    {
        //Get needed values in data
        var dataGroup = data[0] as Dictionary<string, object>;
        var dataEnnemiesAsList = dataGroup["monsters"] as List<object>;
        var dataPosGroup = dataGroup["position"] as Dictionary<string, object>;
        var dataEnnemies = new List<Dictionary<string, object>>();
        var dataEnnemiesCaracteristic = new List<Dictionary<string, object>>();

        foreach (var obj in dataEnnemiesAsList) {
            dataEnnemies.Add(obj as Dictionary<string, object>);
            dataEnnemiesCaracteristic.Add(dataEnnemies[dataEnnemies.Count-1]["characteristic"] as Dictionary<string, object>);
        }


        //Create EnnemyGroup
        EnnemyGroupMain ennemyGroup = niEnnemyGroup.gameObject.AddComponent<EnnemyGroupMain>();
        ennemyGroup.m_HUDUIManager = m_HUDUIManager;

        //Create each EnnemyManager
        GameObject goEnnemy;
        EnnemyManagerMain ennemy;
        List<object> spellsAsList;
        Dictionary<string, object> dataPosFight;
        for (int i = 0; i < dataEnnemies.Count; i++)
        {
            //Add EnnemyManager
            ennemy = niEnnemies[i].gameObject.AddComponent<EnnemyManagerMain>();
            ennemy.m_character = Characters.Character.ENNEMY;

            //Set Stats
            ennemy.SetStats(
                dataEnnemiesCaracteristic[i]["name"] as string,
                (int)(double)dataEnnemies[i]["health"],
                (int)(double)dataEnnemies[i]["actionPoints"],
                (int)(double)dataEnnemies[i]["movementPoints"]
            );
            //TODO : Subscribe to the events (observer)

            //Find the icon in the resources
            ennemy.FindIconInResources();

            //Set SpellTree
            spellsAsList = dataEnnemies[i]["myspells"] as List<object>;
            foreach (var obj in spellsAsList)
            {
                string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
                ennemy.SetSpellTree(spellJson);
            }
            ennemyGroup.AddToEnnemyGroup(ennemy);
            ennemy.gameObject.SetActive(false);

            //Set the ennemy's position
            dataPosFight = dataEnnemies[i]["position"] as Dictionary<string, object>;
            float xEnnemy = (float)(double)dataPosFight["x"];
            float yEnnemy = (float)(double)dataPosFight["y"];
            ennemy.m_positionArrayFight = new Vector2(xEnnemy, yEnnemy);
        }

        //Set the EnnemyGroup position
        float x = (float)(double)dataPosGroup["x"];
        float z = (float)(double)dataPosGroup["z"];
        ennemyGroup.m_position = new Vector2(x, z);
    }

}
