﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransform : MonoBehaviour
{
    [SerializeField]
    [GreyOut]
    private Vector3 m_oldPosition;

    private NetworkIdentity m_networkIdentity;
    private Player m_player;

    private float stillCounter = 0;

    public void Start()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
        m_oldPosition = transform.position;
        m_player = new Player();
        m_player.id = m_networkIdentity.GetID();

        if(!m_networkIdentity.IsControlling()) {
            enabled = false;
        }
    }

    public void Update()
    {
        if(m_networkIdentity.IsControlling())
        {
            if(m_oldPosition != transform.position)
            {
                m_oldPosition = transform.position;
                stillCounter = 0;
                SendData();
            }
            else
            {
                stillCounter += Time.deltaTime;

                //To avoid hamering the server with data
                if(stillCounter >= 1)
                {
                    stillCounter = 0;
                    SendData();
                }
            }
        }
    }

    private void SendData()
    {
        //Update player information
        m_player.positionInWorld.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
        m_player.positionInWorld.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
        m_player.positionInWorld.z = Mathf.Round(transform.position.z * 1000.0f) / 1000.0f;

        var jsonObject = JsonConvert.SerializeObject(m_player);

        m_networkIdentity.GetSocket().socketManagerRef.Socket.Emit("updatePosition", jsonObject);
    }
}