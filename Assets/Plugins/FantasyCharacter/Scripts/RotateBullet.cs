using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RotateBullet:Bullet
{
	protected override void init ()
	{
		base.init ();
	
	}


	public override void bulleting ()
	{
		base.bulleting ();
		Vector3 attackPos = MathUtil.findChild(player, "attackPivot").position;
		transform.position = attackPos;
		stopParticle ();
		StartCoroutine (delayBulleting ());
	}

    public float flag = 1f;
	IEnumerator delayBulleting()
	{
		yield return null;
		bulletState = BulletState.line;
		Vector3 attackPos = MathUtil.findChild(player, "attackPivot").position;
        transform.position = attackPos;
		startParticle ();
	}

	protected override void complete ()
	{
		base.complete ();
	}
    float time = 0f;
    float r = 0f;
    public float y;
	protected override void update ()
	{
		if(bulletState == BulletState.none)
		{
			return;
		}
        time += Time.deltaTime;
        r += Time.deltaTime * speed;
        if(time >= 5f)
        {
            complete();
            return;
        }
        if(player == null)
        {
            return;
        }
        Vector3 pos = MathUtil.calcTargetPosByRotation(player, 180f * time * flag, r, false);
        pos.y += y;
		base.update ();
        transform.forward = pos - transform.position;
        transform.position = pos;
        
		
	}
}
