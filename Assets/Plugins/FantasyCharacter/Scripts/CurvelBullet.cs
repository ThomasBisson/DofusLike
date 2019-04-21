using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CurvelBullet:Bullet
{
	protected override void init ()
	{
		base.init ();
	
	}

    beisaier b;
    float time = 0f;
	public override void bulleting ()
	{
		base.bulleting ();
		Vector3 attackPos = MathUtil.findChild(player, "attackPivot").position;
		transform.position = attackPos;
		stopParticle ();
        Vector3 attackedPos = MathUtil.findChild(target, "attackedPivot").position;
        Vector3 pos1 = (attackedPos - transform.position) / 3 + transform.position;
        pos1.y += Random.Range(1f, 3f);
        pos1 += new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        Vector3 pos2 = 2f * (attackedPos - transform.position) / 3f + transform.position;
        pos2.y += Random.Range(0f, 1f);
        pos2 += new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        b = new beisaier(transform.position, pos1, pos2, attackedPos);
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
        time += Time.deltaTime * speed;
        if (time >= 1f)
		{
			complete();
			return;
		}
        Vector3 pos = b.GetPointAtTime(time);
		transform.forward = pos - transform.position;
		transform.position = pos;
	}
}
