using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Algorithms.AStar;
using UnityEngine;

public class GridParent : MonoBehaviour
{
    #region VARS

    [Header("Grid creation")]
    [SerializeField]
    protected float m_size = 1f;
    [SerializeField]
    protected float m_offsetX = 0.5f;
    [SerializeField]
    protected float m_offsetZ = 0.5f;
    //[SerializeField]
    public int m_totalSizeX = 18;
    //[SerializeField]
    public int m_totalSizeZ = 10;

    //Astar
    protected AStar m_astar;

    #endregion

    #region UNItY_METHODS

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {
        m_astar = new AStar(m_totalSizeX, m_totalSizeZ);
    }

    #endregion


    #region METHODS

    public virtual Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / m_size);
        int yCount = Mathf.RoundToInt(position.y / m_size);
        int zCount = Mathf.RoundToInt(position.z / m_size);

        Vector3 result = new Vector3(
            (float)xCount * m_size + m_offsetX,
            (float)yCount * m_size,
            (float)zCount * m_size + m_offsetZ);

        result += transform.position;

        return result;
    }

    public Vector2 GetArrayPosition(Vector3 position)
    {
        return new Vector2(Mathf.RoundToInt(position.x / m_size), Mathf.RoundToInt(position.z / m_size));
    }

    #endregion

    #region GIZMO

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = -(m_totalSizeX / 2); x < (m_totalSizeX / 2); x += m_size)
        {
            for (float z = -((m_totalSizeZ / 2)); z < (m_totalSizeZ / 2); z += m_size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }

        }
    }

    #endregion

}
