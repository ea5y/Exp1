/// <summary>
/// モデルデータ処理.
/// 
/// 2015/02/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class AvaterModel : MonoBehaviour
{
	#region 定義.
	const string RootTransformName = "Root";
	#endregion

	#region フィールド＆プロパティ
	// モデルデータのTransform.
	private Transform modelTransform;
	public Transform ModelTransform
	{
		get
		{
			if (modelTransform == null)
			{
				modelTransform = GetModelTransform();
			}
			return modelTransform;
		}
	}
	// モデルデータのRootの位置.
	private Transform rootTransform;
	public Transform RootTransform
	{
		get
		{
			if (rootTransform == null && ModelTransform != null)
			{
				rootTransform = ModelTransform.SafeSearch(RootTransformName);
			}
			return rootTransform;
		}
	}
	public ObjectBase ObjectBase { get; private set; }
	public Renderer[] MeshRenderers { get; private set; }
	#endregion

	#region メソッド
	public void Setup(ObjectBase objectBase, AssetReference assetReference)
	{
		this.ObjectBase = objectBase;
        
		Net.Network.Instance.StartCoroutine(LoadModelDataCoroutine(assetReference));
	}
	private IEnumerator LoadModelDataCoroutine(AssetReference assetReference)
	{
		while(!assetReference.IsFinish)
		{
			yield return null;
		}
		
		var modelData = assetReference.GetAsset<ScmModelData>(CharacterName.ModelPath);
		if(modelData != null)
		{
			var animationData = modelData.AnimationReference;
			// 読み込み中にいなくなっていたら生成しない.
			if(this != null)
			{
				GameObject model = modelData.CreatePrefab(this.transform);
				this.modelTransform = model.transform;
				this.ObjectBase.LoadModelCompleted(model, animationData);
			}
		}
		yield break;
	}

	public void ChangeDrawEnable(bool isDraw)
	{
		if(this.MeshRenderers == null || this.MeshRenderers.Length == 0)
		{
			if(this.ModelTransform != null)
			{
				this.MeshRenderers = this.ModelTransform.GetComponentsInChildren<Renderer>();
			}
		}

		if(this.MeshRenderers != null)
		{
			foreach(var meshRenderer in this.MeshRenderers)
			{
				if(meshRenderer != null)
				{
					meshRenderer.enabled = isDraw;
				}
			}
		}
	}
	#endregion

	#region 旧メソッド
	private Transform GetModelTransform()
	{
		var rData = GetComponent<ReferenceData>();
		if (rData)
		{
			// meshPrefabsの0番目がモデルデータとみなす.
			if (0 < rData.meshPrefabs.Count)
			{
				return this.transform.SafeSearch(rData.meshPrefabs[0].name + "(Clone)");
			}
		}
		return null;
	}
	#endregion
}
