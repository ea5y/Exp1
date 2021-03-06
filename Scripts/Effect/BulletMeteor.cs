/// <summary>
/// メテオ
/// 
/// 2013/04/16
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletMeteor : BulletHomingBase
{
	#region 静的メソッド
	public static bool Setup(GameObject go, Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		// コンポーネント取得
		BulletMeteor bullet = go.GetSafeComponent<BulletMeteor>();
		if (bullet == null)
		{
			manager.Destroy(go);
			return false;
		}
		bullet.SetupHoming(manager, target, targetPosition, caster, skillID, bulletSet);
#if UNITY_EDITOR || UNITY_STANDALONE
		GameGlobal.AddCubePolygon(go.transform, bullet.BoxCollider, false);
		GameGlobal.AddSpherePolygon(go.transform, bullet.SphereCollider, false);
#endif

		return true;
	}
	protected override void SetupLayer()
	{
		this.gameObject.layer = LayerNumber.IgnoreRaycast;
	}
	protected override void SetupLayerMask()
	{
		this.LayerMask = GameController.MeteorLayerMask;
	}
	#endregion

	#region BulletBase
	protected override void DestroyTimer()
	{
		this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);
		// ロストエフェクト
		EffectManager.CreateLost(this.transform.position, this.transform.rotation, this.Bullet);

		base.DestroyTimer();
	}
	#endregion

	#region BulletHomingBase
	protected override void Update()
	{
		base.Update();

		float judgeY = 0f;
		if (this.Target)
		{
			judgeY = this.TargetPosition.HasValue ? this.TargetPosition.Value.y :  this.Target.transform.position.y;
		}
		if (judgeY > this.transform.position.y)
		{
			// 目標地点とピッタリ合わせる
			this.transform.position = new Vector3(
				this.transform.position.x,
				judgeY,
				this.transform.position.z);
			this.DestroyDistance();
		}
	}
	protected override void DestroyDistance()
	{
		this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);
		// ロストエフェクト
		EffectManager.CreateLost(this.transform.position, this.transform.rotation, this.Bullet);

		base.DestroyDistance();
	}
	#endregion
}
