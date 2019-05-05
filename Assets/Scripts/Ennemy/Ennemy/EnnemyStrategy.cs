using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnnemyStrategy
{
    protected EnnemyManager m_ennemyManager;

    public EnnemyStrategy(EnnemyManager ennemy)
    {
        m_ennemyManager = ennemy;
    }

    public abstract void Start();

    public abstract void OnMouseEnter();
    public abstract void OnMouseExit();
}
