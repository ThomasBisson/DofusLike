using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerFight : PlayerManager
{

    #region VARS

    [SerializeField]
    [GreyOut]
    private GridFight m_grid;

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
        RandomizePlayerPosition(true);
    }

    // Update is called once per frame
    public override void Update()
    {
        HandleMouvement();
    }

    #endregion

    #region METHODS

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
        Debug.Log("Stats : " + m_playerStats.CurrentMovementPoint == null);
        return m_playerStats.CurrentMovementPoint >= movementPointUsed;
    }

    public void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_positionPlayer = new Vector2(Random.Range(0, m_grid.Map.GetLength(0)), Random.Range(0, m_grid.Map.GetLength(1)));
        if (mustTeleportPlayer)
            transform.position = m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
    }

    public void HandleSpellButtonClick(int idSpell)
    {
        if (m_grid.IsMovementStopped())
        {
            m_grid.DeactivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
            m_grid.EnableMovement();
        }
        else
        {
            m_grid.ActivateTileInRange((int)m_spellTree.GetSpells()[idSpell].range);
            m_grid.StopMovement();
        }
    }
    public void SetHUDSpellButtons()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < m_spellTree.GetSpells().Count; i++)
            ids.Add(i);

        m_HUDUIManager.FillCallbackButtons(HandleSpellButtonClick, ids);
    }

    //TODO : make him go tile by tile from combat
    public virtual void GoNear(Vector3 clickPoint)
    {
        base.GoNear(clickPoint);
        m_targetPosition = clickPoint;
        m_targetPositionChanged = true;
        m_animator.SetBool("isRunning", true);
        transform.LookAt(m_targetPosition);
    }

    #endregion
}
