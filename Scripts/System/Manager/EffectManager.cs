/// <summary>
/// エフェクトマネージャー
/// 
/// 2012/12/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class EffectManager : Manager
{
	public enum UIEffctPriorityType
	{
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3,
		Best = 4,
		None_Hit = 5,
		None_Damage = 6,
		None_LevelUP = 7,
		
		Max
	}
	
	#region フィールド＆プロパティ
	public static EffectManager Instance;
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}
	#endregion

	#region 自殺エフェクト
	public static void CreateSelfDestroy(Vector3 position, Quaternion rotation, string assetPath, bool isPrefabValue=false)
	{
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = GameConstant.EffectPath.DefaultBundlePath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

		// インスタンス化
		ResourceLoad.Instantiate(GameConstant.EffectPath.DefaultBundlePath, GameConstant.SelfDestroyPath, position, rotation, Instance,
			(GameObject go) => {SelfDestroyEffect.Setup(go, Instance, bundlePath, assetPath, isPrefabValue);}
		);
	}
	#endregion

	#region 弾丸エフェクト
	public static void CreateBulletAmmo(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			{ return; }
		string assetPath = bullet.File;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		// 弾丸生成
		string bundlePath = bullet.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {BulletAmmo.Setup(go, Instance, target, targetPosition, caster, skillID, bulletSet);}
		);
	}
	public static void CreateBulletBirth(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			{ return; }
		string assetPath = bullet.File;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		// 弾丸生成
		string bundlePath = bullet.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {BulletBirth.Setup(go, Instance, target, targetPosition, caster, skillID, bulletSet);}
		);
	}
	public static void CreateBulletSelf(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			{ return; }

		// コリジョン弾丸生成
		//string path = GameConstant.SelfAmmoPath;
		// インスタンス化
		ResourceLoad.Instantiate(GameConstant.EffectPath.DefaultBundlePath, GameConstant.SelfAmmoPath, position, rotation, null,
			(GameObject go) => {BulletSelf.Setup(go, target, targetPosition, caster, skillID, bulletSet);}
		);
	}
	public static void CreateBulletFall(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			{ return; }
		string assetPath = bullet.File;
		if (string.IsNullOrEmpty(assetPath))
		{ return; }
		
		// 弾丸生成
		string bundlePath = bullet.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {BulletMeteor.Setup(go, Instance, target, targetPosition, caster, skillID, bulletSet);}
		);
	}
	public static void CreateBulletAlong(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			{ return; }
		string assetPath = bullet.File;
		if (string.IsNullOrEmpty(assetPath))
		{ return; }
		
		// 弾丸生成
		string bundlePath = bullet.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {BulletAlong.Setup(go, Instance, target, targetPosition, caster, skillID, bulletSet);}
		);
	}
	#endregion

	#region 消失エフェクト
	public static void CreateLost(Vector3 position, Quaternion rotation, SkillBulletMasterData bullet)
	{
		if (bullet == null)
			{ return; }
		string assetPath = bullet.LostFile;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = bullet.LostFileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

		// インスタンス化
		ResourceLoad.Instantiate(GameConstant.EffectPath.DefaultBundlePath, GameConstant.SelfDestroyPath, position, rotation, Instance,
			(GameObject go) => {SelfDestroyEffect.Setup(go, Instance, bundlePath, assetPath, false);}
		);
	}
	#endregion

	#region ヒットエフェクト
	public static void CreateHit(Vector3 position, Quaternion rotation, SkillBulletMasterData bullet)
	{
		if (bullet == null)
			{ return; }
		string assetPath = bullet.HitFile;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath =bullet.HitFileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

		// インスタンス化
		ResourceLoad.Instantiate(GameConstant.EffectPath.DefaultBundlePath, GameConstant.SelfDestroyPath, position, rotation, Instance,
			(GameObject go) => {SelfDestroyEffect.Setup(go, Instance, bundlePath, assetPath, false);}
		);
	}
	#endregion

	#region 軌跡エフェクト
	public static void CreateLocus(ObjectBase caster, SkillEffectMasterData effect)
	{
		if (caster == null)
			return;
		if (effect == null)
			return;
		string assetPath = effect.File;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		// アタッチヌル取得
		Transform attachTransform = caster.transform;
		if (!string.IsNullOrEmpty(effect.AttachNull))
		{
			attachTransform = caster.transform.Search(effect.AttachNull);
			if (attachTransform == null)
				attachTransform = caster.transform;
		}

		// 補正
		Vector3 position = attachTransform.position;
		Quaternion rotation = attachTransform.rotation;
		GameGlobal.AddOffset(effect, ref position, ref rotation, attachTransform.localScale);

		// アタッチするかしないか
		Manager shotManager = (effect.IsAttach ? null : Instance);

		// 弾丸生成
		string bundlePath = effect.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// エフェクトファイル読み込み
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, shotManager,
			(GameObject go) =>
			{
				LocusEffect.Setup(go, shotManager, caster, effect);
				if(effect.IsAttach)
				{
					// 接続オブジェクト
					EffectManager.CreateConnectObject(go, attachTransform, true);
				}
			}
		);
	}
	#endregion

	#region メテオエフェクト
