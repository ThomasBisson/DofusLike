using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zhaoyunDemo : MonoBehaviour {

    public GameObject attackBullet;
    public GameObject magicBullet;
    public GameObject magic2Bullet;
    public GameObject ultimateBullet;
    public GameObject damageEffect1;
    public GameObject damageEffect2;
    public GameObject damageEffect3;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator delayBullet()
    {
        int count = 10;
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(attackBullet);
            PosBullet bullet = obj.GetComponent<PosBullet>();
            bullet.player = transform;
            Vector3 newPos = c.transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(2f, 5f), Random.Range(-5f, 5f));
            Vector3 attackedPos = MathUtil.findChild(c.transform, "attackedPivot").position;
            bullet.startPos = MathUtil.calcTargetPosByRotation(attackedPos, Quaternion.LookRotation(newPos - c.transform.position), 0f, 5f);
            bullet.tarPos = attackedPos - bullet.startPos + attackedPos;
            bullet.effectObj = damageEffect1;
            bullet.bulleting();
            yield return null;
            if (i % 9 == 0)
            {
               
                c.attacked();
                if (damageEffect2 != null)
                {
                    GameObject obj1 = GameObject.Instantiate(damageEffect2);
                    ParticlesEffect effect = obj1.AddComponent<ParticlesEffect>();
                    Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = MathUtil.findChild(target, "attackedPivot").position;
                    effect.play();
                }
            }
        }
    }
    void preAction(string actionName)
    {
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        string[] arr = actionName.Split('|');
        string name = arr[0];
        switch(name)
        {
            case AnimationName.Attack:
                if (damageEffect1 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect1);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();
                    Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = MathUtil.findChild(target, "attackedPivot").position;
                    effect.play();
                }
                c.attacked();
                break;
            case AnimationName.Magic:
                if (magicBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(magicBullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect1;
                    bullet.bulleting();
                }
                if (damageEffect2 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect2);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();
                    Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = MathUtil.findChild(target, "attackedPivot").position;
                    effect.play();
                }
                c.attacked();
                StartCoroutine(delayBullet());
                break;
            case AnimationName.Magic2:
                if (magic2Bullet != null)
                {
                    GameObject obj = GameObject.Instantiate(magic2Bullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect2;
                    bullet.bulleting();
                }
                if (damageEffect2 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect2);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();

                    effect.transform.position = GameObject.Find("bigzhangjiao (1)").transform.position;
                    effect.play();
                }
                c.attacked();
                break;
            case AnimationName.Ultimate:
                if (ultimateBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(ultimateBullet);
                    LightBullet bullet = obj.GetComponent<LightBullet>();
                    bullet.player = transform;
                    bullet.target = GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect3;
                    bullet.bulleting();
                }
                if(damageEffect3 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect3);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();

                    effect.transform.position = GameObject.Find("bigzhangjiao (1)").transform.position;
                    effect.play();
                }
                c.attacked();
                StartCoroutine(delayBullet());
                break;
        }
    }

    IEnumerator delayAttacked()
    {
        yield return new WaitForSeconds(1.5f);
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        c.attacked();
        //yield return new WaitForSeconds(2.5f);
        //c.attacked();
    }
}
