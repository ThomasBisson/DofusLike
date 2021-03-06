﻿using Newtonsoft.Json;
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

    public void SendSpellHitMessage(Vector2 XY, string spellID)
    {
        var jsonObject = "{ \"spellID\" : \"" + spellID + "\", \"posXY\" : { \"x\" : " + XY.x + ", \"y\" : " + XY.y + "} }";

        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("TryToHitSpell", jsonObject);
    }

    public void SendBattleReadyInClient()
    {
        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("BattleReadyInClient");
    }

    public void SendPositionBattle(Vector2 XY)
    {
        var jsonObject = "{ \"posInBattle\" : { \"x\" : " + XY.x + ", \"y\" : " + XY.y + " } }";
        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("TryToMove", jsonObject);
    }

    public void SendEndTurnNotification()
    {
        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("EndTurn");
    }

    public void SendEngageBattleMessage(string id)
    {
        Debug.Log("Send message");
        //TODO : Send main pos and verify with server if it's OK
        var jsonObject = "{ \"id\" : \"" + id + "\"}";

        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("EngageBattle", jsonObject);
    }
}
