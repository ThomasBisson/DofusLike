using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public enum Character
    {
        PLAYER,
        ENNEMY
    }

    [Header("Characters parent")]
    public Character m_character;


    void Start()
    {
        
    }
}
