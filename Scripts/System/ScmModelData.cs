/// <summary>
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScmModelData : ScriptableObject
{
	#region フィールド＆プロパティ
	public List<GameObject> meshPrefabs = new List<GameObject>();

	[SerializeField]
	private AnimationReference animationReference;
	public AnimationReference AnimationReference { get { return animationReference; } }
	#endregion

	#region メソッド.
	public GameObject CreatePrefab(Transform parent)
	{
		GameObject ret = null;
		foreach(GameObject prefab in meshPrefabs)
		{
			GameObject go = this.Instantiate(prefab, parent);
			if(ret == null)
			{
				ret = go;
			}
		}

		return ret;
	}

	/// <summary>
	/// インスタンス化
	/// </summary>
	private GameObject Instantiate(GameObject prefab, Transform parent)
	{
		GameObject go = SafeObject.Instantiate(prefab) as GameObject;
		if(go != null)
		{
			// 親子付け
			go.transform.parent = parent;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}
		return go;
	}
	#endregion
}
