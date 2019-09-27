using System;
using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Mathematics;
using UnityEngine;

public class PlayerFight : PlayerStrategy
{
    #region Vars

    public delegate GridFight GetGridFight();
    private GetGridFight m_getGridFight;

    public delegate SpellTree GetSpellTree();
    private GetSpellTree m_getSpellTree;

    private string m_spellUsedID;

    #endregion

    #region ConstructorAndStart

    public PlayerFight(PlayerManager player) : base(player)
    {


    }

    public void SetGetterCallbacks(GetGridFight getGridFight, GetSpellTree getSpellTree)
    {
        m_getGridFight = getGridFight;
        m_getSpellTree = getSpellTree;
    }

    public override void Start()
    {
        m_getGridFight().m_playerManager = m_playerManager;
        m_playerManager.transform.position = m_getGridFight().Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
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

    public override void Update()
    {
        
    }

    #region Movements

    public bool CanMove(int movementPointUsed)
    {
        if (!m_playerManager.IsItsTurn())
            return false;
        return m_playerManager.m_stats.CurrentMovementPoint >= movementPointUsed;
    }

    public override void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_playerManager.m_positionArrayFight = new Vector2(UnityEngine.Random.Range(0, m_getGridFight().Map.GetLength(0)), UnityEngine.Random.Range(0, m_getGridFight().Map.GetLength(1)));
        if (mustTeleportPlayer)
            m_playerManager.transform.position = m_getGridFight().Map[(int)m_playerManager.m_positionArrayFight.x, (int)m_playerManager.m_positionArrayFight.y].transform.position;
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

    public void SetHUDSpellButtons()
    {
        List<string> ids = new List<string>();
        List<Sprite> sprites = new List<Sprite>();
        foreach (var keyValuePair in m_getSpellTree().GetSpells())
        {

            //m_HUDUIManager.SwitchableMana.SwitchableF.SpellAndControlsUI.SetSpellButton(counter, GetPlayerFight().HandleSpellButtonClick, key)

            ids.Add(m_getSpellTree().GetSpell(keyValuePair.Key)._id);
            sprites.Add(m_getSpellTree().GetSpell(keyValuePair.Key).icon);
            //counter++;
        }

        m_playerManager.m_HUDUIManager.SwitchableMana.SwitchableF.SpellAndControlsUI.FillCallbacksAndIconsSpellButtons(HandleSpellButtonClick, ids, sprites);
    }

    public void HandleSpellButtonClick(string idSpell)
    {
        if (m_getGridFight().GetCurrentAction() == GridFight.Action.Spell)
        {
            if (idSpell != m_spellUsedID)
            {
                m_getGridFight().DeactivateTileInRange((int)m_getSpellTree().GetSpell(m_spellUsedID).range);
                m_getGridFight().ActivateTileInRange((int)m_getSpellTree().GetSpell(idSpell).range);
                m_spellUsedID = idSpell;
            }
            else
            {
                m_getGridFight().DeactivateTileInRange((int)m_getSpellTree().GetSpell(idSpell).range);
                m_getGridFight().SetCurrentAction(GridFight.Action.Movement);
                m_spellUsedID = "";
            }
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

    public void ActivateSpell(string spellID, Vector2 playerPos, Vector2 endPos)
    {
        m_playerManager.SetSpellTarget(endPos);
        m_getSpellTree().ActivateSpellFX(spellID);
    }

    #region Event

    public delegate void SpellCooldownEventHandler(Dictionary<String, float> spellCooldownAsPercent);
    protected event SpellCooldownEventHandler m_spellCooldownsEvents;
 
    public void SubscribeToSpellCooldownEvents(SpellCooldownEventHandler newObserver)
    {
        if (!IsObserverSpellCooldownInList(newObserver))
        {
            m_spellCooldownsEvents += new SpellCooldownEventHandler(newObserver);
            m_spellCooldownsEvents.Invoke(GetSpellCooldownsAsPercent());
        }
    }

    public void UnsubscribeToSpellCooldownEvents(SpellCooldownEventHandler newObserver)
    {
        if (IsObserverSpellCooldownInList(newObserver))
        {
            m_spellCooldownsEvents -= new SpellCooldownEventHandler(newObserver);
        }
    }

    private bool IsObserverSpellCooldownInList(SpellCooldownEventHandler newObserver)
    {
        if (m_spellCooldownsEvents != null)
        {
            foreach (var existingHandler in m_spellCooldownsEvents.GetInvocationList())
                if (Delegate.Equals(existingHandler, newObserver))//existingHandler == newObserver) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                    return true;
        }
        return false;
    }

    private Dictionary<String, float> GetSpellCooldownsAsPercent()
    {
        Dictionary<String, float> dic = new Dictionary<string, float>();
        foreach (KeyValuePair<string, Spell> spell in m_getSpellTree().GetSpells())
            dic.Add(spell.Value._id, MathsUtils.PercentValueFromAnotherValue((float)spell.Value.m_turnCooling, (float)spell.Value.cooldown) / 100f);
        return dic;
    }

    public void SetSpellActualCooldown(Dictionary<String, float> actualCooldown)
    {
        foreach(var Spell in actualCooldown)
            m_playerManager.SetSpellActualCooldown(Spell.Key, Spell.Value);

        m_spellCooldownsEvents.Invoke(GetSpellCooldownsAsPercent());
    }

    #endregion

    #endregion

    #region Network

    public void SendMovementInFightMessage(Vector2 XY)
    {
        m_playerManager.m_networkBattle.SendPositionBattle(XY);
    }

    #endregion

    #endregion
}
