using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

/// <summary>
/// プレイヤースキルモーションパラメータクラス
/// </summary>
[System.Serializable]
public class PlayerSkillMotionParam : SkillMotionParam
{
	#region 宣言
	public class UseSkillParam
	{
		public GUISkillButton Button { get; private set; }
		public SkillMasterData Skill { get; private set; }
		public UseSkillParam(GUISkillButton button, SkillMasterData skill = null)
		{
			this.Button = button;
			this.Skill = skill;
		}
		public bool TryGetSkill(Character character)
		{
			int skillID = GameGlobal.GetSkillID(character.AvatarType, this.Button.SkillButtonType, character.Level);
			SkillMasterData skill;
			if (MasterData.TryGetSkill(skillID, out skill))
			{
				this.Skill = skill;
				return true;
			}

			Debug.LogWarning(string.Format("スキルがない({0}:ID={1})", character.UserName, skillID));
			return false;
		}
		public void SetLinkSkill(SkillMasterData skill)
		{
			this.Skill = skill;
		}
		public void SetTempCoolTime()
		{
			if(this.Button != null && this.Skill != null)
			{
				this.Button.CoolTime = this.Skill.CoolTime;
				this.Button.IsCounter = false;
			}
		}
		public void SetCoolTime(float rate)
		{
			if(this.Button != null && this.Skill != null)
			{
				this.Button.CoolTime = this.Skill.CoolTime * rate;
                Debug.Log("===> SetCoolTime " + this.Button.CoolTime);
			}
		}
	}
	#endregion

	#region リンク・キャンセルスキル情報
	private readonly Player player;
	protected override Character Character { get { return player; } }

	const float CancelCoolTimeRate = 0.1f;

	public bool IsCancelAccept { get; set; }

	public UseSkillParam UsingSkill { get; private set; }
	public UseSkillParam CancelSkill { get; private set; }
	public int LinkSkillID { get; private set; }
	public int ForceLinkSkillID { get; private set; }

	public bool IsLink { get { return LinkSkillID != GameConstant.InvalidID; } }
	public bool IsCancel { get { return CancelSkill != null; } }
	public bool IsForceLink { get { return ForceLinkSkillID != GameConstant.InvalidID; } }

	private List<UseSkillParam> UsedSkillList { get; set; }

	/// <summary>
	/// 次のスキルに強制リンクする状態か？.
	/// </summary>
	public bool CanForceLink
	{
		get {
			// 現スキルが強制リンクスキルで,リンク数が規定回数以内か？.
			return (this.Skill.ForceLinkFlag && 
					this.Step < this.MaxLinkCount);
		}
	}
	
	#region リンク中継続する情報
	public int Step { get; set; }
	public int MaxLinkCount { get; set; }
	public SkillMasterData FirstSkill { get; protected set; }
	#endregion

	protected Fiber CancelFiber { get; set; }
	protected List<IEnumerator> BulletFiberList { get; set; }
	protected IEnumerator FixedRotationFiber { get; set; }
	protected FiberSet MoveFiberSet { get; set; }
	protected FiberSet GravityFiberSet { get; set; }
	protected IEnumerator DelayFiber { get; set; }
	protected FiberSet BackMoveFiberSet { get; private set; }

	#endregion
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public PlayerSkillMotionParam(Player player)
	{
		this.player = player;

		// ファイバーの初期化
		this.MoveFiberSet = new FiberSet();
		this.GravityFiberSet = new FiberSet();
		this.UsedSkillList = new List<UseSkillParam>();
		this.BackMoveFiberSet = new FiberSet();

		this.InitLink();
	}
	/// <summary>
	/// 生使用時の初期化
	/// </summary>
	protected void InitFirst()
	{
		this.InitCancel();

		// 連携終了まで継続するパラメータはここで初期化.
		this.SetCoolTime();
		this.BackMoveFiberSet.Clear();
		this.FirstSkill = this.Skill;
	}
	/// <summary>
	/// キャンセル時の初期化
	/// </summary>
	protected void InitCancel()
	{
		this.InitLink();

		// 別のボタンでキャンセルした場合はここで初期化.
		this.Step = 0;
		this.SelfBulletList.Clear();
		this.MaxLinkCount = this.Skill.MaxLinkCount;
	}
	/// <summary>
	/// リンク時の初期化
	/// </summary>
	protected void InitLink()
	{
		this.IsCancelAccept = false;
		this.LinkSkillID = GameConstant.InvalidID;
		this.CancelSkill = null;
		this.ForceLinkSkillID = GameConstant.InvalidID;
	}

