using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerManager : Characters
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_gridMain = FindObjectOfType<GridMain>();
        transform.position = m_gridMain.GetNearestPointOnGrid(transform.position);
    }
}
