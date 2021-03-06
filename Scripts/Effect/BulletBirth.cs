/// <summary>
/// 発生タイプの弾丸
/// 
/// 2013/01/29
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletBirth : BulletBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 直近弾かどうか.
	/// </summary>
	public bool IsNearest { get; private set; }
	#endregion

	#region 生成＆破棄
	public static bool Setup(GameObject go, Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		// コンポーネント取得
		BulletBirth bullet = go.GetSafeComponent<BulletBirth>();
		if (bullet == null)
		{
			// 07/09 SkillDataがnullになるバグ調査用 → 解決.当たり判定を持たない演出エフェクトにスクリプトがセットされていたのが原因だった.再発時用にコードはそのまま.
			Debug.Log("SkillID = " + skillID);
			BugReportController.SaveLogFile("SkillID = " + skillID);
			
			manager.Destroy(go);
			return false;
		}
		bullet.SetSetupTime();
		
		bullet.SetupBirth(manager, target, targetPosition, caster, skillID, bulletSet);
#if UNITY_EDITOR || UNITY_STANDALONE || VIEWER
		GameGlobal.AddCubePolygon(go.transform, bullet.BoxCollider, true);
		GameGlobal.AddSpherePolygon(go.transform, bullet.SphereCollider, true);
#endif

		return true;
	}

	private void SetupBirth(Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		this.SetupBase(manager, target, targetPosition, caster, skillID, bulletSet);
		
		this.IsNearest = (this.Bullet.Type == SkillBulletMasterData.BulletType.Nearest);
	}
	#endregion

	#region BulletBase
	protected override void Update()
	{
		base.Update();
		
		// RigidbodyのSleep防止.
		this.transform.position = this.transform.position;

		if (0 < this.transform.childCount)
			{ return; }

		base.DestroyTimer();
	}
	public override bool CollisionProc(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		SetHitTime();
		
		// オブジェクトではない場合
		ObjectBase objectBase = ObjectCollider.GetCollidedObject(hitObject);
		if (objectBase == null)
		{
			return false;
		}
		if (this.Bullet == null)
		{
			this.BugLog("CollisionProc");
			return false;
		}
		if (this.IsNearest && (objectBase != this.Target))
		{
			return false;
		}
		// タイプ別でヒットさせる
		switch (objectBase.EntrantType)
		{
		case EntrantType.Pc:
		case EntrantType.Npc:
		case EntrantType.MiniNpc:
		case EntrantType.Mob:
		case EntrantType.MainTower:
		case EntrantType.SubTower:
		case EntrantType.Tank:
		case EntrantType.Wall:
		case EntrantType.Barrier:
		case EntrantType.Transporter:
        case EntrantType.Hostage:
			break;
		default:
			// 上記以外だとヒットしない！
			return false;
		}

		switch (this.Bullet.Attacktype)
		{
		case SkillBulletMasterData.AttackType.Enemy:
			// 味方はスルー
			if (this.CasterTeamType == objectBase.TeamType)
				{ return false; }
			// 無敵状態の敵PCには当たらない.
			if(objectBase.IsInvincible)
				{ return false; }
			break;
		case SkillBulletMasterData.AttackType.Friend:
			// 敵はスルー
			if (this.CasterTeamType != objectBase.TeamType)
				{ return false; }
			break;
		case SkillBulletMasterData.AttackType.All:
			break;
		}

		// 既にヒットしている
		if (this.PierceingList.Contains(objectBase.InFieldId))
		{
			return false;
		}
		// 初めて食らうのでヒット済みリストに加える
		this.PierceingList.Add(objectBase.InFieldId);

		if (this.Caster != null)
		{
			// ヒットパケットを送る
			Quaternion rot;
			if(this.IsNearest)
			{
				rot = this.transform.rotation;
			}
			else if(this.Caster.GameObject != null)
			{
				Vector3 direction = hitObject.transform.position - this.Caster.GameObject.transform.position;
				if (direction.sqrMagnitude > 0f)
					rot = Quaternion.LookRotation(direction);
				else
					rot = this.transform.rotation;
			}
			else
			{
				rot = this.transform.rotation;
			}
			this.SendHitPacket(hitObject, position, rotation, rot.eulerAngles.y);
			return true;
		}

		return false;
	}
	#endregion

	#region BulletBase
	protected override void DestroyCollision()
	{
		base.DestroyCollision();
	}
	#endregion

	
	
	// 07/09 SkillDataがnullになるバグ調査用 → 解決.当たり判定を持たない演出エフェクトにスクリプトがセットされていたのが原因だった.再発時用にコードはそのまま.
	float setupTime = 0;
	float hitTime = 0;
	private void SetSetupTime()
	{
		setupTime = Time.time;
		if(hitTime != 0)
		{
			BugLog("SetSetupTime");
		}
	}
	private void SetHitTime()
	{
		hitTime = Time.time;
		if(setupTime == 0)
		{
			BugLog("SetHitTime");
		}
	}
	private void BugLog(string type)
	{
		string msg = "BulletBirth : " + type +
					", Name = " + this.gameObject.name +
					", SkillID = " + this.SkillID +
					", SetUpTime = " + this.setupTime + 
					", HitTime = " + this.hitTime;
		Debug.Log(msg);
		BugReportController.SaveLogFileWithOutStackTrace(msg);
	}
}
