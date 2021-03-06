/// <summary>
/// FBX カメラコンバータ
/// FBX の「ScaleXにNear」「ScaleYにFar」「ScaleZにFOV」を焼きこむ
/// 
/// 2013/07/03
/// </summary>
using UnityEngine;
using System.Collections;

public class CameraConverter : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private Camera _camera;
	public  Camera Camera { get { return _camera; } private set { _camera = value; } }
	#endregion

	#region 初期化
	void Start()
	{
		if (this.Camera != null)
		{
			this.Camera = this.gameObject.GetComponentInChildren<Camera>();
		}
	}
	#endregion

	#region 更新
	void LateUpdate()
	{
		if (this.Camera == null)
			{ return; }

		this.Camera.nearClipPlane = this.transform.localScale.x;
		this.Camera.farClipPlane = this.transform.localScale.y;
		this.Camera.fieldOfView = this.transform.localScale.z;
	}
	#endregion
}
