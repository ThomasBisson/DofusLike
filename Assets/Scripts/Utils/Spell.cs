using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spell
{
    public string _id;
    public string name;
    public float actionPointsConsuption;
    public float damage;
    public float range;
    public float explosiveRange;
    public float shield;
    public float shieldDuration;
    public float cooldown;
    public string animationKind;

    public Sprite icon;
    public int m_turnCooling;
}
