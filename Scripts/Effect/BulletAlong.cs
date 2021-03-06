/// <summary>
/// 壁沿い弾丸
/// 
/// 2014/02/04
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletAlong : BulletHomingBase
{
	#region フィールド＆プロパティ
	#endregion

	#region セットアップ
	public static bool Setup(GameObject go, Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		// コンポーネント取得
		BulletAlong bullet = go.GetSafeComponent<BulletAlong>();
		if (bullet == null)
		{
			manager.Destroy(go);
			return false;
		}
		bullet.SetupHoming(manager, target, targetPosition, caster, skillID, bulletSet);
		// アイリーンの超必殺技のボムが壁の中にめり込むことによって
		// 上空へ飛んでいき、距離判定で爆発する（ゼロ地点で爆発するのは不明）
		// LayerMask を空にすることによってUnity側の物理演算をストップし回避
		bullet.LayerMask = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || VIEWER
		GameGlobal.AddCubePolygon(go.transform, bullet.BoxCollider, false);
		GameGlobal.AddSpherePolygon(go.transform, bullet.SphereCollider, false);
#endif
		return true;
	}
	#endregion

	#region BulletHomingBase
	/// <summary>
	/// SphereCastによる当たり判定の処理.
	/// </summary>
	protected override bool SphereCastCollision()
	{
		bool isCollision = false;
		List<RaycastHit> hits;
		if(this.SphereCast(out hits))
		{
			foreach(var hit in hits)
			{
				if(this.IsAlongTarget(hit.transform.gameObject))
				{
					this.transform.position = hit.point + hit.normal * (this.Radius);
					break;	// 距離順なのでこれ以降は無視.
				}
				else
				{
					// 壁処理は上記で終えているのでbaseを呼ぶ.
					isCollision |= base.CollisionProc(hit.transform.gameObject, hit.point, this.transform.rotation);
				}
			}
		}
		
		return isCollision;
	}
	#endregion
	
	#region BulletBase
	/// <summary>
	/// OnCollision,OnTriggerによる当たり判定の処理.
	/// </summary>
	public override bool CollisionProc(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		if(this.IsAlongTarget(hitObject))
		{
			// 既に壁内部に入っている場合は動かない.
			this.transform.position = this.PrevPosition;
		}
		
		return base.CollisionProc(hitObject, position, rotation);
	}
	#endregion
	
	/// <summary>
	/// targetが壁沿い対象かどうかを判定する.
	/// </summary>
	bool IsAlongTarget(GameObject target)
	{
		ObjectBase objectBase = ObjectCollider.GetCollidedObject(target);
		if (objectBase)
		{
			switch (objectBase.EntrantType)
			{
				case EntrantType.MainTower:
				case EntrantType.SubTower:
				case EntrantType.Wall:
					return true;
				case EntrantType.Barrier:
					// 味方のバリアではない場合,壁扱い.
					if(this.CasterTeamType != objectBase.TeamType)
					{
						return true;
					}
					break;
			}
		}
		else
		{
			// 地形は壁扱い.
			return true;
		}
		
		return false;
	}
}
