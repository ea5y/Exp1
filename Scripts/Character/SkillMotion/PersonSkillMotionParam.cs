using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

/// <summary>
/// スキルモーションパラメータ
/// </summary>
public class PersonSkillMotionParam : SkillMotionParam
{
    #region 宣言
    public class UseSkillParam {
        public GUISkillButton Button { get; private set; }
        public SkillMasterData Skill { get; private set; }
        public UseSkillParam(GUISkillButton button, SkillMasterData skill = null) {
            this.Button = button;
            this.Skill = skill;
        }
        public bool TryGetSkill(Character character) {
            int skillID = GameGlobal.GetSkillID(character.AvatarType, this.Button.SkillButtonType, character.Level);
            SkillMasterData skill;
            if (MasterData.TryGetSkill(skillID, out skill)) {
                this.Skill = skill;
                return true;
            }

            Debug.LogWarning(string.Format("スキルがない({0}:ID={1})", character.UserName, skillID));
            return false;
        }
        public void SetLinkSkill(SkillMasterData skill) {
            this.Skill = skill;
        }
        public void SetTempCoolTime() {
            if (this.Button != null && this.Skill != null) {
                this.Button.CoolTime = this.Skill.CoolTime;
                this.Button.IsCounter = false;
            }
        }
        public void SetCoolTime(float rate) {
            if (this.Button != null && this.Skill != null) {
                this.Button.CoolTime = this.Skill.CoolTime * rate;
                Debug.Log("===> SetCoolTime " + this.Button.CoolTime);
            }
        }
    }
    #endregion

    private readonly Person person;
	protected override Character Character { get { return person; } }

    protected Fiber CancelFiber { get; set; }
    protected List<IEnumerator> BulletFiberList { get; set; }
    protected IEnumerator FixedRotationFiber { get; set; }
    protected FiberSet MoveFiberSet { get; set; }
    protected FiberSet GravityFiberSet { get; set; }
    protected IEnumerator DelayFiber { get; set; }
    protected FiberSet BackMoveFiberSet { get; private set; }

    public bool IsCancelAccept { get; set; }
    private List<UseSkillParam> UsedSkillList { get; set; }
    public UseSkillParam UsingSkill { get; private set; }
    public UseSkillParam CancelSkill { get; private set; }
    const float CancelCoolTimeRate = 0.1f;

    public int LinkSkillID { get; private set; }
    public int ForceLinkSkillID { get; private set; }

    public bool IsLink { get { return LinkSkillID != GameConstant.InvalidID; } }
    public bool IsCancel { get { return CancelSkill != null; } }
    public bool IsForceLink { get { return ForceLinkSkillID != GameConstant.InvalidID; } }

