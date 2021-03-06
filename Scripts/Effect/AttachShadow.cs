/// <summary>
/// 影をアタッチする
/// 
/// 2013/12/16
/// </summary>
using UnityEngine;
using System.Collections;

public class AttachShadow : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private Transform attachTransform;
	public  Transform AttachTransform { get { return attachTransform; } }
	[SerializeField]
	private Vector3 shadowScale = Vector3.one;
	public  Vector3 ShadowScale { get { return shadowScale; } }
	#endregion

	#region 初期化
	void Start()
	{
		// 影を作成
		GameObject prefab = NpcManager.Instance.ShadowPrefab;
		GameObject go = SafeObject.Instantiate(prefab) as GameObject;
		if (go)
		{
			Transform attach = (this.attachTransform == null ? this.transform : this.attachTransform);
			Transform t = go.transform;
			t.parent = attach;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = this.ShadowScale;
		}
	}
	#endregion
}
