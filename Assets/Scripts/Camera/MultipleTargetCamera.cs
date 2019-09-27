using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to render all of the maze
/// </summary>
[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{

    #region Vars

    //This pragma disable " Field 'XXXXX' is never assigned to, and will always have its default value null"
    //It appears when you use [SerializedFlied] and there is no other solution than disable the warning
#pragma warning disable 0649

    [Header("Targets")]
    /// <summary>
    /// The targets of the camera
    /// </summary>
    [SerializeField]
    //GreyOut will put the value in "read only" in the inspector
    [GreyOut]
    private List<Transform> m_targets = new List<Transform>();

    [Header("Movements")]
    //The offset of the camera
    [SerializeField]
    private Vector3 m_offset;
    //The smoothed time that take the camera to move
    [SerializeField]
    private float m_smoothTime = .5f;

#pragma warning restore 0649

    //Velocity when camera move
    private Vector3 m_velocity;
    //The camera
    private Camera m_cam;

    //1 tile of the maze = 2.5 in orthgraphic camera
    private const float m_sizeOrthographic = 2.5f;

    #endregion

    #region UnityMethods

    private void Start()
    {
        m_cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (m_targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    #endregion

    #region Methods

    #region Zoom

    /// <summary>
    /// Increase or decrease orthographic size in function of targets
    /// </summary>
    private void Zoom()
    {
        float newZoom = GetGreatestDistance();
        //5 is the scale of my walls, it gives me the size of the maze
        newZoom /= 5f;
        newZoom *= m_sizeOrthographic;
        m_cam.orthographicSize = Mathf.Lerp(m_cam.orthographicSize, newZoom, Time.deltaTime);
    }

    /// <summary>
    /// Get the greatest distance between the targets
    /// </summary>
    /// <returns></returns>
    private float GetGreatestDistance()
    {
        var bounds = new Bounds();
        for (int i = 0; i < m_targets.Count; i++)
            bounds.Encapsulate(m_targets[i].position);

        if (bounds.size.x >= bounds.size.z)
            return bounds.size.x;
        else
            return bounds.size.z;
    }

    #endregion

    #region Move

    /// <summary>
    /// Move at the center of the targets and apply an offset
    /// </summary>
    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + m_offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref m_velocity, m_smoothTime);
    }

    /// <summary>
    /// Get the center point between of all the targets
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenterPoint()
    {
        if (m_targets.Count == 1)
        {
            return m_targets[0].position;
        }

        var bounds = new Bounds(m_targets[0].position, Vector3.zero);
        for (int i = 0; i < m_targets.Count; i++)
        {
            bounds.Encapsulate(m_targets[i].position);
        }

        return bounds.center;
    }

    #endregion

    #region Targets

    /// <summary>
    /// Add one target to the list of targets
    /// </summary>
    /// <param name="transf"></param>
    public void AddTarget(Transform transf) { m_targets.Add(transf); }
    /// <summary>
    /// Add targets to the list of targets
    /// </summary>
    /// <param name="transfs"></param>
    public void AddTargets(List<Transform> transfs) { m_targets.AddRange(transfs); }
    /// <summary>
    /// Clear the list of targets
    /// </summary>
    public void ResetTargets() { m_targets.Clear(); }

    #endregion

    #endregion

}