    #region リンク中継続する情報
    public int Step { get; set; }
    public int MaxLinkCount { get; set; }
    public SkillMasterData FirstSkill { get; protected set; }
    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PersonSkillMotionParam(Person person)
	{
		this.person = person;

        // ファイバーの初期化
        this.MoveFiberSet = new FiberSet();
        this.GravityFiberSet = new FiberSet();
        this.UsedSkillList = new List<UseSkillParam>();
        this.BackMoveFiberSet = new FiberSet();

        this.InitLink();
    }
	public bool StartSkill(int skillID, ObjectBase target)
	{
		// スキルとモーションの取得.
		if (!SetSkillData(skillID))
			{ return false; }

		// リンク中継続するパラメータはここで初期化
		this.SelfBulletList.Clear();

		this.SetupSkillMotionParam(target, false);

		return true;
	}
    public bool SetCancelSkill(UseSkillParam skill, ObjectBase target, bool isInterrupt) {
        // スキルとモーションの取得.
        if (!SetSkillData(skill.Skill)) { return false; }

        // クールタイムの仮設定と予約.
        if (this.UsingSkill != null) {
            this.UsingSkill.SetTempCoolTime();
            this.UsedSkillList.Add(this.UsingSkill);
        }

        this.InitCancel();

        this.UsingSkill = skill;

        this.SetupSkillMotionParam(target, isInterrupt);

        return true;
    }
    /// <summary>
	/// キャンセル時の初期化
	/// </summary>
	protected void InitCancel() {
        this.InitLink();

        // 別のボタンでキャンセルした場合はここで初期化.
        this.Step = 0;
        this.SelfBulletList.Clear();
        this.MaxLinkCount = this.Skill.MaxLinkCount;
    }
    /// <summary>
	/// リンク時の初期化
	/// </summary>
	protected void InitLink() {
        this.IsCancelAccept = false;
        this.LinkSkillID = GameConstant.InvalidID;
        this.CancelSkill = null;
        this.ForceLinkSkillID = GameConstant.InvalidID;
    }
    public bool SetLinkSkill(int skillID, ObjectBase target, bool isInterrupt) {
        // スキルとモーションの取得.
        if (!SetSkillData(skillID)) { return false; }

        this.InitLink();

        this.UsingSkill.SetLinkSkill(this.Skill);

        this.SetupSkillMotionParam(target, isInterrupt);

        return true;
    }
    /// <summary>
    /// スキルモーションとパラメータのセットアップ
    /// </summary>
    protected void SetupSkillMotionParam(ObjectBase target, bool isInterrupt)
	{
		// パラメータのセットアップ
		SetupParams(target, isInterrupt, false, this.TargetPosition);

		// ファイバーのセットアップ
		SetupFibers();

		IsSetup = true;
	}

    public override void UpdateMotionFiber() {
        base.UpdateMotionFiber();

        // 弾丸コルーチンの実行＆監視
        if (this.BulletFiberList != null) {
            foreach (var fiber in this.BulletFiberList) {
                fiber.MoveNext();
            }
        }
        // 向き固定コルーチンの実行＆監視
        if (this.FixedRotationFiber != null) {
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
        if (this.CancelFiber != null) {
            this.CancelFiber.Update();
        }
        // 反動コルーチンの実行
        this.BackMoveFiberSet.Update();
    }

    protected override void SetupFibers() {
        base.SetupFibers();

        if (person != null && person.EntrantInfo.NeedCalcInertia) {
            // 弾丸コルーチン
            this.BulletFiberList = this.person.SetBulletFiber(this.Skill, this.Target);
            // キャンセルタイミングコルーチン
            this.CancelFiber = this.person.SetCancelFiber(this.Skill.CancelAcceptTime, this.Skill.CancelPointTime, this.Skill.CancelEndTime, this.Target);
            // スーパーアーマーコルーチン
            this.SuperArmorFiberList = this.person.SetSuperArmorFiber(this.Skill);
            // 向き固定コルーチン
            //TODO Lee Change 释放技能阻碍转向
            this.FixedRotationFiber = null;
            //		this.FixedRotationFiber = this.player.SetFixedRotationFiber(this.Skill.FixedRotationTiming);
            // ディレイコルーチン
            this.DelayFiber = this.person.SetDelayFiber(this.Skill.DelayTime);
            // 移動コルーチン
            this.MoveFiberSet = this.person.SetMoveFiber(this.Skill);
            // 重力コルーチン
            this.GravityFiberSet = this.person.SetGravityFiber(this.Skill);
        }
        
    }

    /// <summary>
    /// 反動コルーチンの追加.
    /// </summary>
    public void AddBackMoveFiber(SkillBulletMasterData bullet) {
        if (bullet.BackTime > 0f) {
            this.BackMoveFiberSet.AddFiber(this.person.BackMoveCoroutine(bullet));
        }
    }

    public void SetCoolTime() {
        float rate = 1f + (this.UsedSkillList.Count * PersonSkillMotionParam.CancelCoolTimeRate);

        // クールタイム設定
        if (this.UsingSkill != null) {
            this.UsedSkillList.Add(this.UsingSkill);
            this.UsingSkill = null;
        }
        foreach (var param in this.UsedSkillList) {
            param.SetCoolTime(rate);
        }
        this.UsedSkillList.Clear();
    }

    public override bool IsDelay() { return false; }
	public override void UpdateDelayFiber() { }
}
