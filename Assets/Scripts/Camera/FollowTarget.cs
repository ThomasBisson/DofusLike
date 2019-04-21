using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform m_target;
    public Transform Target
    {
        get { return m_target; }
        set
        {
            m_target = value;
            m_mustFolowTarget = true;
        }
    }
    private bool m_mustFolowTarget = false;

    // Update is called once per frame
    void Update()
    {
        if(m_mustFolowTarget)
        {
            var pos = transform.position;
            pos.x = m_target.position.x;
            pos.z = m_target.position.z;
            transform.position = pos;
        }
    }
}
