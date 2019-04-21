using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkIdentity : MonoBehaviour
{
    [Header("Helpful values")]
    [SerializeField]
    [GreyOut]
    private string m_id;
    [SerializeField]
    [GreyOut]
    private bool m_isControlling;

    private NetworkClient m_socket;

    // Start is called before the first frame update
    void Awake()
    {
        m_isControlling = false;
    }

    public bool SetControllerID(string ID)
    {
        m_id = ID;
        //Check incomming ID
        m_isControlling = NetworkClient.ClientID == ID;

        return m_isControlling;
    }

    public void SetSocketReference(NetworkClient socket)
    {
        m_socket = socket;
    }

    public string GetID() { return m_id; }
    public bool IsControlling() { return m_isControlling; }
    public NetworkClient GetSocket() { return m_socket; }
}
