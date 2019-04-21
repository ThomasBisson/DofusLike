using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zhugeliangDemo : MonoBehaviour {

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
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        int count = 10;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            xuanzhuanBullet bullet = obj.GetComponent<xuanzhuanBullet>();
            bullet.player = transform;
            bullet.effectObj = damageEffect1;
            bullet.target = c.transform;
            bullet.bulleting();
            yield return new WaitForSeconds(0.1f);
            if(i % 9 == 0)
            {
                
                c.attacked();
            }
        }
    }

    IEnumerator delayBullet1()
    {
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        int count = 10;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            xuanzhuanBullet bullet = obj.GetComponent<xuanzhuanBullet>();
            bullet.player = transform;
            bullet.effectObj = damageEffect1;
            bullet.target = c.transform;
            bullet.flag = -1f;
            bullet.bulleting();
            yield return new WaitForSeconds(0.1f);
            if (i % 9 == 0)
            {

                c.attacked();
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
                if(attackBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(attackBullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect1;
                    bullet.bulleting();
                }
                break;
            case AnimationName.Magic:
                if (magicBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(magicBullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect2;
                    bullet.bulleting();


                }
                StartCoroutine(delayBullet());
                StartCoroutine(delayBullet1());
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
                break;
            case AnimationName.Ultimate:
                if (damageEffect3 != null)
                {
                    GameObject obj1 = GameObject.Instantiate(damageEffect3);
                    ParticlesEffect effect = obj1.AddComponent<ParticlesEffect>();
                    Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = MathUtil.findChild(target, "attackedPivot").position;
                    effect.play();
                }
                c.attacked();
                break;
        }
    }
}
