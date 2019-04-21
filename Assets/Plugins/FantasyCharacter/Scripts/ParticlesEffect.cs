using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ParticlesEffect:BaseEffect
{
	public ParticleSystem[] particles;

	protected override void preInit ()
	{
		base.preInit ();
        init();
	}

	public void init()
	{
		particles = GetComponentsInChildren<ParticleSystem> ();
	}

	bool fadeOut = false;

	public void reset()
	{
		if(fadeOut)
		{
			for(int i = 0; i < colorList.Count && i < particles.Length; i++)
			{
                ParticleSystem.MainModule main = particles[i].main;
                main.startColor = colorList[i];
			}

			colorList.Clear();
		}

		fadeOut = false;
	}
	float startTime;
    public float time = -1f;
	protected override void update()
	{
		bool canReclaim = true;
		if(fadeOut)
		{
			for(int i = 0; i < particles.Length; i++)
			{
                ParticleSystem.MainModule main = particles[i].main;
                Color color = main.startColor.color;
				color.a -= Time.deltaTime / 2f;
				if(color.a > 0)
				{
					canReclaim = false;
				}
                main.startColor = color;
			}

			if(canReclaim)
			{
                GameObject.Destroy(gameObject);
			}
			return;
		}


		float nowTime = Time.timeSinceLevelLoad;
		if(time > 0)
		{
			float reallyLifeTime = time;
			if(nowTime - startTime < reallyLifeTime)
			{
				canReclaim = false;
			}
		}
		else
		{
			if(particles.Length == 0)
			{
				return;
			}
			for(int i = 0; i < particles.Length; i++)
			{
				ParticleSystem particle = particles[i];
                ParticleSystem.MainModule main = particles[i].main;
                if (main.loop == false && main.duration + 3f < nowTime - startTime && !particle.isStopped)
				{
					particle.Stop();
				}
				
				if(particle.isStopped == false)
				{
					canReclaim = false;
				}
			}
		}

		if(canReclaim)
		{
			stop();
		}
		
	}

	public void play()
	{
		startTime = Time.timeSinceLevelLoad;
	}
	
	
	List<Color> colorList = new List<Color>();
    public bool fade = true;
	public override void stop()
	{
		if(fade)
		{
			for(int i = 0; i < particles.Length; i++)
			{
				colorList.Add(particles[i].main.startColor.color);
			}
			fadeOut = true;
			return;
		}
		base.stop ();
        GameObject.Destroy(gameObject);
	}
}
