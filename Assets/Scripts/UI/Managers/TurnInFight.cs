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
            m_iconsTurnInFight.Add(Instantiate<IconTurnInFight>(m_prefabTurnInFight, transform));

            if(charac.m_character == Characters.Character.PLAYER)
            {
                //m_iconsTurnInFight[m_iconsTurnInFight.Count - 1].SetObserver(delegate
                //{
                //    return (charac as PlayerManagerFight).GetTimeAsPercent() / 100f;
                //});
            } else
            {
                //m_iconsTurnInFight[m_iconsTurnInFight.Count - 1].SetObserver(delegate
                //{
                //    return (charac as EnnemyManagerFight).GetTimeAsPercent() / 100f;
                //});
            }
            //m_iconsTurnInFight[m_iconsTurnInFight.Count - 1].UpdateValue();
        }
    }



    public void UpdateValues()
    {
        foreach (var iconturn in m_iconsTurnInFight)
            iconturn.UpdateValue();
    }
}