	/// <summary>
	/// スキルを開始する【非キャンセル】.
	/// 戻り値がfalseの場合はSkillMasterData,SkillMotionMasterDataの取得ミス.
	/// </summary>
	public bool SetFirstSkill(GUISkillButton button, ObjectBase target, bool isTracingTarget, Vector3? targetPosition)
	{
		var newSkill = new UseSkillParam(button);
		if(!newSkill.TryGetSkill(this.player))
			{ return false; }
		// スキルとモーションの取得.
		if (!SetSkillData(newSkill.Skill))
			{ return false; }

		this.InitFirst();

		this.UsingSkill = newSkill;
        Debug.Log("===> new skill" + newSkill);
		this.SetupSkillMotionParam(target, false, isTracingTarget, targetPosition);
        Debug.Log("===> new skill" + newSkill);
		return true;
	}
	public bool SetCancelSkill(UseSkillParam skill, ObjectBase target, bool isInterrupt)
	{
		// スキルとモーションの取得.
		if (!SetSkillData(skill.Skill))
			{ return false; }

		// クールタイムの仮設定と予約.
		if(this.UsingSkill != null)
		{
			this.UsingSkill.SetTempCoolTime();
			this.UsedSkillList.Add(this.UsingSkill);
		}

		this.InitCancel();

		this.UsingSkill = skill;

		this.SetupSkillMotionParam(target, isInterrupt, true, this.TargetPosition);

		return true;
	}
	public bool SetLinkSkill(int skillID, ObjectBase target, bool isInterrupt)
	{
		// スキルとモーションの取得.
		if (!SetSkillData(skillID))
			{ return false; }

		this.InitLink();

		this.UsingSkill.SetLinkSkill(this.Skill);

		this.SetupSkillMotionParam(target, isInterrupt, true, this.TargetPosition);

		return true;
	}
	/// <summary>
	/// スキルモーションとパラメータのセットアップ
	/// </summary>
	protected void SetupSkillMotionParam(ObjectBase target, bool isInterrupt, bool isTracingTarget, Vector3? targetPosition)
	{
		// パラメータのセットアップ
		this.SetupParams(target, isInterrupt, isTracingTarget, targetPosition);

		// ファイバーのセットアップ
		SetupFibers();

		IsSetup = true;

		// 強制リンクスキルをセット.
		if(this.Skill.ForceLinkFlag)
		{
			this.SetForceLinkParam();
		}
	}
	/// <summary>
	/// キャンセル、またはリンク入力
	/// </summary>
	public bool SetCancelOrLink(GUISkillButton skillButton)
	{
		// キャンセルを受け付けているかどうか
		if(!this.CanForceLink && this.IsCancelAccept)
		{
			// キャンセル,またはリンクスキルとして設定.
			if (this.UsingSkill != null && this.UsingSkill.Button != null)
			{
				if(this.UsingSkill.Button == skillButton)
				{
					// 現在使用中のスキルと同じボタンならリンク.
					return this.SetLinkParam();
				}
				else
				{
					// 超必殺技からのキャンセルは受け付けない.
					if (this.UsingSkill.Button.SkillButtonType != SkillButtonType.SpecialSkill)
					{
						return this.SetCancelParam(skillButton);
					}
				}
			}
			else
			{
				// スキルかボタンが不明.復帰するためにキャンセル扱い.
				BugReportController.SaveLogFile((this.UsingSkill == null ? "UsingSkill" : "UsingSkill.Button") + " null");
				return this.SetCancelParam(skillButton);
			}
		}
        return false;
	}
	private bool SetLinkParam()
	{
		// 既に先行入力済み
		if (this.IsLink)
			return false;
		// 最大リンク回数を超えている
		if (this.Step >= this.MaxLinkCount)
			return false;

		// リンクパラメーター設定
		this.LinkSkillID = this.Skill.LinkSkillID;
		this.Step++;
        return true;
	}
	private bool SetCancelParam(GUISkillButton newSkillButton)
	{
		// 既に先行入力済み
		if (this.IsCancel)
			return false;

		// キャンセルパラメーター設定
		this.CancelSkill = new UseSkillParam(newSkillButton, null);
        Debug.LogError("===> CancelSkill");
		return true;
	}
	private void SetForceLinkParam()
	{
		// 既に先行入力済み
		if (this.IsForceLink)
			return;
		// 最大リンク回数を超えている
		if (this.Step >= this.MaxLinkCount)
			return;
		
		// リンクパラメーター設定
		this.ForceLinkSkillID = this.Skill.LinkSkillID;
		this.Step++;
	}
	public void SetCoolTime()
	{
		float rate = 1f + (this.UsedSkillList.Count * PlayerSkillMotionParam.CancelCoolTimeRate);

		// クールタイム設定
		if(this.UsingSkill != null)
		{
			this.UsedSkillList.Add(this.UsingSkill);
			this.UsingSkill = null;
		}
		foreach (var param in this.UsedSkillList)
		{
			param.SetCoolTime(rate);
		}
		this.UsedSkillList.Clear();
	}

