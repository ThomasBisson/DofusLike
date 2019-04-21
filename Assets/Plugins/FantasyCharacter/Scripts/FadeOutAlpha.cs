using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FadeOutAlpha : MonoBehaviour {

	public float startTime;
	public float fadeSpeed;

	bool isstarted =false;
	Renderer[] renderers;
	// Use this for initialization
	List<Color> alphaArr = new List<Color>();
	void Awake () {
		renderers = GetComponentsInChildren<Renderer> ();
		for(int i = 0; i < renderers.Length; i++)
		{
			Renderer rd = renderers[i];
			alphaArr.Add(rd.material.GetColor("_TintColor"));
		}
	}


	// Update is called once per frame
	void Update () {
		if(!isstarted) return;
		foreach(Renderer rd in renderers)
		{
			foreach(Material mat in rd.materials){
				mat.SetColor("_TintColor", new Color(rd.material.GetColor("_TintColor").r,
				                                  rd.material.GetColor("_TintColor").g,
				                                  rd.material.GetColor("_TintColor").b,
				                                  rd.material.GetColor("_TintColor").a-Time.deltaTime*0));
			}
		}
		
	}

	void SetStart()
	{
		isstarted = true;

	}


	void OnEnable()
	{
		isstarted = false;
		Invoke ("SetStart", startTime);
		if(renderers != null)
		{
			for(int i = 0; i < renderers.Length; i++)
			{
				Renderer rd = renderers[i];
				foreach(Material mat in rd.materials){
					mat.SetColor("_TintColor", alphaArr[i]);
				}
			}
		}
	}

	void OnDisable()
	{
		CancelInvoke ();
	}

}
