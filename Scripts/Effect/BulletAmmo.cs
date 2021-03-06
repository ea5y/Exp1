/// <summary>
/// 弾丸エフェクト
/// 
/// 2012/12/17
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletAmmo : BulletHomingBase
{
	#region 静的メソッド
	public static bool Setup(GameObject go, Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		// コンポーネント取得
		BulletAmmo bullet = go.GetSafeComponent<BulletAmmo>();
		if (bullet == null)
		{
			manager.Destroy(go);
			return false;
		}
		bullet.SetupHoming(manager, target, targetPosition, caster, skillID, bulletSet);
#if UNITY_EDITOR || UNITY_STANDALONE || VIEWER
		GameGlobal.AddCubePolygon(go.transform, bullet.BoxCollider, false);
		GameGlobal.AddSpherePolygon(go.transform, bullet.SphereCollider, false);
#endif

		return true;
	}
	#endregion
}
