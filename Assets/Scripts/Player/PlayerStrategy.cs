using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStrategy
{
    protected PlayerManager m_playerManager;

    public PlayerStrategy(PlayerManager player)
    {
        m_playerManager = player;
    }

    public abstract void Start();

    public abstract void GoNear(Vector3 clickPoint);

    public abstract void HandleClickOnGround();

    public abstract void RandomizePlayerPosition(bool mustTeleportPlayer);
}
