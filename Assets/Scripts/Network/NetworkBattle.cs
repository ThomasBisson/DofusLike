using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBattle : MonoBehaviour
{

    private NetworkIdentity m_networkIdentity;

    void Start()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
    }

    public void SendEngageBattleMessage(string id)
    {
        var jsonObject = "{ \"id\" : \"" + id + "\"}";

        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("EngageBattle", jsonObject);
    }
}
