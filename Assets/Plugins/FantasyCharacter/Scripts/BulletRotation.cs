using UnityEngine;
using System.Collections;

public class BulletRotation:MonoBehaviour
{
	public float speed = 720f;
	public bool isX = false;
	public bool isY = true;
	public bool isZ = false;
    float time = 0;
    public float allTime = 2f;
    private void OnEnable()
    {
        time = 0;
    }
    void Update()
	{
		if(isX)
		{
			transform.Rotate (speed * Time.deltaTime, 0, 0);
		}
		else if(isY)
		{
			transform.Rotate (0, speed * Time.deltaTime, 0);
		}
		else if(isZ)
		{
			transform.Rotate (0, 0, speed * Time.deltaTime);
		}
        time += Time.deltaTime;
        if(time > allTime)
        {
            gameObject.SetActive(false);
        }
	}
}
