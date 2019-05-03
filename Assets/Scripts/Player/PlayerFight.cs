using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Mathematics;
using UnityEngine;

public class PlayerFight : PlayerStrategy
{
    #region VARS

    public delegate GridFight GetGridFight();
    private GetGridFight m_getGridFight;

    public delegate Transform GetTransform();
    private GetTransform m_getTransform;

    [SerializeField]
    [GreyOut]
    private GridFight m_grid;

    private int m_spellUsedID;

    #endregion

    #region UNITY_METHODS

    public PlayerFight(PlayerManager player) : base(player)
    {


    }

    public void SetGetterCallbacks(GetGridFight getGridFight, GetTransform getTransform)
    {
        m_getGridFight = getGridFight;
        m_getTransform = getTransform;
    }

    public override void Start()
    {
        m_grid.m_playerManager = m_playerManager;
        m_getTransform().position = m_grid.Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
        m_playerManager.m_HUDUIManager.SwitchableMana.SwitchableF.SpellAndControlsUI.SetEndTurnButton(() =>
        {
            if (m_playerManager.IsItsTurn())
                m_playerManager.m_networkBattle.SendEndTurnNotification();
        });
        m_playerManager.m_networkBattle.SendBattleReadyInClient();
    }

    #endregion

    #region METHODS

    #region MOVEMENTS

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
        m_playerManager.m_positionArrayFight = new Vector2(Random.Range(0, m_grid.Map.GetLength(0)), Random.Range(0, m_grid.Map.GetLength(1)));
        if (mustTeleportPlayer)
            m_getTransform().position = m_grid.Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
    }

    //TODO : make him go tile by tile from combat
    public override void GoNear(Vector3 clickPoint)
    {
        m_playerManager.ChangeTarget(clickPoint);
    }

    public void NewDestination(Vector2 newPos)
    {
        m_playerManager.m_positionArrayFight = newPos;
        GoNear(m_grid.Map[(int)newPos.x, (int)newPos.y].transform.position);
    }

    #endregion


    #region SPELLS

    public void HandleSpellButtonClick(int idSpell)
    {
        if (m_grid.GetCurrentAction() == GridFight.Action.Spell)
        {
            m_grid.DeactivateTileInRange((int)m_playerManager.m_spellTree.GetSpells()[idSpell].range);
            m_grid.SetCurrentAction(GridFight.Action.Movement);
            m_spellUsedID = -1;
        }
        else
        {
            m_grid.ActivateTileInRange((int)m_playerManager.m_spellTree.GetSpells()[idSpell].range);
            m_grid.SetCurrentAction(GridFight.Action.Spell);
            m_spellUsedID = idSpell;
        }
    }

    public void SetHUDSpellButtons()
    {
        List<int> ids = new List<int>();
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < m_playerManager.m_spellTree.GetSpells().Count; i++)
        {
            ids.Add(i);
            sprites.Add(Resources.Load<Sprite>("SpellsIcons/" + m_playerManager.m_spellTree.GetSpells()[i].name));
            if (sprites[i] == null)
            {
                Debug.Log("Image null");
            }
        }

        m_playerManager.m_HUDUIManager.SwitchableMana.SwitchableF.SpellAndControlsUI.FillCallbacksAndIconsSpellButtons(HandleSpellButtonClick, ids, sprites);
    }

    public void TryToActivateSpell(Vector2 XY)
    {
        if (m_spellUsedID == -1 || !m_playerManager.IsItsTurn())
            return;

        int dist = (int)MathsUtils.CircleDistance(XY, m_playerManager.m_positionArrayFight);
        if (dist <= (int)m_playerManager.m_spellTree.GetSpells()[m_spellUsedID].range)
        {
            //Use spell
            Debug.Log("Can use spell");
            m_playerManager.m_networkBattle.SendSpellHitMessage(XY, m_playerManager.m_spellTree.GetSpells()[m_spellUsedID]._id);
        }
        else
        {
            //Don't use spell
            Debug.Log("Can't use spell");
        }
        m_grid.DeactivateTileInRange((int)m_playerManager.m_spellTree.GetSpells()[m_spellUsedID].range);
        m_grid.SetCurrentAction(GridFight.Action.Movement);
        m_spellUsedID = -1;

    }

    #endregion

    #region NETWORK

    public void SendMovementInFightMessage(Vector2 XY)
    {
        m_playerManager.m_networkBattle.SendPositionBattle(XY);
    }

    #endregion

    #endregion
}
