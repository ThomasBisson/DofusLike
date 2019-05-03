using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnnemyStrategy : MonoBehaviour
{
    protected EnnemyManager m_ennemyManager;

    public EnnemyStrategy(EnnemyManager ennemy)
    {
        m_ennemyManager = ennemy;
    }
}
