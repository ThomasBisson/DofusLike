using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BulletState
{
	none = 0,
	line = 1,
	wand = 2,
	lineRender = 3
}

public class Bullet:MonoBehaviour
{
    public float speed = 5f;
	public BulletState bulletState;
	protected ParticleSystem[] particles;
	TrailRenderer[] trails;
    public Transform player;
    public Transform target;
    public GameObject effectObj;
	void Awake()
	{
		init ();
	}


	public void setParam()
	{
		updateParticle ();
		bulletState = BulletState.none;
	}
	

	protected virtual void updateParticle()
	{

	}


	protected virtual void init()
	{
		particles = GetComponentsInChildren<ParticleSystem> ();


		trails = GetComponentsInChildren<TrailRenderer> ();
		for(int i = 0; i < trails.Length; i++)
		{
			trails[i].time = 0;
		}
	}

	protected void startParticle()
	{
		StartCoroutine (delaySetTrialTime ());
	}

	IEnumerator delaySetTrialTime()
	{
		yield return null;
		for(int i = 0; i < particles.Length; i++)
		{
			particles[i].gameObject.SetActive(true);
		}
		for(int i = 0; i < trails.Length; i++)
		{
			trails[i].time = 0.5f;
		}
	}

	protected void stopParticle()
	{
		for(int i = 0; i < particles.Length; i++)
		{
			if(particles[i].gameObject == gameObject)
			{
				continue;
			}
			particles[i].gameObject.SetActive(false);
		}
		for(int i = 0; i < trails.Length; i++)
		{
			trails[i].time = 0;
		}
	}



	void Update()
	{
		update ();
	}

	protected virtual void update()
	{

	}

    public int effectPos = 0;
	protected virtual void complete()
	{
        if(target == null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        AttackedController c = target.GetComponent<AttackedController>();
        if(c != null)
        {
            c.attacked();
        }
		stopParticle ();
        if (effectObj != null)
        {
            GameObject obj = GameObject.Instantiate(effectObj);
            ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();
            if(effectPos == 0)
            {
                effect.transform.position = MathUtil.findChild(target, "attackedPivot").position;
            }
            else
            {
                effect.transform.position = target.position;
            }
            effect.play();
        }
        GameObject.Destroy(gameObject);
	}

	public virtual void bulleting()
	{
		gameObject.SetActive (true);
	}




	void OnDestroy()
	{
	}
}
