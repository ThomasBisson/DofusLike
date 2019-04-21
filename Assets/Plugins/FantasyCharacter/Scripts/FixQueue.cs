using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[ExecuteInEditMode]
/// <summary>
/// 挂在父物体上修正所有子物体深度
/// </summary>
public class FixQueue : MonoBehaviour {

	/// <summary>
	/// 需要修正的深度
	/// </summary>
	public int fixQueue;

	private Dictionary<Transform,int> _dicQueue=new Dictionary<Transform, int>();
	void Awake () {
		FixAllQueue();
	}
//	void Update(){
////		if(Application.isEditor&!Application.isPlaying){FixAllQueue();}//在编辑器环境下实时运行
//	}
//	/// <summary>
//	/// 修正渲染顺序
//	/// </summary>
	public void FixAllQueue(bool restore=false){
		int off = fixQueue;
		if(restore) 
			off = -fixQueue;

		_dicQueue.Clear ();
		if(_dicQueue!=null&&_dicQueue.Count<1)
			FindAllChild(transform);
		if(GetComponent<Renderer>()!=null)_dicQueue.Add(transform,GetComponent<Renderer>().material.renderQueue);//加上自己
		foreach(KeyValuePair<Transform,int> item in _dicQueue){
			item.Key.GetComponent<Renderer>().material.renderQueue=item.Value+off;
//			Debug.LogWarning("!!!!!!!!!!!!!!!!!!!!!!!item.Key.renderer.material.renderQueue : "+item.Key.renderer.material.renderQueue+ " gameObject : " +item.Key.name);	
		}
	}

	/// <summary>
	/// 遍历所有子物体
	/// </summary>
	/// <param name="tra">Tra.</param>
	void FindAllChild(Transform tra){
		foreach(Transform child in tra){
			if(child.GetComponent<Renderer>()!=null){
				_dicQueue.Add(child,child.GetComponent<Renderer>().material.renderQueue);
			}
			FindAllChild(child);
		}

	}
}
