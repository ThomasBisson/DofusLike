using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void EachTurn();

    public List<EachTurn> m_listeners = new List<EachTurn>();

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

    [Header("Gameplay parameters")]
    [SerializeField]
    private int m_secondInTurn;

    private HUDUIManager m_HUDUIManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_HUDUIManager = HUDUIManager.Instance;
        StartCoroutine("WaitEachTurn");
    }

    IEnumerator WaitEachTurn()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_secondInTurn);
            foreach (var callback in m_listeners)
                callback();
        }
    }
}
