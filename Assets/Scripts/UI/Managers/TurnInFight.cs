using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThomasBisson.Mathematics;

public class TurnInFight : MonoBehaviour
{

    [SerializeField]
    private IconTurnInFight m_prefabTurnInFight;

    private List<IconTurnInFight> m_iconsTurnInFight = new List<IconTurnInFight>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PopulateTurnInFightBar(List<Characters> characters)
    {
        foreach(var charac in characters)
        {
            Debug.Log(charac.name);
            m_iconsTurnInFight.Add(Instantiate<IconTurnInFight>(m_prefabTurnInFight, transform));

            charac.SubscribeToIconEvents(m_iconsTurnInFight[m_iconsTurnInFight.Count - 1].UpdateIcon);
            charac.SubscribeToTimeAsPercentEvents(m_iconsTurnInFight[m_iconsTurnInFight.Count - 1].UpdateSecondsImage);
        }
    }
}
