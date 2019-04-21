using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class ActionEffect:MonoBehaviour
{
	public ParticleSystem[] particles;
	float oldSpeed = 1f;
	public float reallyLifeTime = -1;
	Animator[] animators;
	TrailRenderer[] renders;
	void Awake()
	{
		particles = GetComponentsInChildren<ParticleSystem> ();
		animators = GetComponentsInChildren<Animator> ();
		renders = GetComponentsInChildren<TrailRenderer> ();
	}



	void Start()
	{

	}


	public void reset()
	{
		particles = GetComponentsInChildren<ParticleSystem> ();

		for(int i = 0; i < particles.Length; i++)
		{
			ParticleSystem particle = particles[i];
            ParticleSystem.MainModule main = particle.main;
            main.playOnAwake = false;
		}
	}
	float startTime;
	void Update()
	{
		bool needInActive = true;
        float nowTime = Time.timeSinceLevelLoad;
		if(reallyLifeTime > 0)
		{
			if(nowTime - startTime < reallyLifeTime)
			{
				needInActive = false;
			}
		}
		else
		{
			for(int i = 0; i < particles.Length; i++)
			{
				ParticleSystem particle = particles[i];
                ParticleSystem.MainModule main = particle.main;
                if (main.loop == false && main.duration + 3f < nowTime - startTime)
				{
					if(!particle.isStopped)
					{
						particle.Stop();
					}
				}
				if(!particle.isStopped)
				{
					needInActive = false;
				}
			}
		}


		if(needInActive)
		{
			gameObject.SetActive(false);
		}
	}

	public void play()
	{
		gameObject.SetActive (true);
		startTime = Time.timeSinceLevelLoad;
	}

	public void setSpeed(float speed)
	{
		for(int i = 0; i < particles.Length; i++)
		{
			ParticleSystem particle = particles[i];
            ParticleSystem.MainModule main = particle.main;
            main.simulationSpeed = speed;
		}
	}

	public void pause()
	{
		for(int i = 0; i < particles.Length; i++)
		{
			ParticleSystem particle = particles[i];
			if(particle.isPlaying)
			{
				particle.Pause(false);
			}
		}

		for(int i = 0; i < animators.Length; i++)
		{
			animators[i].speed = 0f;
		}
	}
	
	public void resume()
	{
		for(int i = 0; i < particles.Length; i++)
		{
			ParticleSystem particle = particles[i];
			if(particle.isPaused)
			{
				particle.Play(false);
			}
		}

		for(int i = 0; i < animators.Length; i++)
		{
			animators[i].speed = 1f;
		}
	}
	
	
	
	public void stop()
	{
		gameObject.SetActive (false);
	}
}