	/// <summary>
	/// ファイバーのセットアップ
	/// </summary>
	protected override void SetupFibers()
	{
		base.SetupFibers();

		// 弾丸コルーチン
		this.BulletFiberList = this.player.SetBulletFiber(this.Skill, this.Target, this.TargetPosition);
		// キャンセルタイミングコルーチン
	    this.CancelFiber = this.player.SetCancelFiber(this.Skill.CancelAcceptTime, this.Skill.CancelPointTime, this.Skill.CancelEndTime, this.Target);
		// スーパーアーマーコルーチン
		this.SuperArmorFiberList = this.player.SetSuperArmorFiber(this.Skill);
		// 向き固定コルーチン
        //TODO Lee Change 释放技能阻碍转向
	    this.FixedRotationFiber = null;
//		this.FixedRotationFiber = this.player.SetFixedRotationFiber(this.Skill.FixedRotationTiming);
		// ディレイコルーチン
		this.DelayFiber = this.player.SetDelayFiber(this.Skill.DelayTime);
		// 移動コルーチン
		this.MoveFiberSet = this.player.SetMoveFiber(this.Skill);
		// 重力コルーチン
		this.GravityFiberSet = this.player.SetGravityFiber(this.Skill);
	}
	/// <summary>
	/// 反動コルーチンの追加.
	/// </summary>
	public void AddBackMoveFiber(SkillBulletMasterData bullet)
	{
		if (bullet.BackTime > 0f)
		{
			this.BackMoveFiberSet.AddFiber(this.player.BackMoveCoroutine(bullet));
		}
	}

	/// <summary>
	/// SkillMotionProg.Motion の際に実行するファイバー処理.
	/// </summary>
	public override void UpdateMotionFiber()
	{
		base.UpdateMotionFiber();

		// 弾丸コルーチンの実行＆監視
		if (this.BulletFiberList != null)
		{
			foreach (var fiber in this.BulletFiberList)
			{
				fiber.MoveNext();
			}
		}
		// 向き固定コルーチンの実行＆監視
		if (this.FixedRotationFiber != null)
		{
			this.FixedRotationFiber.MoveNext();
		}
		// 移動コルーチンの実行
		this.MoveFiberSet.Update();
		// 重力コルーチンの実行
		this.GravityFiberSet.Update();
		// キャンセルコルーチンの実行＆監視
		// キャンセルコルーチンの順番死守！
		// コルーチン系で一番最後に処理しないと
		// キャンセルが発生した時にプロセスが初期化されているのに
		// 中途半端にコルーチンが回ってしまうため
		if (this.CancelFiber != null)
		{
			this.CancelFiber.Update();
		}
		// 反動コルーチンの実行
		this.BackMoveFiberSet.Update();
	}
	public override bool IsDelay()
	{
		return this.DelayFiber != null;
	}
	/// <summary>
	/// SkillMotionProg.Delay の際に実行するファイバー処理.
	/// </summary>
	public override void UpdateDelayFiber()
	{
		// ディレイコルーチンの実行＆監視
		{
			IEnumerator fiber = this.DelayFiber;
			if (fiber != null)
			{
				fiber.MoveNext();
			}
		}
		// 反動コルーチンの実行
		this.BackMoveFiberSet.Update();
	}
}
