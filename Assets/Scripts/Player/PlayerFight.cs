using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Mathematics;
using UnityEngine;

public class PlayerFight : PlayerStrategy
{
    #region Vars

    public delegate GridFight GetGridFight();
    private GetGridFight m_getGridFight;

    public delegate Transform GetTransform();
    private GetTransform m_getTransform;

    public delegate SpellTree GetSpellTree();
    private GetSpellTree m_getSpellTree;

    private string m_spellUsedID;

    #endregion

    #region ConstructorAndStart

    public PlayerFight(PlayerManager player) : base(player)
    {


    }

    public void SetGetterCallbacks(GetGridFight getGridFight, GetTransform getTransform, GetSpellTree getSpellTree)
    {
        m_getGridFight = getGridFight;
        m_getTransform = getTransform;
        m_getSpellTree = getSpellTree;
    }

    public override void Start()
    {
        m_getGridFight().m_playerManager = m_playerManager;
        m_getTransform().position = m_getGridFight().Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
        m_playerManager.m_HUDUIManager.SwitchableMana.SwitchableF.SpellAndControlsUI.SetEndTurnButton(() =>
        {
            if (m_playerManager.IsItsTurn())
                m_playerManager.m_networkBattle.SendEndTurnNotification();
        });
        SetHUDSpellButtons();
        m_playerManager.m_networkBattle.SendBattleReadyInClient();
    }

    #endregion

    #region Methods

    #region Movements

    public override void HandleClickOnGround()
    {
        
    }

    public bool CanMove(int movementPointUsed)
    {
        if (!m_playerManager.IsItsTurn())
            return false;
        return m_playerManager.m_stats.CurrentMovementPoint >= movementPointUsed;
    }

    public override void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_playerManager.m_positionArrayFight = new Vector2(Random.Range(0, m_getGridFight().Map.GetLength(0)), Random.Range(0, m_getGridFight().Map.GetLength(1)));
        if (mustTeleportPlayer)
            m_getTransform().position = m_getGridFight().Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
    }

    //TODO : make him go tile by tile from combat
    public override void GoNear(Vector3 clickPoint)
    {
        m_playerManager.ChangeTarget(clickPoint);
    }

    public void NewDestination(Vector2 newPos)
    {
        m_playerManager.m_positionArrayFight = newPos;
        GoNear(m_getGridFight().Map[(int)newPos.x, (int)newPos.y].transform.position);
    }

    #endregion


    #region Spells

    public void HandleSpellButtonClick(string idSpell)
    {
        if (m_getGridFight().GetCurrentAction() == GridFight.Action.Spell)
        {
            m_getGridFight().DeactivateTileInRange((int)m_getSpellTree().GetSpell(idSpell).range);
            m_getGridFight().SetCurrentAction(GridFight.Action.Movement);
            m_spellUsedID = "";
        }
        else
        {
            m_getGridFight().ActivateTileInRange((int)m_getSpellTree().GetSpell(idSpell).range);
            m_getGridFight().SetCurrentAction(GridFight.Action.Spell);
            m_spellUsedID = idSpell;
        }
    }

    public void TryToActivateSpell(Vector2 XY)
    {
        if (m_spellUsedID == "" || !m_playerManager.IsItsTurn())
            return;

        int dist = (int)MathsUtils.CircleDistance(XY, m_playerManager.m_positionArrayFight);
        if (dist <= (int)m_getSpellTree().GetSpell(m_spellUsedID).range)
        {
            //Use spell
            Debug.Log("Can use spell");
            m_playerManager.m_networkBattle.SendSpellHitMessage(XY, m_getSpellTree().GetSpell(m_spellUsedID)._id);
        }
        else
        {
            //Don't use spell
            Debug.Log("Can't use spell");
        }
        m_getGridFight().DeactivateTileInRange((int)m_getSpellTree().GetSpell(m_spellUsedID).range);
        m_getGridFight().SetCurrentAction(GridFight.Action.Movement);
        m_spellUsedID = "";

    }

    #endregion

    #region Network

    public void SendMovementInFightMessage(Vector2 XY)
    {
        m_playerManager.m_networkBattle.SendPositionBattle(XY);
    }

    #endregion

    #endregion
}
