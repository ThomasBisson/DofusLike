using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    [GreyOut]
    private Transform m_target;

    public Transform Target
    {
        get { return m_target; }
        set
        {
            m_target = value;
            m_offset = transform.position - Vector3.zero;//m_target.position;
            m_mustFolowTarget = true;
        }
    }

    [SerializeField]
    [GreyOut]
    private bool m_mustFolowTarget = false;

    [SerializeField]
    [GreyOut]
    private Vector3 m_offset;

    // Update is called once per frame
    void Update()
    {
        if(m_mustFolowTarget)
        {
            //var pos = transform.position;
            //pos.x = m_target.position.x;
            //pos.z = m_target.position.z;
            //transform.position = pos;
            transform.position = m_target.position + m_offset;
        }
    }
}
