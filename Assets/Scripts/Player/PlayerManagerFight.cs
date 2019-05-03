//using System.Collections;
//using System.Collections.Generic;
//using ThomasBisson.Mathematics;
//using UnityEngine;

//public class PlayerManagerFight : PlayerManager
//{

//    #region VARS

//    [SerializeField]
//    [GreyOut]
//    private GridFight m_grid;

//    private int m_spellUsedID;

//    #endregion

//    #region UNITY_METHODS

//    protected override void Awake()
//    {
//        base.Awake();
//        m_grid = FindObjectOfType<GridFight>();
//    }

//    protected override void Start()
//    {
//        base.Start();
//        m_grid.m_PlayerManagerFight = this;
//        transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
//        m_secondsLeftInTurn = 0;
//        m_HUDUIManager.SetEndTurnButton(() =>
//        {
//            if (m_secondsLeftInTurn >= 0)
//                m_networkBattle.SendEndTurnNotification();
//        });
//        m_networkBattle.SendBattleReadyInClient();
//    }

//    protected override void Update()
//    {
//        base.Update();
//    }

//    #endregion

//    #region METHODS

//    #region MOVEMENTS

//    public bool CanMove(int movementPointUsed)
//    {
//        if (m_secondsLeftInTurn <= 0)
//            return false;
//        return m_stats.CurrentMovementPoint >= movementPointUsed;
//    }

//    public void RandomizePlayerPosition(bool mustTeleportPlayer)
//    {
//        m_positionArrayFight = new Vector2(Random.Range(0, m_grid.Map.GetLength(0)), Random.Range(0, m_grid.Map.GetLength(1)));
//        if (mustTeleportPlayer)
//            transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
//    }

//    //TODO : make him go tile by tile from combat
//    public override void GoNear(Vector3 clickPoint)
//    {
//        base.GoNear(clickPoint);
//        m_targetPosition = clickPoint;
//        m_targetPositionChanged = true;
//        m_animator.SetBool("isRunning", true);
//        transform.LookAt(m_targetPosition);
//    }

//    public virtual void NewDestination(Vector2 newPos)
//    {
//        m_positionArrayFight = newPos;
//        GoNear(m_grid.Map[(int)newPos.x, (int)newPos.y].transform.position);
//    }

//    #endregion


//    #region SPELLS

//    public void HandleSpellButtonClick(int idSpell)
//    {
//        if (m_grid.GetCurrentAction() == GridFight.Action.Spell)
//        {
//            m_grid.DeactivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
//            m_grid.SetCurrentAction(GridFight.Action.Movement);
//            m_spellUsedID = -1;
//        }
//        else
//        {
//            m_grid.ActivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
//            m_grid.SetCurrentAction(GridFight.Action.Spell);
//            m_spellUsedID = idSpell;
//        }
//    }

//    public void SetHUDSpellButtons()
//    {
//        List<int> ids = new List<int>();
//        List<Sprite> sprites = new List<Sprite>();

//        for (int i = 0; i < m_spellTree.GetSpells().Count; i++)
//        {
//            ids.Add(i);
//            sprites.Add(Resources.Load<Sprite>("SpellsIcons/" + m_spellTree.GetSpells()[i].name));
//            if(sprites[i] == null)
//            {
//                Debug.Log("Image null");
//            }
//        }

//        m_HUDUIManager.FillCallbacksAndIconsSpellButtons(HandleSpellButtonClick, ids, sprites);
//    }

//    public void TryToActivateSpell(Vector2 XY)
//    {
//        if (m_spellUsedID == -1 || m_secondsLeftInTurn <= 0)
//            return;

//        int dist = (int)MathsUtils.CircleDistance(XY, m_positionArrayFight);
//        if (dist <= (int)m_spellTree.GetSpells()[m_spellUsedID].range)
//        {
//            //Use spell
//            Debug.Log("Can use spell");
//            m_networkBattle.SendSpellHitMessage(XY, m_spellTree.GetSpells()[m_spellUsedID]._id);
//        }
//        else
//        {
//            //Don't use spell
//            Debug.Log("Can't use spell");
//        }
//        m_grid.DeactivateTileInRange((int)m_spellTree.GetSpells()[m_spellUsedID].range);
//        m_grid.SetCurrentAction(GridFight.Action.Movement);
//        m_spellUsedID = -1;

//    }

//    #endregion

//    #region NETWORK

//    public void SendMovementInFightMessage(Vector2 XY)
//    {
//        m_networkBattle.SendPositionBattle(XY);
//    }

//    #endregion

//    #endregion
//}
