using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTemporary : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField m_login;
    [SerializeField]
    private TMPro.TMP_InputField m_password;

    //Need so that even ifthe player press the button multiple times it will only send one message
    private bool m_isAlreadyGoing = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleVisible = true;
    }

    public void SendLoginAndPassword()
    {
        if (m_isAlreadyGoing)
            return;

        m_isAlreadyGoing = true;

        string myjson = "{ \"login\" : \"" + m_login.text + "\", \"password\" : \"" + m_password.text + "\" }";
        NetworkClient.Instance.socketManagerRef.GetSocket().Emit("Loggin", myjson);
    }
}
