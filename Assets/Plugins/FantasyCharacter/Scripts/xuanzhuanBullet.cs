using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class xuanzhuanBullet:Bullet
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
        basePos = attackPos;
		
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

    float time = 0f;
    public float flag = 1f;
    Vector3 basePos;
	protected override void update ()
	{
		if(bulletState == BulletState.none)
		{
			return;
		}
        if(target == null || player == null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
		base.update ();
		Vector3 attackedPos = MathUtil.findChild(target, "attackedPivot").position;
        attackedPos += (target.position - player.position) * 10000f;
        time += Time.deltaTime;
        if (time > 3f)
		{
			complete();
			return;
		}

		basePos = MathUtil.calcTargetPosByDis (basePos, attackedPos, speed * Time.deltaTime);
		transform.forward = basePos - transform.position;
        Vector3 newPos = MathUtil.calcTargetPosByRotation1(basePos, Quaternion.LookRotation(Vector3.up), time * 360f * flag, 0.2f);
        transform.position = newPos;
	}
}
