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
        for (int x = -(m_totalSizeX / 2); x < (m_totalSizeX / 2); x += 1)
        {
            for (int z = -(m_totalSizeZ / 2); z < (m_totalSizeZ / 2); z += 1)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                go = Instantiate(m_groundGridPrefab, point, Quaternion.identity, transform);
                var pos = go.transform.localPosition;
                pos.y = 0.01f;
                go.transform.localPosition = pos;
                m_map[x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2)] = go.GetComponent<GroundGrid>();
                m_map[x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2)].XY = new Vector2(x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2));
                m_map[x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2)].m_OnHover = OnGridHover;
                m_map[x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2)].m_OnStopHover = OnStopGridHover;
                m_map[x + (m_totalSizeX / 2), z + (m_totalSizeZ / 2)].m_OnClicked = OnGridClicked;
            }
        }
    }

    #endregion

    #region GRID_TILES_CALLBACKS
    public void OnGridHover(Vector2 XY)
    {
        m_astar.AstarRun(PlayerManagerFight.m_positionPlayer, XY);
        var path = m_astar.GetPath();
        if (!PlayerManagerFight.CanMove(path.Count))
            return;
        foreach (var tile in path)
            m_map[tile.X, tile.Y].DisplayYourself(GroundGrid.PossibleDisplay.Movement);
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

    #region METHOD

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
                        m_map[(int)PlayerManagerFight.m_positionPlayer.x - i, (int)PlayerManagerFight.m_positionPlayer.y + j].DisplayYourself(GroundGrid.PossibleDisplay.Spell);
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



}
