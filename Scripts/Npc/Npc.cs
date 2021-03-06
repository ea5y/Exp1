/// <summary>
/// NPCキャラクター
/// 
/// 2013/00/00
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class Npc : NpcBase, Turret.ITurretCarrier
{
	#region フィールド＆プロパティ

	// 経験値処理
	protected override Vector3 ExpEffectOffsetMin{ get{ return new Vector3(0.0f, 1.0f, 0.0f); } }
	protected override Vector3 ExpEffectOffsetMax{ get{ return new Vector3(0.0f, 1.0f, 0.0f); } }

	const float MovePacketInterval      = 0.1f;
	const float MoveAnimationMinSpeed   = MovePacketInterval * 0.2f;
	const float RotateAnimationMinAngle = MovePacketInterval * 2.0f;

	public Vector3 Velocity { get; protected set; }
	public float AngularVelocity { get; protected set; }

	private NpcAnimation NpcAnimation{ get{ return this.Animation as NpcAnimation; } }
	
	private bool faceOpen = false;
	
	// 砲台オブジェクト
	public Turret Turret { get; set; }
	#endregion
	
	#region セットアップ
	public static void Setup(GameObject go, NpcManager manager, ObjectMasterData objectData, NpcInfo info, AssetReference assetReference)
	{
        go.layer = LayerNumber.vsPlayer_Bullet;
		go.name += " : " + Time.time;
		// コンポーネント取得
		Npc npc = go.GetSafeComponent<Npc>();
		if (npc == null)
		{
			manager.Destroy(go);
			return;
		}
		// 初期設定
		npc.Setup(manager, objectData, info, assetReference);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		GameGlobal.AddCubePolygon(go.transform, go.GetComponentInChildren<BoxCollider>(), true);
		GameGlobal.AddSpherePolygon(go.transform, go.GetComponentInChildren<SphereCollider>(), true);
#endif
		npc.SetupCompleted();
	}
	#endregion

	#region 初期化
	void Start()
	{
		this.Velocity = this.transform.forward;
	}
	#endregion
	
	#region 更新
	protected override void Update()
	{
		UpdateMove();
		base.Update();
	}
	void UpdateMove()
	{
		Vector3 moveValue = this.Velocity * Time.deltaTime;
		if(Vector3.SqrMagnitude(this.NextPosition - this.transform.position) <= Vector3.SqrMagnitude(moveValue))
		{
			this.transform.position = this.NextPosition;
		}
		else
		{
			this.transform.position += moveValue;
		}
		
		float rotateValue = this.AngularVelocity * Time.deltaTime;
		if(Mathf.Abs(this.NextRotation.eulerAngles.y - this.transform.rotation.eulerAngles.y) <= Mathf.Abs(rotateValue))
		{
			this.transform.rotation = this.NextRotation;
		}
		else
		{
			// 投げの最中の値を取得してしまうとおかしな向きになってしまうことがあるため.
			// XZに関してはthis.transform.rotationではなくthis.NextRotationを使う.
			this.transform.rotation = Quaternion.Euler(
				new Vector3(this.NextRotation.eulerAngles.x,
							this.transform.rotation.eulerAngles.y + rotateValue,
							this.NextRotation.eulerAngles.z));
		}
	}
	#endregion

	#region ObjectBase Override
	#region 移動パケット
	public override void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
	{
		base.MoveBase(position, rotation);
		
		// モーション処理.
		NpcAnimation npcAnimation = this.NpcAnimation;
		if (npcAnimation)
		{
			// 移動モーションを再生するか？
			if(MoveAnimationMinSpeed * MoveAnimationMinSpeed < Vector3.SqrMagnitude(this.NextPosition - position))
			{
				npcAnimation.AnimationFade(NpcAnimationParam.MotionState.run);
			}
			else
			{
				// 旋回モーションを再生するか？
				if(RotateAnimationMinAngle < Mathf.Abs(this.NextRotation.eulerAngles.y - rotation.eulerAngles.y))
				{
					npcAnimation.AnimationFade(NpcAnimationParam.MotionState.rotate);
				}
				else
				{
					npcAnimation.AnimationFade(NpcAnimationParam.MotionState.wait);
				}
			}
		}
		
		this.NextPosition = position;
		this.Velocity = (this.NextPosition - this.transform.position) / MovePacketInterval;
		
		if(Turret != null)
		{
			Turret.SetRotation(rotation, MovePacketInterval);
		}
		else
		{
			this.NextRotation = rotation;
			this.AngularVelocity = ((this.NextRotation.eulerAngles.y - this.transform.rotation.eulerAngles.y + 360f) % 360f);
			this.AngularVelocity = 180 < this.AngularVelocity ? this.AngularVelocity - 360 : this.AngularVelocity;
			this.AngularVelocity /= MovePacketInterval;
		}
	}
	#endregion
	
	#region スキルモーション処理
	protected override void PlaySkillMotion(SkillMasterData skill)
	{
		if (skill.Motion != null)
		{
			NpcAnimation npcAnimation = this.NpcAnimation;
			if(npcAnimation != null)
			{
				npcAnimation.AnimationFade(NpcAnimationParam.MotionState.wait);
				npcAnimation.AnimationCut(skill.Motion.File);
			}
		}
	}
	#endregion
	
	#region ヒット処理
	const float vibrateDistanceSqr = 576f;
	public override void Hit(HitInfo hitInfo)
	{
		this.HitBase(hitInfo);

		NpcAnimation npcAnimation = this.NpcAnimation;
		if(npcAnimation)
		{
			if(faceOpen)
			{
			}
			else
			{
				if(hitInfo.hitPoint < MaxHitPoint * 0.3f)
				{
					faceOpen = true;
					npcAnimation.AnimationCut(NpcAnimationParam.MotionState.skill4);
				}
			}
		}
	}
	#endregion

	#region 破壊処理
	protected override void Destroy()
	{
		// あらかじめ非破壊でRemoveEntrantを掛けておくことでEntrant管理とGameObjectの破壊を無関係にする.
		Entrant.RemoveEntrant(this.EntrantInfo, false);

		base.CreateBrokenEffect();
		StartCoroutine(DeadCoroutine());	// base.Destroy();はコルーチン終了時に呼ぶ.
	}
	IEnumerator DeadCoroutine()
	{
		NpcAnimation npcAnimation = this.NpcAnimation;
		if(npcAnimation)
		{
			npcAnimation.AnimationFade(NpcAnimationParam.MotionState.dead);
			yield return new WaitForSeconds(npcAnimation.GetAnimationLength(NpcAnimationParam.MotionState.dead));
		}
		base.Destroy();
	}
	protected override void CreateBrokenEffect()
	{
		switch(EntrantType)
		{
		case EntrantType.Npc:
			Player player = GameController.GetPlayer();
			if(player != null)
			{
				if((player.transform.position - this.transform.position).sqrMagnitude < vibrateDistanceSqr)
				{
					// カメラを揺らす
					CharacterCamera cc = GameController.CharacterCamera;
					if (cc)
					{
						cc.Shake();
					}
				}

				if(player.TeamType == this.TeamType)
				{
					// 撃破演出
					GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.GuardianDst);
				}
				else
				{
					// 被撃破演出
					GUIEffectMessage.SetTacticalInfo(GUITacticalMessageItem.TacticalType.GuardianDst);
				}
			}
			break;
		}
	}
	#endregion

	#endregion
}
