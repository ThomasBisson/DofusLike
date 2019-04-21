using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIController : MonoBehaviour {

    // Use this for initialization
    public static string[] heros = new string[]{
       "zhugeliang","caohong", "simayi","zhaoyun","zhangjiao","huangyueying",  "lusu",  "xuchu",  "diaochan",  "zhouyu", 
    };
    int num = 10;
    int index = 0;
	void Start () {
        Button pre = transform.Find("Pre").GetComponent<Button>();
        pre.onClick.AddListener(preClick);
        Button next = transform.Find("Next").GetComponent<Button>();
        next.onClick.AddListener(nextClick);
        UpdateHero();
	}

    void preClick()
    {
        index--;
        if(index < 0)
        {
            index = num - 1;
        }
        UpdateHero();
    }

    void nextClick()
    {
        index++;
        if(index == num)
        {
            index = 0;
        }
        UpdateHero();
    }

    GameObject nowObj;
    void UpdateHero()
    {
        if(nowObj != null)
        {
            nowObj.SetActive(false);
            GameObject.Destroy(nowObj);
        }
        string name = heros[index];
        GameObject obj = Resources.Load("Unit/" + name) as GameObject;
        GameObject inst = GameObject.Instantiate(obj);
        ActionEffectManager c = inst.GetComponent<ActionEffectManager>();
        if(c.isFar)
        {
            inst.transform.position = new Vector3(1, 0, 0);
            inst.transform.rotation = Quaternion.Euler(0, -127f, 0f);
        }
        else
        {
            inst.transform.position = c.pos;
            inst.transform.rotation = Quaternion.Euler(0f, -117.2f, 0f);
        }
        nowObj = inst;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
