/// <summary>
/// 透過カメラ
/// 
/// 2013/03/29
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public class TransparentCamera : MonoBehaviour
{
	public Camera transparentCamera;
	public CharacterCamera mainCamera;
	public Shader shader;
	private RaycastHit[] rayHitWalls = new RaycastHit[0];
	/// <summary>
	/// カメラと目標との間のレイに衝突しているオブジェクト.
	/// </summary>
	private LinkedList<GameObject> transparentRayObjs = new LinkedList<GameObject>();
	/// <summary>
	/// カメラ自身が衝突しているオブジェクト.
	/// </summary>
	private LinkedList<GameObject> transparentTriggerObjs = new LinkedList<GameObject>();
	
	void Start()
	{
		// 使用シェーダを入れ替える.
		transparentCamera.SetReplacementShader(shader, "Replace");
	}
	
	void Update()
	{
		// プレイヤーの前にある壁を透過レイヤーに設定.
		if(mainCamera.getRayHitWall(ref rayHitWalls))
		{
			foreach(RaycastHit rayHitWall in rayHitWalls)
			{
				rayHitWall.transform.parent.gameObject.layer = LayerNumber.MapWallAlpha;
				transparentRayObjs.AddLast(rayHitWall.transform.parent.gameObject);
			}
		}
		// カメラ自身が衝突しているオブジェクトを透過レイヤーに設定.
		foreach(GameObject transparentObj in transparentTriggerObjs)
		{
			if(transparentObj && transparentObj.transform.parent.gameObject)
			{
				transparentObj.transform.parent.gameObject.layer = LayerNumber.MapWallAlpha;
			}
		}
	}
	
	/// <summary>
	/// カメラ自身の衝突処理.
	/// </summary>
	void OnTriggerEnter(Collider collision)
	{
		if(collision.transform.parent.gameObject)
		{
			collision.transform.parent.gameObject.layer = LayerNumber.MapWallAlpha;
			transparentTriggerObjs.AddLast(collision.gameObject);
		}
	}
	void OnTriggerExit(Collider collision)
	{
		if(collision.transform.parent.gameObject)
		{
			collision.transform.parent.gameObject.layer = 0;
		}
		transparentTriggerObjs.Remove(collision.gameObject);
	}
	
	void OnPostRender()
	{
		// 透過レイヤーの壁を元に戻す(現状Defaultのみだが、他のレイヤーを使うなら改変の必要有り).
		foreach(GameObject transparentObj in transparentRayObjs)
		{
			if(transparentObj)
			{
				transparentObj.layer = 0; // Default
			}
		}
		transparentRayObjs.Clear();
	}
}
