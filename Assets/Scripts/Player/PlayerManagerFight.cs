using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerFight : PlayerManager
{

    #region VARS

    [SerializeField]
    [GreyOut]
    private GridFight m_grid;

    private int m_spellUsedID;

    #endregion

    #region UNITY_METHODS

    public override void Awake()
    {
        base.Awake();
        m_grid = FindObjectOfType<GridFight>();
    }

    public override void Start()
    {
        base.Start();
        m_grid.PlayerManagerFight = this;
        transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
    }

    // Update is called once per frame
    public override void Update()
    {
        HandleMouvement();
    }

    #endregion

    #region METHODS

    #region MOVEMENTS

    private void HandleMouvement()
    {
        if (m_targetPositionChanged)
        {
            if (m_targetPosition == transform.position)
            {
                m_animator.SetBool("isRunning", false);
                m_targetPositionChanged = false;
            }
            else
            {
                var step = m_speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, step);
            }
        }
    }

    public bool CanMove(int movementPointUsed)
    {
        return m_playerStats.CurrentMovementPoint >= movementPointUsed;
    }

    public void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_positionArrayFight = new Vector2(Random.Range(0, m_grid.Map.GetLength(0)), Random.Range(0, m_grid.Map.GetLength(1)));
        if (mustTeleportPlayer)
            transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
    }

    //TODO : make him go tile by tile from combat
    public override void GoNear(Vector3 clickPoint)
    {
        base.GoNear(clickPoint);
        m_targetPosition = clickPoint;
        m_targetPositionChanged = true;
        m_animator.SetBool("isRunning", true);
        transform.LookAt(m_targetPosition);
    }

    #endregion


    #region SPELLS

    public void HandleSpellButtonClick(int idSpell)
    {
        if (m_grid.GetCurrentAction() == GridFight.Action.Spell)
        {
            m_grid.DeactivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
            m_grid.SetCurrentAction(GridFight.Action.Movement);
            m_spellUsedID = -1;
        }
        else
        {
            m_grid.ActivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
            m_grid.SetCurrentAction(GridFight.Action.Spell);
            m_spellUsedID = idSpell;
        }
    }

    public void SetHUDSpellButtons()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < m_spellTree.GetSpells().Count; i++)
            ids.Add(i);

        m_HUDUIManager.FillCallbacksSpellButtons(HandleSpellButtonClick, ids);
    }

    public void TryToActivateSpell(Vector2 XY)
    {
        if (m_spellUsedID == -1)
            return;

        int dist = (int)MathsUtils.CircleDistance(XY, m_positionArrayFight);
        if (dist <= (int)m_spellTree.GetSpells()[m_spellUsedID].range)
        {
            //Use spell
            Debug.Log("Can use spell");
            m_networkBattle.SendSpellHitMessage(XY, m_spellTree.GetSpells()[m_spellUsedID]._id);
        }
        else
        {
            //Don't use spell
            Debug.Log("Can't use spell");
        }
        m_grid.DeactivateTileInRange((int)m_spellTree.GetSpells()[m_spellUsedID].range);
        m_grid.SetCurrentAction(GridFight.Action.Movement);
        m_spellUsedID = -1;

    }

    #endregion

    #endregion
}
