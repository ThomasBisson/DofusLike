using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFight : GridParent
{
    #region VARS


    public static GridFight Instance;

    [Header("Groundgrids prefabs")]
    [SerializeField]
    private GameObject m_groundGridPrefab;
    private bool m_isMovementStoped = false;
    private GroundGrid[,] m_map;

    public GroundGrid[,] Map
    {
        get { return m_map; }
        private set { m_map = value; }
    }

    public PlayerManagerFight PlayerManagerFight { get; set; }

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
        if (PlayerManagerFight == null)
            return;

        if (PlayerManagerFight.m_positionPlayer == XY)
            return;

        m_map[(int)XY.x, (int)XY.y].AddColor(GroundGrid.PossibleDisplay.Pointed);

        m_astar.AstarRun(PlayerManagerFight.m_positionPlayer, XY);
        var path = m_astar.GetPath();
        if (PlayerManagerFight.CanMove(path.Count))
        {
            foreach (var tile in path)
                m_map[tile.X, tile.Y].AddColor(GroundGrid.PossibleDisplay.Movement);
        }
    }

    public void OnStopGridHover(Vector2 XY)
    {
        var path = m_astar.GetPath();
        foreach (var tile in path)
            m_map[tile.X, tile.Y].HideYourself();

        m_astar.ResetMapAndPath();
    }

    public void OnGridClicked(Vector2 XY)
    {
        int dist = (int)MathsUtils.CircleDistance(XY, PlayerManagerFight.m_positionPlayer);
        if (!PlayerManagerFight.CanMove(dist))
            return;

        PlayerManagerFight.GoNear(m_map[(int)XY.x, (int)XY.y].transform.position);
        OnStopGridHover(XY);
        PlayerManagerFight.m_positionPlayer = XY;
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
                    if ((int)(PlayerManagerFight.m_positionPlayer.x - i) >= 0 && (int)(PlayerManagerFight.m_positionPlayer.x - i) < m_totalSizeX &&
                        (int)(PlayerManagerFight.m_positionPlayer.y + j) >= 0 && (int)(PlayerManagerFight.m_positionPlayer.y + j) < m_totalSizeZ)
                    {
                        m_map[(int)PlayerManagerFight.m_positionPlayer.x - i, (int)PlayerManagerFight.m_positionPlayer.y + j].AddColor(GroundGrid.PossibleDisplay.Spell);
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
                    if ((int)(PlayerManagerFight.m_positionPlayer.x - i) >= 0 && (int)(PlayerManagerFight.m_positionPlayer.x - i) < m_totalSizeX &&
                        (int)(PlayerManagerFight.m_positionPlayer.y + j) >= 0 && (int)(PlayerManagerFight.m_positionPlayer.y + j) < m_totalSizeZ)
                    {
                        m_map[(int)PlayerManagerFight.m_positionPlayer.x - i, (int)PlayerManagerFight.m_positionPlayer.y + j].HideYourself();
                    }
                }
    }

    public void EnableMovement()
    {
        m_isMovementStoped = false;
        foreach (var tile in m_map)
        {
            tile.m_OnHover = OnGridHover;
            tile.m_OnStopHover = OnStopGridHover;
            tile.m_OnClicked = OnGridClicked;
        }
    }

    public void StopMovement()
    {
        m_isMovementStoped = true;
        foreach (var tile in m_map)
        {
            tile.m_OnHover = null;
            tile.m_OnStopHover = null;
            tile.m_OnClicked = null;
        }
    }

    #region GETTER & SETTER

    public bool IsMovementStopped() { return m_isMovementStoped; }

    #endregion

    #endregion


    private void OnDrawGizmos()
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
