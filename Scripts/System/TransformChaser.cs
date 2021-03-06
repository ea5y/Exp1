/// <summary>
/// 対象のTransformを追跡する.
/// 
/// 2014/06/19
/// </summary>
using UnityEngine;
using System.Collections;

public class TransformChaser : MonoBehaviour
{
	private Transform targetTransform;

	/// <summary>
	/// TransformChaserを作成する.
	/// </summary>
	static public TransformChaser Create(GameObject go, Transform target)
	{
		if(go != null)
		{
			var transformChaser = go.AddComponent<TransformChaser>();
			transformChaser.targetTransform = target;
			
			return transformChaser;
		}
		return null;
	}

	void Update()
	{
		if(targetTransform == null)
		{
			// 親子関係の場合と違い, target消滅後もGameObjectはその場に残る.
			Destroy(this);
			return;
		}
		
		this.transform.position = targetTransform.position;
		this.transform.rotation = targetTransform.rotation;
		{
			Vector3 lossyScale = transform.lossyScale;
			Vector3 localScale = transform.localScale;
			this.transform.localScale = new Vector3(localScale.x / lossyScale.x * targetTransform.lossyScale.x,
													localScale.y / lossyScale.y * targetTransform.lossyScale.y,
													localScale.z / lossyScale.z * targetTransform.lossyScale.z);
		}
	}
}

