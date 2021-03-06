/// <summary>
/// 
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System;
using System.Collections;

public class AssetBundleBinder : MonoBehaviour
{
	[SerializeField]
	private string assetBundlePath;
	
	/// <summary>
	/// 読み込むアセットバンドルのPath.一度設定したら変更は非推奨.
	/// </summary>
	public string AssetBundlePath
	{
		get
		{
			return assetBundlePath;
		}
		set
		{
			if(assetBundlePath != value)
			{
				assetBundlePath = value;
				LoadAssetBundle();
			}
		}
	}
	public AssetReference.LoadState LoadState
	{
		get
		{
			if(assetReference != null)
			{
				return assetReference.loadState;
			}
			else
			{
				return AssetReference.LoadState.Failed;
			}
		}
	}
	private AssetReference assetReference;
	
	void Awake()
	{
		LoadAssetBundle();
	}
	
	void OnDestroy()
	{
		assetReference = null;
	}
	
	private void LoadAssetBundle()
	{
		if(string.IsNullOrEmpty(this.assetBundlePath))
		{
			assetReference = null;
		}
		else
		{
			//StopAllCoroutines();
			assetReference = AssetReference.GetAssetReference(this.assetBundlePath);
		}
	}
	public void SetAssetReference(AssetReference assetReference)
	{
		this.assetReference = assetReference;
		if(assetReference != null)
		{
			this.assetBundlePath = assetReference.BundlePath;
		}
		else
		{
			this.assetBundlePath = string.Empty;
		}
	}
	
	public T GetAsset<T>(string assetPath) where T : UnityEngine.Object
	{
		if(assetReference != null)
		{
			return assetReference.GetAsset<T>(assetPath);
		}
		return null;
	}

	public void GetAssetAsync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
	{
		if(assetReference != null)
		{
			StartCoroutine(assetReference.GetAssetAsync<T>(assetPath, callback));
		}
		else
		{
			StartCoroutine(WaitingGetAssetAsync<T>(assetPath, callback));
		}
	}

	private IEnumerator WaitingGetAssetAsync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
	{
		while(assetReference == null)
		{
			Debug.Log("WaitingGetAssetAsync");
			yield return null;
		}
		StartCoroutine(assetReference.GetAssetAsync<T>(assetPath, callback));
	}
}