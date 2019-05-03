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
    private SocketOptions options;

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

    public bool ModifyNetworkIdentityOfNetworkObject(string id, NetworkIdentity ni) {
        if(m_serverObjects.ContainsKey(id))
        {
            m_serverObjects.Remove(id);
            m_serverObjects.Add(id, ni);
            return true;
        }
        return false;
    }

    public void SetupEvents()
    {
        #region CONNECTION_DECONNECTION

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

        socketManagerRef.Socket.On("playerDisconnect", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;

            Destroy(m_serverObjects[id].gameObject);
            m_serverObjects.Remove(id);
        });

        #endregion

        #region SPAWN_DELETE

        socketManagerRef.Socket.On("spawnPlayer", (socket, packet, args) =>
        {
            Debug.Log("Player : \n" + packet);

            //Get needed values in data
            var dataPlayer = args[0] as Dictionary<string, object>;
            var dataCharacteristic = dataPlayer["characteristic"] as Dictionary<string, object>;

            //Create a player from resources
            GameObject go = Instantiate((GameObject)Resources.Load("PlayersPrefabs/" + dataCharacteristic["name"] as string));
            string id = dataPlayer["id"] as string;
            go.name = string.Format("Player ({0})", id);

            //Set the NetworkIdentity
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            ni.SetSocketReference(this);
            m_serverObjects.Add(id, ni);

            GameManager.Instance.SetAPlayer(ni, args);



            ////Handling all spawning all players
            //var dataPlayer = args[0] as Dictionary<string, object>;
            //string id = dataPlayer["id"] as string;
            //var dataCharacteristic = dataPlayer["characteristic"] as Dictionary<string, object>;
            //Debug.Log(dataCharacteristic == null);
            //GameObject go = Instantiate((GameObject)Resources.Load("PlayersPrefabs/" + dataCharacteristic["name"] as string));
            //go.name = string.Format("Player ({0})", id);
            //NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            //var player = ni.GetComponent<PlayerManagerMain>();
            //player.m_character = Characters.Character.PLAYER;
            //if (ni.SetControllerID(id))
            //{
            //    player.gameObject.AddComponent<NetworkBattle>();
            //    player.FindNetworkBattle();
            //    GameManager.Instance.m_PlayerManagerMain = player;
            //}
            //ni.SetSocketReference(this);
            //m_serverObjects.Add(id, ni);

            ////Find HUD
            //player.FindHUDManager();

            ////Get the caracteristics of xuchu
            //player.SetPlayerStats(
            //    dataCharacteristic["name"] as string,
            //    (int)(double)dataPlayer["health"],
            //    (int)(double)dataPlayer["actionPoints"],
            //    (int)(double)dataPlayer["movementPoints"]
            //);
            //player.FindIconInResources();
            //player.m_stats.SetObservers();
            //player.m_stats.ActivateObserversFirstTime();

            ////Find given spells
            //var spellsAsList = dataCharacteristic["myspells"] as List<object>;
            //foreach (var obj in spellsAsList)
            //{
            //    string spellJson = JsonConvert.SerializeObject(obj as Dictionary<string, object>, Formatting.None);
            //    player.SetSpellTree(spellJson);
            //}

            ////SetPosition
            //var dataPos = dataPlayer["positionInWorld"] as Dictionary<string, object>;
            //float x = (float)(double)dataPos["x"];
            //float z = (float)(double)dataPos["z"];
            //player.transform.position = new Vector3(x, player.transform.position.y, z);

            //var dataPosMain = dataPlayer["positionArrayMain"] as Dictionary<string, object>;
            //float xMain = (float)(double)dataPosMain["x"];
            //float yMain = (float)(double)dataPosMain["y"];
            //player.m_positionArrayMain = new Vector2(xMain, yMain);

            //var dataPosFight = dataPlayer["positionArrayFight"] as Dictionary<string, object>;
            //float xFight = (float)(double)dataPosFight["x"];
            //float yFight = (float)(double)dataPosFight["y"];
            //player.m_positionArrayFight = new Vector2(xFight, yFight);
        });

        socketManagerRef.Socket.On("spawnEnnemies", (socket, packet, args) =>
        {
            Debug.Log("Ennemy : \n" + packet);

            //Handling spwaning group of ennemies
            var dataGroup = args[0] as Dictionary<string, object>;
            var dataEnnemiesAsList = dataGroup["monsters"] as List<object>;
            var dataEnnemies = new List<Dictionary<string, object>>();
            foreach (var obj in dataEnnemiesAsList)
                dataEnnemies.Add(obj as Dictionary<string, object>);
            string id = dataGroup["id"] as string;

            //Instantiate first monster that will be the EnnemyGroup
            GameObject go = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + dataEnnemies[0]["name"], typeof(GameObject)), m_networkContainer);

            //set the NetworkIdentity of the EnnemyGroup
            go.name = string.Format("Ennemies ({0})", id);
            NetworkIdentity niGroup = go.GetComponent<NetworkIdentity>();
            niGroup.SetControllerID(id);
            niGroup.SetSocketReference(this);
            m_serverObjects.Add(id, niGroup);

            //Create each ennemy and set their NetworkIdentity
            GameObject goEnnemy;
            List<NetworkIdentity> niEnnemies = new List<NetworkIdentity>();
            for (int i = 0; i < dataEnnemies.Count; i++)
            {
                goEnnemy = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + dataEnnemies[i]["name"], typeof(GameObject)), go.transform);
                goEnnemy.name = dataEnnemies[i]["name"] + " " + dataEnnemies[i]["id"];

                NetworkIdentity niEnnemy = goEnnemy.GetComponent<NetworkIdentity>();
                niEnnemy.SetControllerID(dataEnnemies[i]["id"] as string);
                niEnnemy.SetSocketReference(this);
                m_serverObjects.Add(dataEnnemies[i]["id"] as string, niEnnemy);
                niEnnemies.Add(niEnnemy);
            }

            GameManager.Instance.SetAnEnnemy(niGroup, niEnnemies, args);
            
        });

        socketManagerRef.Socket.On("deleteEnnemyGroup", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;
            var tempObject = m_serverObjects[id];
            m_serverObjects.Remove(id);
            DestroyImmediate(tempObject.gameObject);
        });

        #endregion

        #region UPDATE

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

            ni.transform.position = new Vector3((float)x, (float)y, (float)z);
        });

        socketManagerRef.Socket.On("NewDestination", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            var pos = data["position"] as Dictionary<string, object>;
            GameManager.Instance.m_playerManagerFight.NewDestination(
                new Vector2((float)(double)pos["x"], (float)(double)pos["y"])
            );
        });

        #endregion

        #region BATTLE

        socketManagerRef.Socket.On("EngageBattle", (socket, packet, args) => {
            Debug.Log("Engage Battle");
            var data = args[0] as Dictionary<string, object>;
            string id = data["id"] as string;

            BattleManager.Instance.SwitchToFightScene(m_serverObjects[id]);
        });

        socketManagerRef.Socket.On("UpdateCharacterStats", (socket, packet, args) => {
            Debug.Log(packet);
            var data = args[0] as Dictionary<string, object>;
            var character = m_serverObjects[data["id"] as string].GetComponent<Characters>();
            if (character.m_character == Characters.Character.PLAYER)
            {
                if ((character as PlayerManagerFight) != null)
                {
                    (character as PlayerManagerFight).m_stats.CurrentHealth = (int)(double)data["currentHealth"];
                    (character as PlayerManagerFight).m_stats.CurrentShield = (int)(double)data["currentShield"];
                    (character as PlayerManagerFight).m_stats.CurrentActionPoint = (int)(double)data["currentActionPoints"];
                    (character as PlayerManagerFight).m_stats.CurrentMovementPoint = (int)(double)data["currentMovementPoints"];
                }
            }
            else
            {
                if ((character as EnnemyManagerFight) != null)
                {
                    (character as EnnemyManagerFight).m_stats.CurrentHealth = (int)(double)data["currentHealth"];
                    (character as EnnemyManagerFight).m_stats.CurrentShield = (int)(double)data["currentShield"];
                    (character as EnnemyManagerFight).m_stats.CurrentActionPoint = (int)(double)data["currentActionPoints"];
                    (character as EnnemyManagerFight).m_stats.CurrentMovementPoint = (int)(double)data["currentMovementPoints"];
                }
                     
            }
        });


        socketManagerRef.Socket.On("UpdateTime", (socket, packet, args) => {
            Debug.Log(packet);
            var data = args[0] as Dictionary<string, object>;
            var character = m_serverObjects[data["id"] as string].GetComponent<Characters>();
            if(character.m_character == Characters.Character.PLAYER)
            {
                if ((character as PlayerManagerFight) != null)
                    (character as PlayerManagerFight).SetTime((int)(double)data["timeEachTurn"], (int)(double)data["currentTime"]);
            } else
            {
                if ((character as EnnemyManagerFight) != null)
                    (character as EnnemyManagerFight).SetTime((int)(double)data["timeEachTurn"], (int)(double)data["currentTime"]);
            }
        });

        #endregion
    }

}
