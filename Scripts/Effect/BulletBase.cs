/// <summary>
/// 弾丸ベース
/// 
/// 2013/01/29
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public abstract class BulletBase : MonoBehaviour
{
	#region 宣言
	/// <summary>
	/// 弾丸SE(飛行,ロスト)の管理クラス.
	/// </summary>
	private class BulletSound
	{
		CriAtomSource passSe;
		string lostSeName;
		public BulletSound(CriAtomSource passSe, string lostSeName)
		{
			this.passSe = passSe;
			this.lostSeName = lostSeName;
		}
		/// <summary>
		/// 飛行SEを停止しロストSEを再生する.複数回呼んでも最初の一回のみ効果がある.
		/// </summary>
		public void Stop(Vector3 position, Quaternion rotation)
		{
			if(passSe != null)
			{
				Object.Destroy(passSe);
			}
			SoundManager.CreateSeObject(position, rotation, lostSeName);
			lostSeName = null;
		}
		/// <summary>
		/// 飛行SEを停止する.ロストSEが今後再生されないようにする.
		/// </summary>
		public void StopByHit()
		{
			if(passSe != null)
			{
				Object.Destroy(passSe);
			}
			lostSeName = null;
		}
	}
	#endregion

	#region フィールド＆プロパティ
	[SerializeField]
	private float destroyCounter = 20f;
	public  float DestroyCounter { get { return destroyCounter; } private set { destroyCounter = value; } }
	[SerializeField]
	private float collisionCounter;
	public  float CollisionCounter { get { return collisionCounter; } private set { collisionCounter = value; } }
	[SerializeField]
	private LayerMask layerMask;
	public  LayerMask LayerMask { get { return layerMask; } protected set { layerMask = value; } }
	[SerializeField]
	private float raduis;
	public  float Radius { get { return raduis; } private set { raduis = value; } }
	[SerializeField]
	private int inFieldCasterID;
	public  int InFieldCasterID { get { return inFieldCasterID; } private set { inFieldCasterID = value; } }
	[SerializeField]
	private TeamType casterTeamType;
	public  TeamType CasterTeamType { get { return casterTeamType; } private set { casterTeamType = value; } }
	[SerializeField]
	private List<int> pierceingList = new List<int>();
	public  List<int> PierceingList { get { return pierceingList; } private set { pierceingList = value; } }
	[SerializeField]
	private float hitCounter;
	public  float HitCounter { get { return hitCounter; } private set { hitCounter = value; } }

	public Manager Manager { get; private set; }
	public ObjectBase Target { get; private set; }
	public EntrantInfo Caster { get; private set; }
	public int SkillID { get; private set; }
	public SkillBulletMasterData Bullet { get; private set; }
	public IBulletSetMasterData BulletSet { get; private set; }

	public Collider Collider { get; private set; }
	public BoxCollider BoxCollider { get; private set; }
	public SphereCollider SphereCollider { get; private set; }
	public Rigidbody Rigidbody { get; protected set; }
	private BulletSound bulletSound;
    public Vector3? TargetPosition;

	/// <summary>
	/// 子弾丸を既に生成済みかどうか.
	/// </summary>
	public bool IsCreatedChildBullet { get; private set; }

	/// <summary>
	/// キャスターの位置に追従するかどうか.
	/// </summary>
	public bool IsCasterTrace { get; private set; }
	#endregion

	#region セットアップ
	protected void SetupBase(Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		this.Manager = manager;
		this.Target = target;
        this.TargetPosition = targetPosition;
        this.Caster = caster;
		this.SkillID = skillID;
		this.Bullet = bulletSet.Bullet;
		if (this.Bullet == null)
		{
			string msg = string.Format("Bullet not found SkillID={0}", skillID);
			BugReportController.SaveLogFile(msg);
			Debug.LogWarning(msg);
			this.Destroy();
			return;
		}
		this.BulletSet = bulletSet;
		this.IsCasterTrace = this.BulletSet != null ? this.BulletSet.TraceFlag : false;
		if (0f != this.Bullet.DestroyTimer)
			{ this.DestroyCounter = this.Bullet.DestroyTimer; }
		if (0f != this.Bullet.CollisionEndTimer)
			{ this.CollisionCounter = this.Bullet.CollisionEndTimer; }
		if (0f != this.Bullet.HitInterval)
			{ this.HitCounter = this.Bullet.HitInterval; }

		if (caster != null)
		{
			this.InFieldCasterID = caster.InFieldId;
			this.CasterTeamType = caster.TeamType;
		}

		// レイヤー設定
		this.SetupLayer();
		this.SetupLayerMask();
		// マーカー設定
		SkillMarker marker = null;
		{
			SkillMarkerMasterData markerData;
			if (MasterData.TryGetSkillMarker(this.Bullet.ID, out markerData))
				marker = EffectManager.CreateSkillMarker(this, markerData);
		}
		// コリジョン設定
		this.SetupCollision(this.Bullet.CollisionRatio * 0.01f);
		this.SetupCollisionStart(this.Bullet.CollisionStartTimer, marker);

		// 弾丸オブジェクトに音源をつける(3D位置、寿命はオブジェクト依存になる).
		this.bulletSound = new BulletSound(SoundController.AddSeSource(gameObject, this.Bullet.PassSeFile), this.Bullet.LostSeFile);

		// Bulletモニターに登録.
		if(this.Caster != null)
		{
			Character chara = this.Caster.GameObject as Character;
			if(chara)
			{
				if(BattleMain.Instance != null)
				{
					BattleMain.Instance.BulletMonitor.AddBullet(this);
				}
				else
				{
#if VIEWER
					if(ViewerMain.BulletMonitor != null)
					{
						// ビュアーシーン時のみビュアーメイン内の弾丸管理クラスを呼び出す
						ViewerMain.BulletMonitor.AddBullet(this);
					}
#endif
				}
			}
		}
	}
	protected virtual void SetupLayer()
	{
		this.gameObject.layer = LayerNumber.Bullet;
	}
	protected virtual void SetupLayerMask()
	{
		this.LayerMask = GameController.BulletLayerMask;
	}
	void SetupCollision(float collisionRatio)
	{
		this.Collider = this.gameObject.GetComponentInChildren<Collider>();
		if (0 >= collisionRatio)
		{
			collisionRatio = 0.001f;
			if (GetComponent<Collider>())
				this.Collider.enabled = false;
		}

		this.BoxCollider = this.gameObject.GetComponentInChildren<BoxCollider>();
		if (this.BoxCollider)
		{
			this.BoxCollider.size *= collisionRatio;
			this.Radius = Mathf.Min(this.BoxCollider.size.x, Mathf.Min(this.BoxCollider.size.y, this.BoxCollider.size.z));
			this.Radius *= 0.5f;
		}
		this.SphereCollider = this.gameObject.GetComponentInChildren<SphereCollider>();
		if (this.SphereCollider)
		{
			this.Radius = this.SphereCollider.radius * collisionRatio;
			this.SphereCollider.radius = this.Radius;
		}
		this.Rigidbody = this.gameObject.GetComponentInChildren<Rigidbody>();
	}
	void SetupCollisionStart(float startTimer, SkillMarker marker)
	{
		if (startTimer <= 0f)
			{ return; }
		if (this.Bullet == null)
			{ return; }
		if (this.Collider == null)
			{ return; }
		if (!this.Collider.enabled)
			{ return; }

		this.StartCoroutine(this.CollisionStartCoroutine(startTimer, marker));
	}
	IEnumerator CollisionStartCoroutine(float startTimer, SkillMarker marker)
	{
		// マーカーがある場合は一旦コライダーのチェックをしないようにする
		// コライダーチェックでマーカーが消えてしまうため
		this.Collider.enabled = false;
		if (marker != null)
			marker.SetupCheckCollider(false);

		// タイマー
		if (0f < startTimer)
			{ yield return new WaitForSeconds(startTimer); }

		this.Collider.enabled = true;
		if (marker != null)
			marker.SetupCheckCollider(true);
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		// Bulletモニターから削除.
		if(this.Caster != null)
		{
			Character chara = this.Caster.GameObject as Character;
			if(chara)
			{
				if(BattleMain.Instance != null)
				{
					BattleMain.Instance.BulletMonitor.RemoveBullet(this);
				}
				else
				{
#if VIEWER
					if(ViewerMain.BulletMonitor != null)
					{
						// ビュアーシーン時のみビュアーメイン内の弾丸管理クラスを呼び出す
						ViewerMain.BulletMonitor.RemoveBullet(this);
					}
#endif
				}
			}
		}

		// マネージャーから削除.
		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
	}
	protected virtual void Destroy()
	{
		// 飛行SE停止,ロストSE再生.
		if(this.bulletSound != null)
		{
			this.bulletSound.Stop(this.transform.position, this.transform.rotation);
		}
		// 立体起動.
		Player player = this.Caster.GameObject as Player;
		if(player)
		{
			Skill3dManeuverGearMasterData maneuverGear3d;
			if(MasterData.TryGet3dManeuverGear(this.Bullet.ID, out maneuverGear3d))
			{
				player.ManeuverGear3D(maneuverGear3d, this, false);
			}
		}
		Object.Destroy(this.gameObject);
	}
	/// <summary>
	/// 何かにヒットして消滅.
	/// 必然的に発生弾系からは呼ばれない.
	/// </summary>
	public void DestroyByHit()
	{
		// 飛行SE停止,ロストSE再生.
		if(this.bulletSound != null)
		{
			this.bulletSound.StopByHit();
		}

		// 子弾丸生成.
		this.CreateChildBullet();

		// 立体起動.
		Player player = this.Caster.GameObject as Player;
		if(player)
		{
			Skill3dManeuverGearMasterData maneuverGear3d;
			if(MasterData.TryGet3dManeuverGear(this.Bullet.ID, out maneuverGear3d))
			{
				player.ManeuverGear3D(maneuverGear3d, this, true);
			}
		}
		this.Destroy();
	}
	protected void DestroyByDistance()
	{
		this.CreateChildBullet();
		this.Destroy();
	}
	protected virtual void DestroyTimer()
	{
		this.CreateChildBullet();
		this.Destroy();
	}
	protected virtual void DestroyCollision()
	{
		if (this.Collider == null)
			{ return; }

		this.Collider.enabled = false;
		// 飛行SE停止,ロストSE再生.
		if(this.bulletSound != null)
		{
			this.bulletSound.Stop(this.transform.position, this.transform.rotation);
		}
	}
	public virtual void DestroyByCaster()
	{
		if(this.Bullet.IsVanishable)	// Casterの状態によって消滅するかのフラグ.
		{
			this.CreateChildBullet(true);
			this.Destroy();
		}
	}
	#endregion

	#region 子弾丸生成
	private void CreateChildBullet(bool destroyByCaster = false)
	{
		if (this.IsCreatedChildBullet)
			{ return; }
		if (this.Bullet == null)
			{ return; }
		// 子弾丸がない
		List<SkillBulletBulletSetMasterData> childBulletSetList;
		if (!MasterData.TryGetChildBulletSetList(this.Bullet.ID, out childBulletSetList))
			{ return; }
		if (BattleMain.Instance == null)
			{ return; }

		foreach (var childBulletSet in childBulletSetList)
		{
			if(destroyByCaster && childBulletSet.Bullet != null && childBulletSet.Bullet.IsVanishable)
			{
				// 親弾丸がDestroyByCasterされた場合,子弾丸もDestroyByCasterする.
				continue;
			}

			if(BattleMain.Instance != null)
			{
				BattleMain.Instance.BulletMonitor.AddChildBullet(this.Caster, this.Target, this.transform.position, this.transform.rotation, this.transform.localScale, this.SkillID, childBulletSet);
			}
			else
			{
#if VIEWER
				if(ViewerMain.BulletMonitor != null)
				{
					// ビュアーシーン時のみビュアーメイン内の弾丸管理クラスを呼び出す
					ViewerMain.BulletMonitor.AddChildBullet(this.Caster, this.Target, this.transform.position, this.transform.rotation, this.transform.localScale, this.SkillID, childBulletSet);
				}
#endif
			}
		}

		// 子弾丸生成フラグを立てる
		// ホーミング弾で飛距離ギリギリの所で当たると子弾丸が二回生成されるのを防ぐ
		this.IsCreatedChildBullet = true;
	}
	#endregion

	#region 更新
	protected virtual void Update()
	{
		// 自身を消滅
		// 初期化時にタイマーが設定されてなければ何もしない
		if (0f < this.DestroyCounter)
		{
			this.DestroyCounter -= Time.deltaTime;
			if (0f >= this.DestroyCounter)
				{ this.DestroyTimer(); }
		}

		// 当たり判定消滅
		// 初期化時にタイマーが設定されてなければ何もしない
		if (0f < this.CollisionCounter)
		{
			this.CollisionCounter -= Time.deltaTime;
			if (0f >= this.CollisionCounter)
				{ this.DestroyCollision(); }
		}

		// ヒットカウンター(貫通弾や発生弾)
		// 初期化時にタイマーが設定されてなければ何もしない
		if (0f < this.HitCounter)
		{
			this.HitCounter -= Time.deltaTime;
			if (0f >= this.HitCounter)
			{
				this.PierceingList.Clear();
				this.HitCounter = this.Bullet.HitInterval;
			}
		}

		// Casterに追従する.
		if(this.IsCasterTrace)
		{
			if(this.Caster.GameObject)
			{
				var position = this.Caster.GameObject.transform.position;
				var rotation = this.Caster.GameObject.transform.rotation;
				GameGlobal.AddOffset(this.BulletSet, ref position, ref rotation, Vector3.one);
				this.transform.position = position;
				this.transform.rotation = rotation;
			}
			else
			{
				this.Destroy();
			}
		}
	}
	#endregion

	#region 当たり判定
	void OnCollisionEnter(Collision collision)	{ this.OnCollision(collision); }
	void OnCollisionStay(Collision collision)	{ this.OnCollision(collision); }
	void OnCollisionExit(Collision collision)	{ }
	void OnCollision(Collision collision)
	{
		try
		{
			ContactPoint contact = collision.contacts[0];
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
			Vector3 position = contact.point;
			this.CollisionProc(collision.gameObject, position, rotation);
		}
		catch(System.Exception e)
		{
			// UNDONE: OnCollisionExitから来た場合,Collision.contactのサイズが0の場合がある？.
			// そもそもTriggerを使っていない弾丸が何か不明なので調査.
			string str = this.gameObject.name + e.ToString();
			BugReportController.SaveLogFileWithOutStackTrace(str);
		}
	}
	void OnTriggerEnter(Collider collider)	{ this.OnTrigger(collider); }
	void OnTriggerStay(Collider collider)	{ this.OnTrigger(collider); }
	void OnTriggerExit(Collider collider)	{ }
	void OnTrigger(Collider collider)
	{
		Vector3 p = collider.ClosestPointOnBounds(this.transform.position);
		this.CollisionProc(collider.gameObject, p, this.transform.rotation);
	}
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		this.CollisionProc(hit.gameObject, hit.point, this.transform.rotation);
	}
	public abstract bool CollisionProc(GameObject hitObject, Vector3 position, Quaternion rotation);
	#endregion

	#region ヒットパケット送信
	protected bool SendHitPacket(GameObject hitObject, Vector3 position, Quaternion rotation, float bulletDirection)
	{
		// 現在ObjectBaseの直接の子クラスはCharacter,Gadget,ItemBaseの３つ.
		ObjectBase objectBase = ObjectCollider.GetCollidedObject(hitObject);
		if (objectBase == null)
		{
			// 地形に当たった.
			this.CreateHitEffect(position, rotation);
			return false;
		}

		// ダメージを食らうオブジェクト判定
		switch(objectBase.EntrantType)
		{
		case EntrantType.Pc:
			// 喰らい側がプレイヤーで無敵状態ではない.
			Player player = objectBase as Player;
            if (player != null) {
                if (!player.IsAbsoluteGuard) {
                    // パケット送信
                    BattlePacket.SendHit(this.Caster, this.SkillID, this.Bullet.ID, objectBase, position, bulletDirection);
                    return true;
                }
            } else {
                Person person = objectBase as Person;
                if (person != null) {
                    // パケット送信
                    if (person.EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                        BattlePacket.ProxySendHit(person.InFieldId, this.Caster, this.SkillID, this.Bullet.ID, objectBase, position, bulletDirection);
                    }
                    return true;
                }
            }

			return false;

		case EntrantType.Item:
		case EntrantType.Start:
		case EntrantType.Respawn:
		case EntrantType.FieldPortal:
		case EntrantType.RankingPortal:
			// 破壊不能オブジェクト.
			this.CreateHitEffect(position, rotation);
			return false;

		default:
			if (!objectBase.IsBreakable)
			{
				// 破壊不能オブジェクト.
				this.CreateHitEffect(position, rotation);
				return false;
			}
			// プレイヤーが撃った弾丸.
			if ((this.Caster is PlayerInfo))
			{
				// パケット送信
				BattlePacket.SendHit(this.Caster, this.SkillID, this.Bullet.ID, objectBase, position, bulletDirection);
				return true;
			}
			else
			{
				Gadget targetGadget = objectBase as Gadget;
				Gadget casterGadget = this.Caster.GameObject as Gadget;
				// 自分の召喚NPCが撃った弾丸.
				if(IsSendGadgetHitPacket(casterGadget, targetGadget))
				{
					// パケット送信
					BattlePacket.SendHit(this.Caster, this.SkillID, this.Bullet.ID, objectBase, position, bulletDirection);
					return true;
				} else {
                    // Test caster
                    if (this.Caster.NeedCalcInertia && GameController.IsRoomOwner()) {
                        // パケット送信
                        BattlePacket.SendHit(this.Caster, this.SkillID, this.Bullet.ID, objectBase, position, bulletDirection);
                        return true;
                    }
                }
			}
			return false;
		}
	}

	private bool IsSendGadgetHitPacket(Gadget casterGadget, Gadget targetGadget)
	{
		// Gadget同士ではない場合送信は行わない
		// (攻防どちらかにCharacterがいる場合,そちらで通信を行う)
		if(casterGadget == null || targetGadget == null)
		{
			return false;
		}

		// 防御側が自分の召喚NPCの場合.
		if(targetGadget.IsChildOfPlayer())
		{
			return true;
		}
		// 防御側が独立NPC且つ,攻撃側が自分の召喚NPCの場合.
		else if(targetGadget.IsIndependentNpc() && casterGadget.IsChildOfPlayer())
		{
			return true;
		}

		// 防御側が他人の召喚NPCの場合か,どちらも自分の召喚NPCではない場合
		return false;
	}

	/// <summary>
	/// ヒットエフェクト作成.Hit通信をしない破壊不可オブジェクト用.
	/// </summary>
	protected void CreateHitEffect(Vector3 position, Quaternion rotation)
	{
		// ヒットエフェクト
		EffectManager.CreateHit(position, rotation, this.Bullet);
		// ヒットSE
		SoundManager.CreateSeObject(position, rotation, this.Bullet.HitSeFile);
	}
	#endregion
}
