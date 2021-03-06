/// <summary>
/// マーカーベース
/// 
/// 2013/01/21
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class MarkerBase : MonoBehaviour
{
	#region マーカー作成
	/// <summary>
	/// マーカーオブジェクトを作成する
	/// </summary>
	protected void CreateMarkerObject(TeamType teamType, ISkillMarkerMasterData marker, System.Action<GameObject> setupFunc)
	{
		string assetPath;
		switch(teamType.GetClientTeam())
		{
			case TeamTypeClient.Friend:
				assetPath = marker.FileBlue;
				break;
			case TeamTypeClient.Enemy:
				assetPath = marker.FileRed;
				break;
			default:
				assetPath = marker.File;
				break;
		}

		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = string.Empty;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

		ResourceLoad.Instantiate(bundlePath, assetPath, null, (GameObject go)=>{setupFunc(go);});
	}
	public static void CalculateBirth(ObjectBase caster, ObjectBase target, Vector3? targetPosition, float range, Vector3 attachPosition, SkillBulletSetMasterData bulletSet, out Vector3 position)
	{
		Quaternion rotation;
		CalculateBirth(caster, target, targetPosition, range, attachPosition, bulletSet, out position, out rotation);
	}
	public static void CalculateBirth(ObjectBase caster, ObjectBase target, Vector3? targetPosition, float range, Vector3 attachPosition, SkillBulletSetMasterData bulletSet, out Vector3 position, out Quaternion rotation)
	{
		// 方向ベクトルを求める
		Vector3 direction;
		{
			float length = range;
			if (target == null)
			{
				// ターゲットなし
				// カメラ方向から最大射程の位置
				direction = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
				length = range;
			}
			else
			{
				// ターゲットあり
				// ターゲットの足元かターゲットが射程外なら最大射程の位置
				direction = (targetPosition.HasValue ? targetPosition.Value : target.transform.position) - caster.transform.position;
				length = Mathf.Min(range, direction.magnitude);
			}
			direction.Normalize();
			direction *= length;
		}

		// オフセットを求める
		Vector3 offsetPosition = Vector3.zero;
		{
			Quaternion r = caster.transform.rotation;
			GameGlobal.AddOffset(bulletSet, ref offsetPosition, ref r, Vector3.one);
		}

		// 位置を求める
		position = attachPosition + offsetPosition + direction;
		// 角度を求める
		Vector3 forward = new Vector3(direction.x, 0f, direction.z);	// Y軸回転のみ適用する
		if (forward.sqrMagnitude > 0f)
			rotation = Quaternion.LookRotation(forward);
		else
			rotation = caster.transform.rotation;
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		Manager manager = EffectManager.Instance;
		if (manager)
		{
			manager.Destroy(this.gameObject);
		}
	}

	const float TweenTime = 0.2f;
	/// <summary>
	/// マーカー削除演出込み
	/// </summary>
	public void MarkerObjectDestroy()
	{
		Manager manager = EffectManager.Instance;

		this.enabled = false;

		// 縮小演出
		TweenScale.Begin(this.gameObject, TweenTime, Vector3.zero);
		if (manager)
		{
			manager.StartCoroutine(this.ManagerDestroy(manager, this.gameObject, TweenTime));
		}
		else
		{
			GameObject.Destroy(this.gameObject, TweenTime);
		}
	}
	IEnumerator ManagerDestroy(Manager manager, GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (obj)
		{
			manager.Destroy(obj);
		}
	}
	#endregion
}
