/// <summary>
/// Buffエフェクト
/// 
/// 2013/09/20
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BuffEffect : MonoBehaviour
{
	#region フィールド＆プロパティ
	public ObjectBase ParentObject { get; private set; }
	public BuffType BuffType { get; private set; }
	private bool isAlive;
	public bool IsAlive
	{
		get
		{
			return isAlive;
		}
		set
		{
			isAlive = value;
			if(!isAlive)
			{
				// 自壊チェックコルーチンを起動.
				DestroyCheck();
			}
		}
	}

	#endregion

	#region セットアップ
	public static BuffEffect Create(ObjectBase objectBase, BuffType buffType)
	{
		// BuffEffectスクリプト用のGameObjectを作成.
		GameObject go = new GameObject(objectBase + "_" + buffType);

		// BuffEffectスクリプト作成.
		BuffEffect effect = go.AddComponent<BuffEffect>();
		effect.ResetParent(objectBase);
		effect.Setup(objectBase, buffType);

		return effect;
	}

	public void ResetParent(ObjectBase objectBase)
	{
		// Rootボーンにつける.
		Transform parent = objectBase.AvaterModel.RootTransform;
		if(parent == null)
		{
			parent = objectBase.AvaterModel.ModelTransform;
			if(parent == null)
			{
				parent = objectBase.transform;
			}
		}
		this.transform.SetParent(parent, false);
	}

	public void Setup(ObjectBase objectBase, BuffType buffType)
	{
		IsAlive = true;
		
		ParentObject = objectBase;
		BuffType = buffType;
		
		StateEffectMasterData masterData;
		if(MasterData.TryGetStateEffect((int)buffType, out masterData))
		{
			string bundlePath = masterData.ContinualEffectFileAssetPath;
			string assetPath = masterData.ContinualEffectFile;

			if(!string.IsNullOrEmpty(assetPath))
			{
				GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

				// RotateFlgがfalseの場合は親のRotationを無視する.
				if(!masterData.IsRotate)
				{
					this.gameObject.AddComponent<RotationReset>();
				}

				ResourceLoad.Instantiate(bundlePath, assetPath, null, (GameObject go) => 
				{
					if(go == null) { return; }
					if(this.gameObject == null) { GameObject.Destroy(go); return; }

					go.transform.SetParent(this.gameObject.transform, false);
					if(objectBase is Player)
					{
						// カメラエフェクト.
						var cameraEffect = go.GetComponent<ReferenceCameraEffectData>();
						if(cameraEffect != null)
						{
							cameraEffect.CreatePrefab();
						}
					}
				});
			}
		}
	}

	#endregion

	#region 破棄
	void OnDestroy()
	{
		this.Destroy();
	}
	public void Destroy()
	{
		GameObject.Destroy(this.gameObject);
	}
	#endregion

	#region 自壊チェックコルーチン
	public void DestroyCheck()
	{
		try
		{
			StartCoroutine(DestroyCoroutine());
		} 
		catch
		{
			// 既にgameobjectが破棄されている場合UnityEngine.MissingReferenceExceptionになる
			// 現状、他にこの状態を判定する方法がない(this.gameobject != null を実行しても上記のExceptionになる)のでtry～catchで処置
		}
	}
	IEnumerator DestroyCoroutine()
	{
		yield return new WaitForEndOfFrame();
		
		if(!IsAlive)
		{
			Destroy();
		}
	}

	#endregion
}

/// <summary>
/// エフェクト用回転無視.
/// </summary>
public class RotationReset : MonoBehaviour
{
	void LateUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}