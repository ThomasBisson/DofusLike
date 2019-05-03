using System;
using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Mathematics;
using UnityEngine;

public class GridFight : GridParent
{
    #region VARS


    public static GridFight Instance;

    [Header("Groundgrids prefabs")]
    [SerializeField]
    private GameObject m_groundGridPrefab;

    private Action m_action = Action.Movement;
    public enum Action
    {
        Movement,
        Spell
    }

    private GroundGrid[,] m_map;

    public GroundGrid[,] Map
    {
        get { return m_map; }
        private set { m_map = value; }
    }

    public PlayerManager m_playerManager { get; set; }

    #endregion

    #region UNITY_METHOD

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }


    public override void Start()
    {
        base.Start();
        //Instantiate map
        m_map = new GroundGrid[m_totalSizeX, m_totalSizeZ];

        //Placing the tile on the grid
        GameObject go;
        float sizeXDivTwo = (float)m_totalSizeX / 2f;
        float sizeZDivTwo = (float)m_totalSizeZ / 2f;
        for (float x = -sizeXDivTwo; x < sizeXDivTwo; x += m_size)
        {
            for (float z = -sizeZDivTwo; z < sizeZDivTwo; z += m_size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                go = Instantiate(m_groundGridPrefab, point, Quaternion.identity, transform);
                go.name = "map[" + ((int)(x + sizeXDivTwo)) + ", " + ((int)(z + sizeZDivTwo)) + "]";
                var pos = go.transform.localPosition;
                pos.y = 0.01f;
                go.transform.localPosition = pos;
                m_map[(int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo)] = go.GetComponent<GroundGrid>();
                m_map[(int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo)].XY = new Vector2((int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo));
                m_map[(int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo)].m_OnHover = OnGridHover;
                m_map[(int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo)].m_OnStopHover = OnStopGridHover;
                m_map[(int)(x + sizeXDivTwo), (int)(z + sizeZDivTwo)].m_OnClicked = OnGridClicked;
            }
        }
    }

    #endregion

    #region GRID_TILES_CALLBACKS
    public void OnGridHover(Vector2 XY)
    {
        if (m_playerManager == null)
            return;

        if (m_playerManager.m_positionArrayFight == XY)
            return;

        m_map[(int)XY.x, (int)XY.y].AddColor(GroundGrid.PossibleDisplay.Pointed);

        if (m_action == Action.Movement)
        {
            m_astar.AstarRun(m_playerManager.m_positionArrayFight, XY);
            var path = m_astar.GetPath();
            if (m_playerManager.GetPlayerFight().CanMove(path.Count))
            {
                foreach (var tile in path)
                    m_map[tile.X, tile.Y].AddColor(GroundGrid.PossibleDisplay.Movement);
            }
        }
    }

    public void OnStopGridHover(Vector2 XY)
    {
        m_map[(int)XY.x, (int)XY.y].RemoveColor(GroundGrid.PossibleDisplay.Pointed);

        if (m_action == Action.Movement)
        {
            var path = m_astar.GetPath();
            foreach (var tile in path)
                m_map[tile.X, tile.Y].HideYourself();

            m_astar.ResetMapAndPath();
        }
    }

    public void OnGridClicked(Vector2 XY)
    {
        if (m_action == Action.Movement)
        {
            int dist = (int)MathsUtils.CircleDistance(XY, m_playerManager.m_positionArrayFight);
            if (!m_playerManager.GetPlayerFight().CanMove(dist))
                return;
            //PlayerManagerFight.GoNear(m_map[(int)XY.x, (int)XY.y].transform.position);
            m_playerManager.GetPlayerFight().SendMovementInFightMessage(XY);

            OnStopGridHover(XY);

            //PlayerManagerFight.m_positionArrayFight = XY;
        } else if(m_action == Action.Spell)
        {
            m_playerManager.GetPlayerFight().TryToActivateSpell(XY);
        }
    }

    #endregion

    #region METHODS

    public override Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        //int xCount = Mathf.RoundToInt(position.x / m_size);
        //int yCount = Mathf.RoundToInt(position.y / m_size);
        //int zCount = Mathf.RoundToInt(position.z / m_size);

        Vector3 result = new Vector3(
            (float)position.x * m_size + m_offsetX,
            (float)position.y * m_size,
            (float)position.z * m_size + m_offsetZ);

        result += transform.position;

        return result;
    }


    public void ActivateTileInRange(int range)
    {
        List<GroundGrid> tileInRange = new List<GroundGrid>();
        for (int i = -range; i < range + 1; i++)
            for (int j = -range; j < range + 1; j++)
                if (Math.Abs(i) + Math.Abs(j) <= range)
                {
                    if ((int)(m_playerManager.m_positionArrayFight.x - i) >= 0 && (int)(m_playerManager.m_positionArrayFight.x - i) < m_totalSizeX &&
                        (int)(m_playerManager.m_positionArrayFight.y + j) >= 0 && (int)(m_playerManager.m_positionArrayFight.y + j) < m_totalSizeZ)
                    {
                        m_map[(int)m_playerManager.m_positionArrayFight.x - i, (int)m_playerManager.m_positionArrayFight.y + j].AddColor(GroundGrid.PossibleDisplay.Spell);
                    }
                }
    }

    public void DeactivateTileInRange(int range)
    {
        List<GroundGrid> tileInRange = new List<GroundGrid>();
        for (int i = -range; i < range + 1; i++)
            for (int j = -range; j < range + 1; j++)
                if (Math.Abs(i) + Math.Abs(j) <= range)
                {
                    if ((int)(m_playerManager.m_positionArrayFight.x - i) >= 0 && (int)(m_playerManager.m_positionArrayFight.x - i) < m_totalSizeX &&
                        (int)(m_playerManager.m_positionArrayFight.y + j) >= 0 && (int)(m_playerManager.m_positionArrayFight.y + j) < m_totalSizeZ)
                    {
                        m_map[(int)m_playerManager.m_positionArrayFight.x - i, (int)m_playerManager.m_positionArrayFight.y + j].HideYourself();
                    }
                }
    }



    #region GETTER & SETTER

    public void SetCurrentAction(Action action) { m_action = action; }
    public Action GetCurrentAction() { return m_action; }

    #endregion

    #endregion


    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        float sizeXDivTwo = (float)m_totalSizeX / 2f;
        float sizeZDivTwo = (float)m_totalSizeZ / 2f;
        for (float x = -sizeXDivTwo; x < sizeXDivTwo; x += m_size)
        {
            for (float z = -sizeZDivTwo; z < sizeZDivTwo; z += m_size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }

        }
    }


}
