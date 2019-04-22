using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;
//using System.Net.Sockets;
//using System.Text;

using BestHTTP;
using BestHTTP.SocketIO;
using System;
using Newtonsoft.Json;
using System.Globalization;

public class NetworkClient : MonoBehaviour
{
    //OLD
    public static string ClientID { get; private set; }


    [Header("Server Connection")]
    [SerializeField]
    private Transform m_networkContainer;

    private Dictionary<string, NetworkIdentity> m_serverObjects = new Dictionary<string, NetworkIdentity>();


    //NEW
    #region PUBLIC_VARS

    //Instance
    public static NetworkClient Instance;

    //SocketManager Reference
    public SocketManager socketManagerRef;

    //Options
    SocketOptions options;

    #endregion

    #region PRIVATE_VARS

    //private PlayerManager m_playerManager;

    #endregion


    #region UNITY_CALLBACKS

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        CreateSocketRef();
        SetupEvents();
    }

    #endregion


    public void CreateSocketRef()
    {
        TimeSpan miliSecForReconnect = TimeSpan.FromMilliseconds(1000);

        //options = new SocketOptions();
        //options.ReconnectionAttempts = 3;
        //options.AutoConnect = false;
        //options.ReconnectionDelay = miliSecForReconnect;

        //Server URI
        socketManagerRef = new SocketManager(new Uri("http://127.0.0.1:8080/socket.io/")/*, options*/);

        socketManagerRef.Open();
    }

    public void SetupEvents()
    {
        socketManagerRef.Socket.On("open", (socket, packet, args) =>
        {
            Debug.Log("Connection made to the server !");
        });

        socketManagerRef.Socket.On("register", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            ClientID = data["id"] as string;

            Debug.Log("Our client's ID : " + ClientID);
        });

        socketManagerRef.Socket.On("spawnPlayer", (socket, packet, args) =>
        {
            //Handling all spawning all players
            var dataPlayer = args[0] as Dictionary<string, object>;
            string id = dataPlayer["id"] as string;
            var dataCharacteristic = dataPlayer["characteristic"] as Dictionary<string, object>;
            GameObject go = Instantiate((GameObject)Resources.Load("PlayersPrefabs/" + dataCharacteristic["name"]));
            go.name = string.Format("Player ({0})", id);
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            var player = ni.GetComponent<PlayerManagerMain>();
            if (ni.SetControllerID(id))
            {
                player.gameObject.AddComponent<NetworkBattle>();
                player.FindNetworkBattle();
                GameManager.Instance.m_PlayerManagerMain = player;
            }
            ni.SetSocketReference(this);
            m_serverObjects.Add(id, ni);

            //Find HUD
            player.FindHUDManager();

            //Get the caracteristics of xuchu
            player.SetPlayerStats(dataCharacteristic);
            player.m_playerStats.SetObservers();
            player.m_playerStats.ActivateObserversFirstTime();

            //Find given spells
            var spellsAsList = dataPlayer["spells"] as List<object>;
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
        });

        socketManagerRef.Socket.On("spawnEnnemies", (socket, packet, args) =>
        {
            //Handling spwaning group of ennemies
            var dataGroup = args[0] as Dictionary<string, object>;
            var dataEnnemiesAsList = dataGroup["monsters"] as List<object>;
            var dataEnnemies = new List<Dictionary<string, object>>();
            foreach (var obj in dataEnnemiesAsList)
                dataEnnemies.Add(obj as Dictionary<string, object>);
            string id = dataGroup["id"] as string;

            //Instantiate first monster
            GameObject go = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + dataEnnemies[0]["name"], typeof(GameObject)), m_networkContainer);

            //Name and set network vars
            go.name = string.Format("Ennemy ({0})", id);
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            ni.SetSocketReference(this);
            m_serverObjects.Add(id, ni);

            //Create EnnemyGroup
            EnnemyGroup ennemyGroup = go.AddComponent<EnnemyGroup>();
            ennemyGroup.m_HUDUIManager = HUDUIManager.Instance;
            EnnemyManagerMain ennemy;
            List<object> spellsAsList;
            for (int i=0; i<dataEnnemies.Count; i++)
            {
                ennemy = new EnnemyManagerMain();
                ennemy.SetEnnemyStats(dataEnnemies[i]["characteristic"] as Dictionary<string, object>);
                spellsAsList = dataEnnemies[i]["spells"] as List<object>;
                foreach(var obj in spellsAsList)
                {
                    string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
                    ennemy.SetSpellTree(spellJson);
                }
                ennemyGroup.AddToEnnemyGroup(ennemy);
                ennemy.m_ennemyStats.SetObservers();
                ennemy.m_ennemyStats.ActivateObserversFirstTime();
            }

            //Position
            var dataPos = dataGroup["position"] as Dictionary<string, object>;
            float x = (float)(double)dataPos["x"];
            float z = (float)(double)dataPos["z"];
            go.transform.position = new Vector3(x, go.transform.position.y, z);
        });

        socketManagerRef.Socket.On("EngageBattle", (socket, packet, args) => {
            Debug.Log("Engage Battle");
            BattleManager.Instance.SwitchToFightScene();
        });

        socketManagerRef.Socket.On("deleteEnnemyGroup", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;
            var tempObject = m_serverObjects[id];
            m_serverObjects.Remove(id);
            DestroyImmediate(tempObject.gameObject);
        });

        socketManagerRef.Socket.On("playerDisconnect", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;

            Destroy(m_serverObjects[id].gameObject);
            m_serverObjects.Remove(id);
        });

        socketManagerRef.Socket.On("updatePosition", (socket, packet, args) =>
        {
            Debug.Log("A position was updated");

            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;

            var posData = data["position"] as Dictionary<string, object>;

            double x = (double)posData["x"];
            double y = (double)posData["y"];
            double z = (double)posData["z"];

            NetworkIdentity ni = m_serverObjects[id];

            //TODO : Find more elegante method to access PlayerManager
            //ni.GetComponent<PlayerManager>().GoNear(new Vector3((float)x, (float)y, (float)z));
            ni.transform.position = new Vector3((float)x, (float)y, (float)z);
        });
    }

}

[System.Serializable]
public class Player {
    public string username;
    public string id;
    public Position positionInWorld;
    public Position positionMain;
    public Position positionFight;

    public Player()
    {
        positionInWorld = new Position();
        positionMain = new Position();
        positionFight = new Position();
    }

    public override string ToString()
    {
        return String.Format("{ Username: {0}, ID: {1}, Position: {2} }", username, id, positionMain.ToString());
    }
    
}

[System.Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;

    public Position()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public override string ToString()
    {
        return String.Format("{ {0};{1} }", x, y);
    }

}
