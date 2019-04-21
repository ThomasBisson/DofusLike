using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NormalBullet:Bullet
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

	protected override void update ()
	{
		if(bulletState == BulletState.none)
		{
			return;
		}
		base.update ();
		Vector3 attackedPos = MathUtil.findChild(target, "attackedPivot").position;

        if (Vector3.SqrMagnitude(attackedPos - transform.position) <= speed * Time.deltaTime)
		{
			complete();
			return;
		}

		Vector3 pos = MathUtil.calcTargetPosByDis (transform.position, attackedPos, speed * Time.deltaTime);
		transform.forward = pos - transform.position;
		transform.position = pos;
	}
}
