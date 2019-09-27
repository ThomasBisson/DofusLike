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
        socketManagerRef = new SocketManager(new Uri("http://127.0.0.1:60000/socket.io/")/*, options*/);

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

        socketManagerRef.Socket.On("LoadMainScene", (socket, packet, args) =>
        {
            MenuTemporary.Instance.GoToMainScene();
        });

        socketManagerRef.Socket.On("register", (socket, packet, args) =>
        {
            var data = args[0] as Dictionary<string, object>;
            ClientID = data["id"] as string;

            Debug.Log("Our client's ID : " + ClientID);

            m_networkContainer = new GameObject().transform;
            m_networkContainer.name = "ServerSpawnObjects";
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
            var dataCharacteristic = dataPlayer["baseCharacteristic"] as Dictionary<string, object>;

            //Create a player from resources
            GameObject go = Instantiate((GameObject)Resources.Load("PlayersPrefabs/" + dataCharacteristic["name"] as string));
            string id = dataPlayer["id"] as string;
            go.name = string.Format("Player ({0})", id);

            //Set the NetworkIdentity
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            ni.SetSocketReference(this);
            m_serverObjects.Add(id, ni);

            GameManager.SetAMainPlayer(ni, args);
        });

        socketManagerRef.Socket.On("spawnAnotherPlayer", (socket, packet, args) =>
        {
            Debug.LogError("Player : \n" + packet);

            //Get needed values in data
            var dataPlayer = args[0] as Dictionary<string, object>;
            var dataCharacteristic = dataPlayer["baseCharacteristic"] as Dictionary<string, object>;

            //Create a player from resources
            GameObject go = Instantiate((GameObject)Resources.Load("OtherPlayersPrefabs/" + dataCharacteristic["name"] as string));
            string id = dataPlayer["id"] as string;
            go.name = string.Format("OtherPlayer ({0})", id);

            //Set the NetworkIdentity
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            ni.SetSocketReference(this);
            m_serverObjects.Add(id, ni);

            GameManager.SetAnotherPlayer(ni, args);
        });

        socketManagerRef.Socket.On("spawnEnnemies", (socket, packet, args) =>
        {
            Debug.Log("Ennemy : \n" + packet);

            //Get needed values in data
            var dataGroup = args[0] as Dictionary<string, object>;
            var dataEnnemiesAsList = dataGroup["monsters"] as List<object>;
            var dataPosGroup = dataGroup["position"] as Dictionary<string, object>;
            var dataEnnemies = new List<Dictionary<string, object>>();
            var dataEnnemiesCaracteristic = new List<Dictionary<string, object>>();

            foreach (var obj in dataEnnemiesAsList)
            {
                dataEnnemies.Add(obj as Dictionary<string, object>);
                dataEnnemiesCaracteristic.Add(dataEnnemies[dataEnnemies.Count - 1]["baseCharacteristic"] as Dictionary<string, object>);
            }
            string id = dataGroup["id"] as string;

            //Instantiate first monster that will be the EnnemyGroup
            GameObject go = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + dataEnnemiesCaracteristic[0]["name"], typeof(GameObject)), m_networkContainer);

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
                goEnnemy = Instantiate((GameObject)Resources.Load("EnnemiesPrefabs/" + dataEnnemiesCaracteristic[i]["name"], typeof(GameObject)), go.transform);
                goEnnemy.name = dataEnnemiesCaracteristic[i]["name"] + " " + dataEnnemies[i]["id"];

                NetworkIdentity niEnnemy = goEnnemy.GetComponent<NetworkIdentity>();
                niEnnemy.SetControllerID(dataEnnemies[i]["id"] as string);
                niEnnemy.SetSocketReference(this);
                m_serverObjects.Add(dataEnnemies[i]["id"] as string, niEnnemy);
                niEnnemies.Add(niEnnemy);
            }

            GameManager.SetAnEnnemy(niGroup, niEnnemies, args);
            
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
            GameManager.PlayerManager.GetPlayerFight().NewDestination(
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
            //Debug.Log(packet);
            var data = args[0] as Dictionary<string, object>;
            var character = m_serverObjects[data["id"] as string].GetComponent<Characters>();
            var spellsCooldown = data["spellsCooldown"] as Dictionary<String, object>;
            if (character.m_character == Characters.Character.PLAYER)
            {
                if ((character as PlayerManager) != null)
                {
                    (character as PlayerManager).m_stats.CurrentHealth = (int)(double)data["currentHealth"];
                    (character as PlayerManager).m_stats.CurrentShield = (int)(double)data["currentShield"];
                    (character as PlayerManager).m_stats.CurrentActionPoint = (int)(double)data["currentActionPoints"];
                    (character as PlayerManager).m_stats.CurrentMovementPoint = (int)(double)data["currentMovementPoints"];

                    //time
                    (character as PlayerManager).SetTime((int)(double)data["timeInTurn"], (int)(double)data["actualTimeInTurn"]);

                    //Cooldown spellsCooldown
                    Dictionary<String, float> dicCooldown = new Dictionary<string, float>();
                    foreach (var spellCooldown in spellsCooldown)
                        dicCooldown.Add(spellCooldown.Key, (int)(double)spellCooldown.Value);
                    (character as PlayerManager).GetPlayerFight().SetSpellActualCooldown(dicCooldown);
                }
            }
            else
            {
                if ((character as EnnemyManager) != null)
                {
                    (character as EnnemyManager).m_stats.CurrentHealth = (int)(double)data["currentHealth"];
                    (character as EnnemyManager).m_stats.CurrentShield = (int)(double)data["currentShield"];
                    (character as EnnemyManager).m_stats.CurrentActionPoint = (int)(double)data["currentActionPoints"];
                    (character as EnnemyManager).m_stats.CurrentMovementPoint = (int)(double)data["currentMovementPoints"];

                    //time
                    (character as EnnemyManager).SetTime((int)(double)data["timeInTurn"], (int)(double)data["actualTimeInTurn"]);
                }
                     
            }
        });

        socketManagerRef.Socket.On("SpellHasHit", (socket, packet, args) =>
        {
            Debug.Log(packet);
            var data = args[0] as Dictionary<string, object>;
            var endPosData = data["endPosition"] as Dictionary<string, object>;
            var playerPosData = data["startPosition"] as Dictionary<string, object>;
            GameManager.PlayerManager.GetPlayerFight().ActivateSpell(
                data["spellID"] as string,
                new Vector2((int)(double)playerPosData["x"], (int)(double)playerPosData["y"]),
                new Vector2((int)(double)endPosData["x"], (int)(double)endPosData["y"])
            );
        });


        //socketManagerRef.Socket.On("UpdateTime", (socket, packet, args) => {
        //    var data = args[0] as Dictionary<string, object>;
        //    var character = m_serverObjects[data["id"] as string].GetComponent<Characters>();
        //    if(character.m_character == Characters.Character.PLAYER)
        //    {
        //        if ((character as PlayerManager) != null)
        //            (character as PlayerManager).SetTime((int)(double)data["timeEachTurn"], (int)(double)data["currentTime"]);
        //    } else
        //    {
        //        if ((character as EnnemyManager) != null)
        //            (character as EnnemyManager).SetTime((int)(double)data["timeEachTurn"], (int)(double)data["currentTime"]);
        //    }
        //});

        socketManagerRef.Socket.On("EndFight", (socket, packet, args) =>
        {
            Debug.Log("End battle");
            Debug.Log(packet);
            var data = args[0] as Dictionary<string, object>;
            var playerPositionMain = data["positionArrayMain"] as Dictionary<string, object>;
            string id = data["monsterGroupID"] as string;
            var tempObject = m_serverObjects[id];
            m_serverObjects.Remove(id);
            DestroyImmediate(tempObject.gameObject);

            BattleManager.Instance.SwitchToMain(new Vector2((float)(double)playerPositionMain["x"], (float)(double)playerPositionMain["y"]));

        });

        #endregion
    }

}
