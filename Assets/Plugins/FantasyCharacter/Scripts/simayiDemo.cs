using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simayiDemo : MonoBehaviour {

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

    void preAction(string actionName)
    {
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
                    bullet.effectObj = damageEffect1;
                    bullet.bulleting();
                }
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
                StartCoroutine(delayBullet());
                break;
            case AnimationName.Ultimate:
                if (ultimateBullet != null)
                {
                    StartCoroutine(delayBullet());
                    StartCoroutine(delayBullet1());
                    StartCoroutine(delayBullet2());
                    StartCoroutine(delayBullet3());
                }
                break;
        }
    }

    IEnumerator delayBullet1()
    {
        int count = 20;
        float angle = -count / 2f * 5f;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            PosBullet bullet = obj.GetComponent<PosBullet>();
            bullet.player = transform;
            bullet.tarPos = MathUtil.calcTargetPosByRotation(transform, angle + i * 10f, 10f);
            bullet.effectObj = damageEffect1;
            bullet.bulleting();
            yield return new WaitForSeconds(0.1f);
            if (i % 9 == 0)
            {
                AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
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

    IEnumerator delayBullet2()
    {
        int count = 20;
        float angle = -count / 2f * 5f;
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            PosBullet bullet = obj.GetComponent<PosBullet>();
            bullet.player = transform;
            bullet.tarPos = MathUtil.calcTargetPosByRotation(transform.position, Quaternion.LookRotation(new Vector3(0, 1f, 0) + c.transform.position - transform.position), angle + i * 10f, 10f);
            bullet.effectObj = damageEffect1;
            bullet.bulleting();
            yield return new WaitForSeconds(0.1f);
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

    IEnumerator delayBullet3()
    {
        int count = 20;
        float angle = -count / 2f * 5f;
        AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            PosBullet bullet = obj.GetComponent<PosBullet>();
            bullet.player = transform;
            bullet.tarPos = MathUtil.calcTargetPosByRotation(transform.position, Quaternion.LookRotation(new Vector3(0, 2f, 0) + c.transform.position - transform.position), angle + i * 10f, 10f);
            bullet.effectObj = damageEffect1;
            bullet.bulleting();
            yield return new WaitForSeconds(0.1f);
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

    IEnumerator delayBullet()
    {
        int count = 30;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(ultimateBullet);
            PosBullet bullet = obj.GetComponent<PosBullet>();
            bullet.player = transform;
            AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
            Vector3 basePos = transform.position + c.transform.position;
            basePos /= 2f;
            basePos.y -= 3f;
            bullet.startPos = basePos;
            float padding = 5f;
            basePos += new Vector3(Random.Range(-padding, padding), 0f, Random.Range(-padding, padding));
            bullet.tarPos = basePos += new Vector3(0f, 10f, 0f);
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
}
