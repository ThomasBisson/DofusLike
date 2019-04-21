using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedController : MonoBehaviour {

    // Use this for initialization
    Animator anim;
    public bool isFar = true;
	void Start () {
        anim = GetComponent<Animator>();
	}

    Coroutine co;
    public void attacked()
    {
        if(co != null)
        {
            StopCoroutine(co);
        }
        
        anim.SetBool("Attacked", true);
        co = StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(0.25f);
        co = null;
        anim.SetBool("Attacked", false);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