#if UNITY_EDITOR
	/// <summary>
	/// メテオエフェクト作成
	/// </summary>
	/// <param name="target"></param>
	/// <param name="casterTeam"></param>
	public static void CreateMeteor(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, TeamType casterTeam)
	{
		// 初期位置
		if (target)
		{
			position.x += target.transform.position.x;
			position.z += target.transform.position.z;
		}
		
		// 攻撃側のメインタワーを攻撃者にする
		EntrantInfo caster = target.EntrantInfo;
		{
			MainTower mainTower = Entrant.Find<MainTower>(mt => mt.TeamType == casterTeam);
			if(mainTower != null)
			{
				caster = mainTower.EntrantInfo;
			}
		}

		// 攻撃パケット送信
		if (Scm.Client.GameListener.ConnectFlg)
		{
			BattlePacket.SendTeamSkill(caster, GameConstant.MeteorBulletSetID, target, position, rotation);
		}
		else
		{
			// メテオ作成
			EffectManager.CreateMeteor(GameConstant.MeteorBulletSetID, target, targetPosition, position, rotation, caster);
		}
	}
#endif
	/// <summary>
	/// メテオエフェクト作成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="caster"></param>
	public static void CreateMeteor(int bulletSetID, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, EntrantInfo caster)
	{
		// スキルデータ取得
		SkillBulletSetMasterData bulletSet;
		if (!MasterData.TryGetBulletSet(bulletSetID, out bulletSet))
			{ return; }
		// メテオファイル名
		string assetPath = bulletSet.Bullet.File;
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = bulletSet.Bullet.FileAssetPath;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {BulletMeteor.Setup(go, Instance, target, targetPosition, caster, bulletSet.SkillID, bulletSet);}
		);
	}
	#endregion

	#region 経験値エフェクト
	public static void CreateExp(ObjectBase target, Vector3 startPosition, Vector3 position, Quaternion rotation, string assetPath)
	{
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = string.Empty;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance,
			(GameObject go) => {ExpEffect.Setup(go, Instance, target, startPosition);}
		);
	}
	#endregion

	#region レベルアップエフェクト
	public static void CreateLevelUp(ObjectBase target, bool isCharaAttach)
	{
		string bundlePath = GameConstant.EffectPath.DefaultBundlePath;
		string assetPath = GameConstant.EffectLevelUp;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);

		// エフェクト本体
		ResourceLoad.Instantiate(GameConstant.EffectPath.DefaultBundlePath, GameConstant.SelfDestroyPath, target.transform.position, target.transform.rotation, null,
			(GameObject go) =>
			{
				SelfDestroyEffect.Setup(go, null, bundlePath, assetPath, false);
				// 接続オブジェクト
				EffectManager.CreateConnectObject(go, target.transform, false);
			}
		);
	}
	#endregion

	#region ダウンエフェクト
	public static void CreateDown(ObjectBase target, string assetPath)
	{
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = string.Empty;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, null,
			(GameObject go) => 
			{
				go.transform.parent = target.gameObject.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
			}
		);
	}
	#endregion
	
	#region プレーンエフェクト
	public static void Create(string assetPath, Vector3 position, Quaternion rotation)
	{
		if (string.IsNullOrEmpty(assetPath))
			{ return; }

		string bundlePath = string.Empty;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, position, rotation, Instance);
	}
	public static void Create(string assetPath, Transform parent)
	{
		if (string.IsNullOrEmpty(assetPath))
		{ return; }
		
		string bundlePath = string.Empty;
		GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
		// インスタンス化
		ResourceLoad.Instantiate(bundlePath, assetPath, null,
			(GameObject go) => 
			{
				if(parent == null)
				{
					Object.Destroy(go);
				}
				else
				{
					go.transform.parent = parent;
					go.transform.localPosition = Vector3.zero;
					go.transform.localRotation = Quaternion.identity;
				}
			}
		);
	}
	#endregion

	#region ターゲットマーカー
	/// <summary>
	/// エイミングマーカー作成
	/// </summary>
	public static void CreateAimingMarker(GUISkillButton button, Character caster, ObjectBase target, Vector3? targetPosition, float range, SkillBulletSetMasterData bulletSet, SkillAimingMarkerMasterData markerData)
	{
		Instance._CreateAimingMarker(button, caster, target, targetPosition, range, bulletSet, markerData);
	}
	void _CreateAimingMarker(GUISkillButton button, Character caster, ObjectBase target, Vector3? targetPosition, float range, SkillBulletSetMasterData bulletSet, SkillAimingMarkerMasterData markerData)
	{
		GameObject go = new GameObject("AimingMarker");
		this.Setup(go);
		AimingMarker marker = go.AddComponent<AimingMarker>();
		marker.Setup(button, caster, target, targetPosition, range, bulletSet, markerData);
	}
	/// <summary>
	/// キャストマーカー作成
	/// </summary>
	public static void CreateCastMarker(ObjectBase caster, ObjectBase target, Vector3? targetPosition, float range, SkillBulletSetMasterData bulletSet, SkillCastMarkerMasterData markerData)
	{
		if (caster == null)
			{ return; }

		Vector3 position;
		Quaternion rotation;
		MarkerBase.CalculateBirth(caster, target, targetPosition, range, caster.transform.position, bulletSet, out position, out rotation);
		position = new Vector3(position.x, 0f, position.z);	// y軸補正をなくす
		Instance._CreateCastMarker(caster, position, rotation, markerData);

		// パケット送信
		BattlePacket.SendSkillCastMarker(markerData.ID, position, rotation);
	}
	public static void CreateCastMarker(ObjectBase caster, Vector3 position, Quaternion rotation, SkillCastMarkerMasterData markerData)
	{
		Instance._CreateCastMarker(caster, position, rotation, markerData);
	}
	void _CreateCastMarker(ObjectBase caster, Vector3 position, Quaternion rotation, SkillCastMarkerMasterData markerData)
	{
		GameObject go = new GameObject("CastMarker");
		this.Setup(go);
		CastMarker marker = go.AddComponent<CastMarker>();
		marker.Setup(caster, position, rotation, markerData);
	}
	/// <summary>
	/// スキルマーカー作成
	/// </summary>
	public static SkillMarker CreateSkillMarker(BulletBase bullet, SkillMarkerMasterData markerData)
	{
		return Instance._CreateSkillMarker(bullet, markerData);
	}
	SkillMarker _CreateSkillMarker(BulletBase bullet, SkillMarkerMasterData markerData)
	{
		GameObject go = new GameObject("SkillMarker");
		this.Setup(go);
		SkillMarker marker = go.AddComponent<SkillMarker>();
		marker.Setup(bullet, markerData);
		return marker;
	}
	#endregion

	#region ConnectObject
	/// <summary>
	/// ConnectObject生成(EffectManager下に無いEffect用).
	/// </summary>
	private static void CreateConnectObject(GameObject obj, Transform attachTransform, bool isPrefabValue)
	{
		// 設計上同期読み必須 かつ スクリプトが一つ付くだけのオブジェクトなので生生成.
		GameObject connectObject = new GameObject(GameConstant.ConnectObjectStartName + obj.name);
		ConnectObject.Create(connectObject, Instance, obj, attachTransform, isPrefabValue);
		Instance.Setup(connectObject);
	}
	#endregion
}
