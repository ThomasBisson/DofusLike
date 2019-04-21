using UnityEngine;
using System.Collections;
public class DelayEnabledTrainRender:MonoBehaviour
{
	public float delay = 0.5f;
	public float time = 0.5f;

	void OnEnable()
	{
		TrailRenderer render = GetComponent<TrailRenderer> ();
		render.time = 0;
		StartCoroutine (enableTime());
	}

	IEnumerator enableTime()
	{
		yield return new WaitForSeconds(delay);
		TrailRenderer render = GetComponent<TrailRenderer> ();
		render.time = time;
	}
}
