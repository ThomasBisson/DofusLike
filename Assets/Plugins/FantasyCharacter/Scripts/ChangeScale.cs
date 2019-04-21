using UnityEngine;
using System.Collections;

public class ChangeScale : MonoBehaviour {

	public float startTime;
	public Vector3 finalScale;
	public float speed;


	bool bstart =false;
	// Use this for initialization
	void Start () {
		Invoke ("SetbStart", startTime);

	}
	
	// Update is called once per frame
	void Update () {
		if(!bstart) return;
		transform.localScale = Vector3.MoveTowards (transform.localScale, finalScale, speed*Time.deltaTime);
		if(transform.localScale == finalScale)
		{
			bstart = false;
		}
	
	}

	void SetbStart()
	{
		bstart = true;
	}
}
