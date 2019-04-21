using UnityEngine;
using System.Collections;
using System;
public class Delay : MonoBehaviour {
	
	public float delayTime = 1.0f;
	bool flag = false;
	long timeId;
	bool isDone = false;

	public bool checkIsDone()
	{
		return isDone;
	}

	public void resetFlag()
	{
		flag = false;
	}

	Delay[] delayArr = null;

	void OnEnable()
	{
		if(flag)
		{
			flag = false;
			return;
		}

		if(delayArr == null)
		{
			delayArr = GetComponentsInParent<Delay> ();
		}

		for(int i = 0; i < delayArr.Length; i++)
		{
			if(delayArr[i] == this)
			{
				continue;
			}
			if(!delayArr[i].checkIsDone())
			{
				return;
			}
		}

		flag = true;
		gameObject.SetActive (false);
        Invoke("DelayFunc", delayTime);
	}
	

	void DelayFunc()
	{
		isDone = true;
		gameObject.SetActive (true);
		//gameObject.SetActiveRecursively(true);
	}

	void OnDisable()
	{
		isDone = false;
	}
}
