using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

/// <summary>
/// スキルモーションパラメータ ベースクラス
/// </summary>
public abstract class SkillMotionParam
{
	public enum SkillMotionProg
	{
		Init,
		Marker,
		Motion,
		Delay,
		End,
	}

	#region 現在のスキル情報
	protected abstract Character Character { get; }

	public bool IsSetup { get; protected set; }
	
	public SkillMotionProg Progress { get; set; }

	public ObjectBase Target { get; set; }
    // If IsTracingTarget is false, do not use the target's position
    public bool IsTracingTarget { get; set; }
    // If this has value, assume this is the target's actual position
    public Vector3? TargetPosition { get; set; }
	public SkillMasterData Skill { get; protected set; }
	public SkillMotionMasterData Motion { get; protected set; }

	public bool IsChargeSkill { get; set; }
	public bool IsAimingSkill { get; protected set; }
	public bool IsSelfBullet { get; protected set; }
	public bool IsInterrupt { get; protected set; }

	protected IEnumerator MotionFiber { get; set; }
	protected List<IEnumerator> MoveRatioFiberList { get; set; }
	protected List<IEnumerator> SuperArmorFiberList { get; set; }
	protected List<IEnumerator> EffectFiberList { get; set; }

	#region リンク中継続する情報
	public List<BulletSelf> SelfBulletList { get; protected set; }
	#endregion

	#endregion

	/// <summary>
	/// コンストラクタ
	/// </summary>
	protected SkillMotionParam()
	{
		this.SelfBulletList = new List<BulletSelf>();

		this.InitSkillMotion();
	}
	/// <summary>
	/// スキルとモーションの初期化
	/// </summary>
	private void InitSkillMotion()
	{
		this.IsSetup = false;

		this.Target = null;
		this.IsInterrupt = false;
		this.Skill = null;
		this.Motion = null;

		this.IsChargeSkill = false;
		this.IsAimingSkill = false;
		this.IsSelfBullet = false;

		this.Progress = SkillMotionProg.Init;

		// ファイバーの初期化
		this.MotionFiber = null;
		this.MoveRatioFiberList = null;
		this.SuperArmorFiberList = null;
		this.EffectFiberList = null;
	}

	/// <summary>
	/// スキルとモーションの取得.
	/// </summary>
	protected bool SetSkillData(int skillID)
	{
		SkillMasterData skill = null;

		// スキルが有るかどうか
		if (!MasterData.TryGetSkill(skillID, out skill))
		{
			Debug.LogWarning(string.Format("スキルがない({0}:ID={1})", this.Character.UserName, skillID));
			return false;
		}
		return this.SetSkillData(skill);
	}
	protected bool SetSkillData(SkillMasterData skill)
	{
		SkillMotionMasterData motion = null;
		// 有効なクリップ名かどうか
		motion = skill.Motion;
		if (motion == null)
		{
			Debug.LogWarning(string.Format("モーションが設定されていない({0}:ID={1})", this.Character.UserName, skill.ID));
			return false;
		}
		if (string.IsNullOrEmpty(motion.File))
		{
			Debug.LogWarning(string.Format("モーションファイル名が不正({0}:File={1})", this.Character.UserName, motion.File));
			return false;
		}

		// もろもろ成功した後に初期化（失敗時には初期化しない）
		this.InitSkillMotion();

		this.Skill = skill;
		this.Motion = motion;

		return true;
	}

	/// <summary>
	/// スキルパラメータのセットアップ
	/// </summary>
	protected void SetupParams(ObjectBase target, bool isInterrupt, bool isTracingTarget, Vector3? targetPosition)
	{
		this.Target = target;
		this.IsInterrupt = isInterrupt;
        this.IsTracingTarget = isTracingTarget;
        this.TargetPosition = targetPosition;

		// チャージスキルかどうか
		this.IsChargeSkill = false;
		if (this.Skill.ChargeSkillId != GameConstant.InvalidID)
			this.IsChargeSkill = true;

		// エイミングスキルかどうか
		this.IsAimingSkill = false;
		if (this.Skill.BulletSetList != null)
		{
			foreach (var bulletSet in this.Skill.BulletSetList)
			{
				SkillBulletMasterData bullet = bulletSet.Bullet;
				if (bullet == null)
					continue;
				// マーカー設定があるかどうか
				SkillAimingMarkerMasterData markerData;
				if (!MasterData.TryGetAimingMarker(bullet.ID, out markerData))
					continue;
				this.IsAimingSkill = true;
				break;
			}
		}
		
		// 自分弾丸かどうか
		this.IsSelfBullet = false;
		if (this.Skill.BulletSetList != null)
		{
			foreach (var bulletSet in this.Skill.BulletSetList)
			{
				switch (bulletSet.Bullet.Type)
				{
					case SkillBulletMasterData.BulletType.SelfNormal:
					case SkillBulletMasterData.BulletType.SelfPierce:
						this.IsSelfBullet = true;
						break;
				}
				if (this.IsSelfBullet)
					break;
			}
		}
	}
	/// <summary>
	/// ファイバーのセットアップ
	/// </summary>
	protected virtual void SetupFibers()
	{
		// 移動速度倍率コルーチン.(移動時歩きモーション合成設定込み)
		this.MoveRatioFiberList = this.Character.SetMoveRatioFiber(this.Motion);
		// モーションコルーチン
		this.MotionFiber = this.Character.SetMotionFiber(this.Skill, this.Motion, this.Target);
		
		// エフェクトコルーチン
		this.EffectFiberList = this.Character.SetEffectFiber(this.Skill);
	}

	/// <summary>
	/// SkillMotionProg.Motion の際に実行するファイバー処理.
	/// </summary>
	public virtual void UpdateMotionFiber()
	{
		// モーションコルーチンの実行＆監視
		{
			if (this.MotionFiber != null)
			{
				this.MotionFiber.MoveNext();
			}
		}
		// 移動倍率コルーチンの実行＆監視
		if (this.MoveRatioFiberList != null)
		{
			foreach (var fiber in this.MoveRatioFiberList)
			{
				fiber.MoveNext();
			}
		}
		// スーパーアーマーコルーチンの実行＆監視
		if (this.SuperArmorFiberList != null)
		{
			foreach (var fiber in this.SuperArmorFiberList)
			{
				fiber.MoveNext();
			}
		}
		// エフェクトコルーチンの実行＆監視
		if (this.EffectFiberList != null)
		{
			foreach (var fiber in this.EffectFiberList)
			{
				fiber.MoveNext();
			}
		}
	}
	public abstract bool IsDelay();
	/// <summary>
	/// SkillMotionProg.Delay の際に実行するファイバー処理.
	/// </summary>
	public abstract void UpdateDelayFiber();
}
