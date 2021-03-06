/// <summary>
/// 他プレイヤー
/// 
/// 2012/12/04
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using System.Collections.Generic;

public class Person : Character
{
    public const int PERSON_LAYER = LayerNumber.vsPlayer_Bullet;

    #region PlayerStateAdapter
    /// <summary>
    /// PersonStateからPersonへアクセスするためのアダプタ.
    /// </summary>
    public class PersonStateAdapter
	{
        readonly Person person;
		public Person Person { get { return person; } }
		
		public PersonStateAdapter(Person person)
		{
			this.person = person;
		}
		public bool HasGrappleAttach
		{
			get { return this.person.grappleAttach != null; }
		}
		public void ResetAnimation()
		{
			this.person.ResetAnimation();
		}

		// 応急処置.
		public void MoveProc()
		{
			this.person.MoveProc();
		}
		public void SkillMotionProc()
		{
			this.person.SkillMotionProc();
		}
		public void SkillMotionFinish()
		{
			this.person.SkillMotionFinish();
		}

        public void ForceSendMovePacket() {
            this.person.ForceSendMovePacket();
        }
        public void SendMovePacket() {
            this.person.SendMovePacket();
        }
        public void SendMotion(MotionState motionstate) {
            person.SendMotion(motionstate);
        }
        
        public IEnumerator BotOrderAnimCoroutine(MotionState staMotion, MotionState endMotion) {
            return person.BotOrderAnimCoroutine(staMotion, endMotion);
        }
        public void CalculateMove(Vector3 moveVector, float deltaTime, out Vector3 movement) {
            person.CharacterMove.CalculateMove(moveVector, deltaTime, out movement);
        }
        public void CalculateMove(Vector3 moveVector, out Vector3 movement) {
            person.CharacterMove.CalculateMove(moveVector, Time.deltaTime, out movement);
        }
        public void PlayActionAnimation(MotionState motion) {
            person.PlayActionAnimation(motion);
        }

        public void SetAbsoluteGuardCounter(float time) {
            person.SetAbsoluteGuardCounter(time);
        }

        public void SetModelRotation(Quaternion localRotation) {
            person.SetModelRotation(localRotation);
        }
    }
	#endregion

	#region フィールド、プロパティ
	public override StateProc State
	{
		get { return this.PState.StateProc; }
	}
	private PersonStateAdapter pSAdapter;
	private PersonStateAdapter PSAdapter
	{
		get
		{
			if(this.pSAdapter == null)
			{
				this.pSAdapter = new PersonStateAdapter(this);
			}
			return pSAdapter;
		}
	}
	private PersonState.PersonState pState;
	private PersonState.PersonState PState
	{
		get
		{
			if(pState == null)
			{
				pState = new PersonState.Move(this.PSAdapter);
			}
			return pState;
		}
		set
		{
			if(pState != null)
			{
				pState.Finish();
			}
			pState = value;
		}
	}

	/// <summary>
	/// モデルの描画を行うか否か.
	/// </summary>
	public override bool IsDrawEnable { get { return !IsDrawLimit && !IsDisappear && IsMoved; } }

	private bool isMoved;
	public bool IsMoved
	{
		get { return isMoved; }
		private set
		{
			bool oldIsDrawEnable = IsDrawEnable;
			isMoved = value;
			if(oldIsDrawEnable != IsDrawEnable)
			{
				this.AvaterModel.ChangeDrawEnable(IsDrawEnable);
			}
		}
	}

	private bool isDrawLimit;
	public bool IsDrawLimit
	{
		get { return isDrawLimit; }
		set
		{
			bool oldIsDrawEnable = IsDrawEnable;
			isDrawLimit = value;
			if(oldIsDrawEnable != IsDrawEnable)
			{
				this.AvaterModel.ChangeDrawEnable(IsDrawEnable);
			}
		}
	}
	#endregion

	#region セットアップ
	public static void Setup(GameObject go, Manager manager, PersonInfo info)
	{
		// コンポーネント取得
		Person person = go.GetSafeComponent<Person>();
        person.gameObject.layer = PERSON_LAYER;

        if (person == null)
		{
			manager.Destroy(go);
			return;
		}
		// 初期設定
		person.SetupCharacter(manager, info);
		person.MovePacketTime = Time.time;
		person.IsMoved = info.IsMoved;
		person.SetupCompleted();
	}
	protected override string GetObjectUIPath()
	{
		return GameGlobal.GetObjectUIPersonPath();
	}
	protected override void Awake()
	{
		base.Awake();

		this.PersonSkillMotionParam = new PersonSkillMotionParam(this);
		// 移動処理設定
		this.PState = new PersonState.Move(this.PSAdapter);
	}
    #endregion

    #region 更新

    private float _nextSendConfirmMoveTime = 0;

	protected override void Update()
	{
		// モーションパケットの処理.
		// base.Update()より前にやらないとモーションに付属したエフェクトの位置などがおかしくなる.
		if (AvaterModel.ModelTransform)
		{
			AvaterModel.ModelTransform.localRotation = Quaternion.identity;
		}
		MotionFiber.MoveNext();

        if (this.EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            CheckFall();
            this.CharacterMove.MoveInertia(Time.deltaTime);
            bool needSendMove = Vector3.Distance(this.Position, this.transform.position) > 0.1f;
            this.Position = this.transform.position;
            if (needSendMove) {
                SendMovePacket();
            }

            if (Time.fixedTime > _nextSendConfirmMoveTime) {
                SendConfirmMovePacket(false);
                _nextSendConfirmMoveTime = Time.fixedTime + 1.0f;
            }
        }

        this.UpdateCounter();
        this.UpdatePState();

		base.Update();
	}
    /// <summary>
    /// 絶対防御時間
    /// </summary>
    public float AbsoluteGuardCounter { get; protected set; }
    private void UpdateCounter() {
        this.airTechDelayCounter -= Time.deltaTime;
        this.AbsoluteGuardCounter -= Time.deltaTime;
    }

    void SendMovePacket() {
        // パケット送信間隔がまだ来てない
        if (this.SendMoveNextTime >= Time.time) { return; }
        ForceSendMovePacket();
    }
    void ForceSendMovePacket() {
        this.SendMoveNextTime = Time.time + GameConstant.MovePacketInterval;
        CommonPacket.ProxySendMove(this.InFieldId, this.OldSendPosition, this.Position, this.Rotation);
        this.OldSendPosition = this.Position;
    }
    void SendConfirmMovePacket(bool collision) {
        CommonPacket.SendConfirmMove(this.InFieldId, this.Position, collision);
    }
    Vector3 OldSendPosition { get; set; }
    public float SendMoveNextTime { get; private set; }
    #endregion

    #region 落下判定
    // 落下時のKillSelf連続送信防止用(boolだと送信ミス時に困るので一応Timeにしてあります).
    const float KillSelfHeight = -5f;	// KillSelfを送信するY座標.
    const float ResendTime = 5f;		// KillSelf再送信までの時間(通信エラー対策).
    float sendFallTime = 0f;
    private void CheckFall() {
        if (this.Position.y > KillSelfHeight) { return; }
        if (sendFallTime > Time.time) { return; }

        if (this.State != Character.StateProc.Dead) {
            // 復帰場所選択中は送らない
            if (!(GUIMapWindow.Mode == GUIMapWindow.MapMode.Respawn || GUIDeckEdit.NowMode != GUIDeckEdit.DeckMode.None)) {
                // KillSelf送信
                BattlePacket.ProxySendKillSelf(this.InFieldId);
            }

            // 復帰or一定時間経過まで送信を控える.
            sendFallTime = Time.time + ResendTime;
        }
    }
    #endregion

    #region UpdateProc
    private void UpdatePState()
	{
		if(!this.PState.Update())
		{
			this.StateCheck();
		}
	}

	protected void MoveProc()
	{
		Vector3 velocity;
		this.UpdatePosition(1f, out velocity);
		this.UpdateRotation();
		this.UpdateMoveAnimation(this.MoveVelocity * Time.deltaTime, mMoveMotionID);
	}
	protected void SkillMotionProc()
	{
		this.SkillMotionProcCharacter();
	}
	#endregion
	#region FinishProc
	protected void SkillMotionFinish()
	{
		this.SkillMotionFinishCharacter();
		
		// レイヤーを元に戻す
//		this.gameObject.layer = LayerNumber.vsBullet;
		this.gameObject.layer = PERSON_LAYER;
        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            CommonPacket.ProxySendMotionFinished(InFieldId, Position, Rotation);
        }
	}
	#endregion

	#region ObjectBase Override
	#region 移動パケット
	float MovePacketTime { get; set; }		// 初期値が必要
	Vector3 MoveVelocity { get; set; }
	int mMoveMotionID = 1;
	/// <summary>
	/// 移動
	/// </summary>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	public override void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
	{
		this.IsMoved = true;

        this.NextPosition = position;
		this.NextRotation = rotation;

        float packetInterval = GameConstant.MovePacketInterval;//Mathf.Max(Time.time - MovePacketTime, 0.01f);
		this.MoveVelocity = (this.NextPosition - this.Position) / (packetInterval);
		MovePacketTime = Time.time;

		// 前回との位置の差がなければモーションを停止.
		if ((position - oldPosition).sqrMagnitude < 0.01f)
		{
			mMoveMotionID = 0;
		}
		else
		{
			mMoveMotionID = 1;
		}
	}
	/// <summary>
	/// スキル使用中移動処理
	/// </summary>
	protected override void SkillMoveProc()
	{
		Vector3 velocity;
		this.UpdatePosition(this.moveSpeedRatio, out velocity);
		this.UpdateRotation();
		this.UpdateMoveAnimation(this.MoveVelocity * Time.deltaTime, moveMotionID);
	}
	/// <summary>
	/// 現在地更新
	/// Position から NextPosition に移動する
	/// </summary>
	/// <param name="velocity">移動ベクトルを返す</param>
	protected void UpdatePosition(float speedRatio, out Vector3 velocity)
	{
		Vector3 prevPosition = this.Position;

		// 移動ベクトル、距離を求める
		Vector3 moveDirection = this.MoveVelocity * Time.deltaTime;

		// 一回に進む距離を求める
		// 一回に進む距離が一定以上なら補正する
		Vector3 def = this.NextPosition - this.Position;
		Vector3 movement;
		if (def.sqrMagnitude < moveDirection.sqrMagnitude)
		{
			movement = def;
		}
		else
		{
			movement = moveDirection;
		}
		// 移動
		this.MovePosition(movement);
        
		// 移動ベクトルを返す
		velocity = this.Position - prevPosition;
	}
	#endregion

	#region スキルモーションパケット
	public PersonSkillMotionParam PersonSkillMotionParam { get; protected set; }
	public override SkillMotionParam SkillMotionParam { get { return PersonSkillMotionParam; } }

	public override bool SkillMotion(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
	{
		if (!this.PersonSkillMotionParam.StartSkill(skillID, target))
			{ return false; }

        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            CommonPacket.ProxySendMotionStarted(this.InFieldId, position, rotation);
        }

        base.SkillMotionCharacter(position, rotation);
		// スキルモーション処理設定
		this.PState = new PersonState.SkillMotion(this.PSAdapter);

		// SkillMotionCharacter内でStateを変更した際に
		// SkillMotionFinishが呼ばれる可能性があるので
		// それより後に処理する必要がある
		if (this.SkillMotionParam.IsSelfBullet)
		{
			// レイヤーを変更する
			// 自分弾丸で壁を突き破ってしまうため
			this.gameObject.layer = LayerNumber.Player;
		}

		// 回避スキルかどうか.
		if (this.SkillMotionParam.Skill.SkillType == SkillType.Avoid)
		{
			// 見た目に弾丸をすり抜けさせるための無敵設定.
			this.InvincibleCounter = 0.5f;
		}

		return true;
	}
	public override void SkillCharge(int skillID, ObjectBase target, bool isCharge)
	{
		this.SkillChargeBase(skillID, target, isCharge);

		// 次のチャージスキルに移行する
		if (!isCharge)
		{
			this.SkillMotion_SetDelay();
		}
	}
	const float ChargeExtendTime = 3f;
	protected override IEnumerator SkillMotion_MotionChargeCoroutine(float chargeTimer, int nextSkillID, int abortSkillID, ObjectBase target)
	{
		// チャージタイムが終わってからすぐ次の行動に移ってしまうため
		// 多少チャージタイムを伸ばしてエフェクトが出なくなるのを防ぐ
		return base.SkillMotion_MotionChargeCoroutine(chargeTimer + ChargeExtendTime, nextSkillID, abortSkillID, target);
	}
	#endregion

	#region 攻撃パケット
	public override void Attack(int bulletSetID, ObjectBase target, Vector3 position, Quaternion rotation, Vector3? casterPos, Quaternion casterRot)
	{
		SkillBulletSetMasterData bulletSet;
		if (!MasterData.TryGetBulletSet(bulletSetID, out bulletSet))
			{ return; }

		// 自分弾丸
		if (casterPos != null)
		{
			this.SelfAttack((Vector3)casterPos, casterRot);
		}

		// 弾丸生成
		this.CreateBullet(target, null, position, rotation, bulletSet.SkillID, bulletSet);
	}
	void SelfAttack(Vector3 casterPos, Quaternion casterRot)
	{
		// キャスターの位置設定
		this.SetPosition(casterPos);
		this.SetNextPosition(casterPos);
		this.SetRotation(casterRot);
		this.SetNextRotation(casterRot);
	}
	#endregion

	#region 状態異常パケット
	public override void Dead(HitInfo hitInfo)
	{
        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            // ユニークモーション取得.
            bool isUniqueMotion = false;
            if (hitInfo.inFieldAttackerId == this.InFieldId) {
                string motionName;
                if (UniqueMotion.TryGetDeadSelfMotionName(hitInfo.bulletID, out motionName)) {
                    this.SetUniqueDeadCoroutine(motionName);
                    isUniqueMotion = true;
                }
            }
            if (!isUniqueMotion) {
                // ユニークモーションではない.
                this.SetDownBlownCoroutine(hitInfo.blownAngleType, hitInfo.bulletDirection, true);
            }
        } else {
            // 死亡処理設定
            this.PState = new PersonState.Dead();
        }
    }
	public override void Respawn(Vector3 position, Quaternion rotation)
	{
		this.RespawnCharacter(position, rotation);
		// 移動処理設定
		this.PState = new PersonState.Move(this.PSAdapter);
	}
	public override void Warp(Vector3 position, Quaternion rotation)
	{
		this.WarpCharacter(position, rotation);
		// 移動処理設定
		this.PState = new PersonState.Move(this.PSAdapter);
	}
	public override void Grapple(GrappleAttach grappleAttach)
	{
		if(this.State == StateProc.Dead){
			return;
		}

        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            if (this.State == StateProc.Dead) {
                return;
            }

            float grappleDelay = 0f;
            // キャスターが自分.
            if (grappleAttach.Caster == this) {
                grappleDelay = grappleAttach.GrappleData.DelayTime;
            }
            this.PState = new PersonState.Grapple(this.PSAdapter, grappleDelay);
            base.GrappleStart(grappleAttach);
        } else {
            float grappleDelay = 0f;
            this.PState = new PersonState.Grapple(this.PSAdapter, grappleDelay);
            this.GrappleStart(grappleAttach);
        }
	}
    public override void GrappleFinish(GrappleAttach grappleAttach) {
        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            if (grappleAttach.Target == this) {
                // ターゲットが自分.
                if (grappleAttach.IsFinish) {
                    // 正常終了. 吹き飛ぶ.
                    this.Down(grappleAttach.GrappleData.SkillBlowPatternID, grappleAttach.HitInfo.bulletDirection);
                } else {
                    // 中断. その場ダウン.
                    this.Down(0, grappleAttach.HitInfo.bulletDirection);
                }
            }
        } else {
            base.GrappleFinish(grappleAttach);
        }
    }

    public GrappleAttach GrappleAttach { get { return this.grappleAttach; } }

    #endregion

    #region 効果パケット
    // 吹き飛びスピード.
    const float BlowSpeedS = 0f;
    const float BlowSpeedM = 2f;
    const float BlowSpeedL = 3.75f;
    public override void Effect(HitInfo hitInfo)
	{
		this.EffectBase(hitInfo.effectType);
		
		// スーパーアーマー中,投げ＆投げられ中は怯み系無効.
		if (this.IsSuperArmor ||
			this.State == StateProc.Grapple){
			return;
		}

		// 効果処理
		switch(this.EffectType)
		{
        case EffectType.None:
            break;
		case EffectType.Guard:
			this.Guard();
			break;
        case EffectType.SuperArmor:
            break;
        case EffectType.Down:
            if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                this.Down(hitInfo.blownAngleType, hitInfo.bulletDirection);
            }
            break;
        case EffectType.FalterMicro:
            break;
        case EffectType.FalterSmall:
            if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                this.Falter(MotionState.damage_s, BlowSpeedS, hitInfo.bulletDirection);
            }
            break;
        case EffectType.FalterMedium:
            if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                this.Falter(MotionState.damage_m, BlowSpeedM, hitInfo.bulletDirection);
            }
            break;
        case EffectType.FalterLarge:
            if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                this.Falter(MotionState.damage_l, BlowSpeedL, hitInfo.bulletDirection);
            }
            break;
        }
	}
	#endregion

	#region ジャンプパケット
	public override void Jump(Vector3 position, Quaternion rotation) {
        if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            // ジャンプ移動量.
            Vector3 vec = position - this.Position;

            // キャラを目的地に向ける.
            Quaternion rot = Quaternion.LookRotation(new Vector3(vec.x, 0, vec.z));
            ResetTransform(this.transform.position, rot);

            // State変更、コルーチン始動.
            StartCoroutine(BotJump(position, rot, vec));
        }
    }
	public override void Wire(Vector3 position, Quaternion rotation) { }


    private IEnumerator BotJump(Vector3 position, Quaternion rotation, Vector3 velocity) {
        const float fixedTime = 0.05f;              // 位置計算間隔.低フレームレート時の計算ズレ,障害物超え失敗を防ぐ.
        const float GravityMag = 10f;               // 重力倍率.カッコよく見せるための嘘数値.
        const float JumpTime = 1;                   // 所要時間(現在1秒固定).

        float startTime = Time.time;
        Vector3 start = this.transform.position;
        float calculateTime = fixedTime;
        float maxG = this.CharacterMove.GravityBaseY * GravityMag * JumpTime * 0.5f;   // 終了時の重力=-開始時の上方向の力.
        Vector3 end = position;
        this.ForceSendMovePacket();

        {
            // エフェクト.
            velocity.y -= maxG;
            EffectManager.Create(GameConstant.EffectJumpBase, this.transform.position, this.transform.rotation);
            EffectManager.Create(GameConstant.EffectJumpDirect, this.transform.position, Quaternion.LookRotation(velocity));

            // ベクトル設定.
            this.CharacterMove.DirectionReset();
            this.CharacterMove.IsGrounded = false;
            this.CharacterMove.Velocity = velocity;
        }

        // 上昇中.
        IEnumerator jumpProcAnimFiber = this.BotJumpProcAnimUp();
        Vector3 outMove = Vector3.up;
        while (0 < outMove.y) {
            float elapsedTime = Time.time - startTime;
            for (; calculateTime < elapsedTime; calculateTime += fixedTime) {
                float t = calculateTime / JumpTime;
                // 目的地への直線移動と重力の放物線から位置を求める
                Vector3 pos = Vector3.Lerp(start, end, t) + new Vector3(0, (t * t - t) * maxG, 0);
                Vector3 move = pos - this.transform.position;
                this.CharacterMove.CalculateMove(move / fixedTime, fixedTime, out outMove);
                this.MovePosition(outMove, false);
            }

            this.CharacterMove.GravityMag = GravityMag;
            this.CharacterMove.UseInertia = false;
            jumpProcAnimFiber.MoveNext();

            yield return null;
        }

        this.gameObject.layer = PERSON_LAYER;
        // 下降中.
        jumpProcAnimFiber = this.BotJumpProcAnimDown();
        while (calculateTime < JumpTime) {
            float elapsedTime = Mathf.Min(Time.time - startTime, JumpTime);
            for (; calculateTime < elapsedTime; calculateTime += fixedTime) {
                float t = calculateTime / JumpTime;
                // 目的地への直線移動と重力の放物線から位置を求める
                Vector3 pos = Vector3.Lerp(start, end, t) + new Vector3(0, (t * t - t) * maxG, 0);
                Vector3 move = pos - this.transform.position;
                this.CharacterMove.CalculateMove(move / fixedTime, fixedTime, out outMove);
                this.MovePosition(outMove, true);
            }

            this.CharacterMove.GravityMag = GravityMag;
            this.CharacterMove.UseInertia = false;
            jumpProcAnimFiber.MoveNext();

            yield return null;
        }

        this.MovePosition(end - this.transform.position, true);
        this.CharacterMove.Velocity = new Vector3(0, maxG, 0);

        // 落下に移行.次フレームに回すとVelocityがリセットされることがあるので即実行.
        this.SetFallProcCoroutine(jumpProcAnimFiber);
        yield return null;
    }

    IEnumerator BotJumpProcAnimUp() {
        return this.BotOrderAnimCoroutine(MotionState.jump_up_sta, MotionState.jump_up_lp);
    }

    IEnumerator BotJumpProcAnimDown() {
        return this.BotOrderAnimCoroutine(MotionState.jump_dw_sta, MotionState.jump_dw_lp);
    }

    IEnumerator BotOrderAnimCoroutine(MotionState first, MotionState end) {
        this.SendMotion(first);
        float motionTime = Time.time + this.pSAdapter.Person.ScmAnimation.GetAnimationLength(first.ToString());
        while (Time.time < motionTime) {
            yield return null;
        }
        this.SendMotion(end);
    }

    #endregion

    #region モーションパケット
    IEnumerator MotionFiber = MotionNone();

	public override void Motion(MotionState motionstate)
	{
        // 先にStateをMoveに戻してSkillMotionFinish()を呼んでおかないと途中でモーションリセットされてしまう.
        /*
        bool needFinish = true;
        if (motionstate == MotionState.jump_dw_sta || motionstate == MotionState.jump_up_lp || motionstate == MotionState.jump_end
            || motionstate == MotionState.jump_up_sta || motionstate == MotionState.jump_dw_lp) {
            if (EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                needFinish = false;
            }
        }
        if (needFinish) {
            this.PState = new PersonState.Move(this.PSAdapter); // DeadやGrappleから復帰してしまう.
        }*/
        this.PState = new PersonState.Move(this.PSAdapter); // DeadやGrappleから復帰してしまう.

        switch (motionstate)
		{
			// モーションリセット.
		case MotionState.wait:
			MotionFiber = MotionNone();
			this.ResetAnimation();
			break;
			
			// モーション再生.
		case MotionState.wake:
			this.ScmAnimation.UpdateAnimation(motionstate, (int)MotionLayer.ReAction);
			break;
		case MotionState.down_end:
			MotionFiber = MotionDownEnd();
			break;
		case MotionState.dead_end:
			MotionFiber = MotionDownEnd();
			break;
		case MotionState.maneuver_up_sta:
			MotionFiber = MotionManeuverUpSta();
			break;
		case MotionState.maneuver_dw_sta:
			MotionFiber = MotionManeuverDwSta();
			break;
		case MotionState.maneuver_f_up_sta:
			MotionFiber = MotionManeuverFUpSta();
			break;
		case MotionState.maneuver_f_dw_sta:
			MotionFiber = MotionManeuverFDwSta();
			break;

			// モーション再生＆弾丸消滅.
		case MotionState.down_sta:
			this.CharacterVoice.Play(CharacterVoice.CueName_down);
			MotionFiber = MotionDownSta(false);
			this.DeleteAllBullets();
			break;
		case MotionState.down_sta_spin:
			this.CharacterVoice.Play(CharacterVoice.CueName_down);
			MotionFiber = MotionDownSta(true);
			this.DeleteAllBullets();
			break;
		case MotionState.dead_sta:
			this.CharacterVoice.Play(CharacterVoice.CueName_dead);
			MotionFiber = MotionDownSta(true);
			this.DeleteAllBullets();
			break;
        case MotionState.jump_dw_sta:
            MotionFiber = MotionJumpDwSta();
            break;
        case MotionState.jump_end:
            MotionFiber = MotionJumpEnd();
            break;
        case MotionState.jump_up_sta:
            MotionFiber = MotionJumpUpSta();
            this.DeleteAllBullets();
            break;
		case MotionState.damage_s:
		case MotionState.damage_m:
		case MotionState.damage_l:
		case MotionState.dizzy:
		case MotionState.dead_self_sp:
			// ボイス.
			this.CharacterVoice.Play(MotionName.GetName(motionstate));
			this.ScmAnimation.UpdateAnimation(motionstate, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
			this.DeleteAllBullets();
			break;

			// 送らないものエラー.
		case MotionState.run:
		case MotionState.run_001_f:
		case MotionState.run_001_b:
		case MotionState.run_001_l:
		case MotionState.run_001_r:
		case MotionState.guard_damage:
		case MotionState.down_mid:
		case MotionState.jump_up_lp:
		case MotionState.jump_dw_lp:
			BugReportController.SaveLogFile("Received an unusual MotionState : "+motionstate);
			break;
			
			// 存在しないものエラー.
		default:
			BugReportController.SaveLogFile("unknown MotionState : "+motionstate);
			break;
		}
	}

	/// <summary>
	/// firstモーション再生終了後にendモーションを再生する.
	/// </summary>
	IEnumerator OrderAnimCoroutine(MotionState first, MotionState end)
	{
        this.ScmAnimation.UpdateAnimation(first, (int)MotionLayer.ReAction);
		float motionTime = Time.time + this.ScmAnimation.GetAnimationLength(first.ToString());
        while (Time.time < motionTime) {
            yield return null;
        }
        this.ScmAnimation.UpdateAnimation(end, (int)MotionLayer.ReAction);
    }


    /// <summary>
    /// モーションを順に再生する.
    /// </summary>
    IEnumerator BotOrderAnimCoroutine(params MotionState[] motionStates) {
        foreach (MotionState motionState in motionStates) {
            this.SendMotion(motionState);
            this.ScmAnimation.UpdateAnimation(motionState, (int)MotionLayer.ReAction);
            float motionTime = Time.time + this.ScmAnimation.GetAnimationLength(motionState.ToString());
            while (Time.time < motionTime) { yield return null; }
        }
    }

    /// <summary>
    /// 空のモーションファイバー.
    /// </summary>
    static IEnumerator MotionNone()
	{
		yield return null;
	}
	/// <summary>
	/// ジャンプ上昇アニメーション.
	/// </summary>
	IEnumerator MotionJumpUpSta()
	{
        // エフェクト.
        // Huhao: to avoid blocked by wall
        this.gameObject.layer = LayerNumber.vsPlayer;
        StartCoroutine(JumpEffect());
		return this.OrderAnimCoroutine(MotionState.jump_up_sta, MotionState.jump_up_lp);
	}
	/// <summary>
	/// ジャンプ下降アニメーション.
	/// </summary>
	IEnumerator MotionJumpDwSta()
	{
        return this.OrderAnimCoroutine(MotionState.jump_dw_sta, MotionState.jump_dw_lp);
	}
	/// <summary>
	/// ジャンプ着地アニメーション.
	/// </summary>
	IEnumerator MotionJumpEnd()
	{
        this.gameObject.layer = PERSON_LAYER;
        this.ScmAnimation.UpdateAnimation(MotionState.jump_end, (int)MotionLayer.ReAction);
		// エフェクト.
		EffectManager.CreateDown(this, GameConstant.EffectDown);
		yield return null;
	}
	/// <summary>
	/// ジャンプエフェクト.
	/// </summary>
	IEnumerator JumpEffect()
	{
		while (this.MoveVelocity.sqrMagnitude < float.MinValue)
		{
			yield return new WaitForEndOfFrame();
		}
		EffectManager.Create(GameConstant.EffectJumpBase, this.transform.position, this.transform.rotation);
		EffectManager.Create(GameConstant.EffectJumpDirect, this.transform.position, Quaternion.LookRotation(this.MoveVelocity));
	}
	/// <summary>
	/// ダウン開始アニメーション.
	/// </summary>
	IEnumerator MotionDownSta(bool isSpin)
	{
		this.ScmAnimation.UpdateAnimation(MotionState.down_sta, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
		float motionTime = Time.time + this.ScmAnimation.GetAnimationLength(MotionState.down_sta.ToString());
		while (Time.time < motionTime)
		{
			// 角度.
			if (AvaterModel.ModelTransform)
			{
				AvaterModel.ModelTransform.LookAt(this.Position - this.MoveVelocity);
			}
			yield return null;
		}
		if(isSpin)
		{
			this.ScmAnimation.UpdateAnimation(MotionState.down_mid_spin, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
		}
		else
		{
			this.ScmAnimation.UpdateAnimation(MotionState.down_mid, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
		}

		while (true)
		{
			// 角度.
			if (AvaterModel.ModelTransform)
			{
				AvaterModel.ModelTransform.LookAt(this.Position - this.MoveVelocity);
			}
			yield return null;
		}
	}
	/// <summary>
	/// ダウン終了アニメーション.
	/// </summary>
	IEnumerator MotionDownEnd()
	{
		this.ScmAnimation.UpdateAnimation(MotionState.down_end, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
		// エフェクト.
		EffectManager.CreateDown(this, GameConstant.EffectDown);

		yield return null;
	}
	/// <summary>
	/// 立体起動UPアニメーション.
	/// </summary>
	IEnumerator MotionManeuverUpSta()
	{
		return this.OrderAnimCoroutine(MotionState.maneuver_up_sta, MotionState.maneuver_up_lp);
	}
	/// <summary>
	/// 立体起動UP終了アニメーション.
	/// </summary>
	IEnumerator MotionManeuverDwSta()
	{
		return this.OrderAnimCoroutine(MotionState.maneuver_dw_sta, MotionState.jump_dw_lp);
	}
	/// <summary>
	/// 立体起動Fアニメーション.
	/// </summary>
	IEnumerator MotionManeuverFUpSta()
	{
		return this.OrderAnimCoroutine(MotionState.maneuver_f_up_sta, MotionState.maneuver_f_up_lp);
	}
	/// <summary>
	/// 立体起動F終了アニメーション.
	/// </summary>
	IEnumerator MotionManeuverFDwSta()
	{
		return this.OrderAnimCoroutine(MotionState.maneuver_f_dw_sta, MotionState.jump_dw_lp);
	}
	#endregion

	#region 経験値処理
	protected override Vector3 ExpEffectOffsetMin{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	protected override Vector3 ExpEffectOffsetMax{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	#endregion

	#region バフ処理
	public override void ChangeBuff(BuffType[] newBuffTypes, BuffPacketParameter[] buffPacket, BuffPacketParameter[] spBuffPacket, float speed)
	{
		bool oldParalysis = this.IsParalysis;

		// バフ処理.
		base.ChangeBuff(newBuffTypes, buffPacket, spBuffPacket, speed);

		// 状態効果アイコン表示.
		if (ObjectUIRoot != null)
		{
			// バフ.SPバフがかかった順に並び替えて取得する
			LinkedList<BuffInfo> buffInfoList = BuffInfo.GetBuffInfoList(newBuffTypes, buffPacket, spBuffPacket);
			// 表示
			this.ObjectUIRoot.SetBuffIcon(buffInfoList);
		}

		// 麻痺判定.
		if (this.IsParalysis && !oldParalysis)
		{
			// ボイス.
			this.CharacterVoice.Play(CharacterVoice.CueName_damage_l);
		}
	}
	#endregion
	#endregion

	#region Character Override
	#region ステートチェック
	/// <summary>
	/// 現在のバフ状況などから次のステートを決める.
	/// </summary>
	protected override void StateCheck()
	{
		this.PState = new PersonState.Move(this.PSAdapter);
	}
    #endregion
    #endregion

    #region AI related
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (this.EntrantInfo != null && this.EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
            /// If a bot is collision with a pc or npc, must notify the server
            
            // FIXME: is this collider a wall? or a npc?
            ObjectBase chara = hit.collider.gameObject.GetComponent<ObjectBase>();
            if (chara != null) {
                this.MoveVelocity = Vector3.zero;
                SendConfirmMovePacket(true);
            }
        }
    }

    void SetFallProcCoroutine(IEnumerator fallAnimFiber) {
        // State変更、コルーチン始動.
        this.PState = new PersonState.PersonFall(this.PSAdapter, fallAnimFiber);
        this.CharacterMove.IsGrounded = false;
        this.PState.Update();
    }
    void Fall() {
        this.SetFallProcCoroutine(BotJumpProcAnimDown());
    }
    public void Down(int blownAngleType, float bulletDirection) {
        if (this.State == StateProc.Wake) {
            return;
        }
        // 処理設定
        if (this.State == StateProc.Dead) {
            // Deadの場合,Stateはそのままで吹き飛びなおす(投げの場合とか).
            this.SetDownBlownCoroutine(blownAngleType, bulletDirection, true);
        } else {
            this.SetDownBlownCoroutine(blownAngleType, bulletDirection, false);
        }
        // カメラを揺らす
        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc) {
                cc.Shake();
            }
        }
    }

    public void Wake() {
        // 処理設定
        IEnumerator wakeMotionFiber = this.BotOrderAnimCoroutine(MotionState.wake);
        this.PState = new PersonState.Wake(this.PSAdapter, wakeMotionFiber);
    }

    private void SetAbsoluteGuardCounter(float time) {
        this.AbsoluteGuardCounter = time;
    }

    public void Falter(MotionState motion, float blowSpeed, float bulletDirection) {
        if (this.PState.CanFalter()) {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, bulletDirection + 180, 0));
            this.SetNextRotation(this.transform.rotation);

            this.PState = new PersonState.Falter(this.PSAdapter, this.transform.forward * -1, blowSpeed, motion);
        }
    }

    private float airTechDelayCounter = 0f;
    private void SetUniqueDeadCoroutine(string motionName) {
        this.PState = new PersonState.Dead(this.PSAdapter, UniqueMotionFiber(motionName));
    }
    private void SetDownBlownCoroutine(int blownAngleType, float bulletDirection, bool isDead) {
        // 吹き飛び方向.
        Vector3 blownVec = Vector3.zero;
        SkillBlowPatternMasterData blowData;
        if (MasterData.TryGetSkillBlowPattern(blownAngleType, out blowData)) {
            blownVec = new Vector3(blowData.VectorX, blowData.VectorY, blowData.VectorZ);
            this.airTechDelayCounter = blowData.AirtechTiming;
        } else {
            this.airTechDelayCounter = -1;
        }
        bool isSpin = false;
        if (this.airTechDelayCounter < 0) {
            this.airTechDelayCounter = float.MaxValue;
            isSpin = true;
        }

        // Down or Dead.
        if (isDead) {
            this.CharacterVoice.Play(CharacterVoice.CueName_dead);
            this.PState = new PersonState.Dead(this.PSAdapter, blownVec, bulletDirection, MotionState.dead_sta, MotionState.dead_mid, MotionState.dead_end);
        } else if (isSpin) {
            this.CharacterVoice.Play(CharacterVoice.CueName_down);
            this.PState = new PersonState.Down(this.PSAdapter, blownVec, bulletDirection, MotionState.down_sta_spin, MotionState.down_mid_spin, MotionState.down_end);
        } else {
            this.CharacterVoice.Play(CharacterVoice.CueName_down);
            this.PState = new PersonState.Down(this.PSAdapter, blownVec, bulletDirection, MotionState.down_sta, MotionState.down_mid, MotionState.down_end);
        }
    }
    private IEnumerator UniqueMotionFiber(params string[] motionNames) {
        List<MotionState> motionStateList = new List<MotionState>();
        foreach (string motionName in motionNames) {
            MotionState motionState;
            try {
                // HACK : 本来はEnum.TryParseを使うべきだが.Net4からなので無理.
                motionState = (MotionState)System.Enum.Parse(typeof(MotionState), motionName);
                motionStateList.Add(motionState);
            } catch (System.Exception e) {
                BugReportController.SaveLogFileWithOutStackTrace(motionName + e.ToString());
            }
        }
        return this.BotOrderAnimCoroutine(motionStateList.ToArray());
    }
    private void PlayActionAnimation(MotionState motionState) {
        this.ScmAnimation.UpdateAnimation(motionState, (int)MotionLayer.ReAction);
        this.SendMotion(motionState);
    }
    private void SetModelRotation(Quaternion localRotation) {
        if (AvaterModel.ModelTransform) {
            AvaterModel.ModelTransform.localRotation = localRotation;
        }
    }
    #region モーションパケット
    private void SendMotion(MotionState motionstate) {
        switch (motionstate) {
        // 送信のみ.
        case MotionState.wait:  // モーションリセット.
        case MotionState.down_end:
        case MotionState.dead_end:
        case MotionState.wake:
        case MotionState.jump_dw_sta:
        case MotionState.jump_end:
        case MotionState.maneuver_up_sta:
        case MotionState.maneuver_dw_sta:
        case MotionState.maneuver_f_up_sta:
        case MotionState.maneuver_f_dw_sta:
            BattlePacket.ProxySendMotion(this.InFieldId, motionstate);
            break;

        // 弾丸消滅＆送信.
        case MotionState.down_sta:
        case MotionState.down_sta_spin:
        case MotionState.dead_sta:
        case MotionState.jump_up_sta:
        case MotionState.damage_s:
        case MotionState.damage_m:
        case MotionState.damage_l:
        case MotionState.dizzy:
        case MotionState.dead_self_sp:
            BattlePacket.ProxySendMotion(this.InFieldId, motionstate);
            this.DeleteAllBullets();    // TODO: really need?
            break;

        // 送らないもの.
        case MotionState.run:
        case MotionState.run_001_f:
        case MotionState.run_001_b:
        case MotionState.run_001_l:
        case MotionState.run_001_r:
        case MotionState.guard_damage:
        case MotionState.down_mid:
        case MotionState.down_mid_spin:
        case MotionState.dead_mid:
        case MotionState.jump_up_lp:
        case MotionState.jump_dw_lp:
        case MotionState.maneuver_up_lp:
        case MotionState.maneuver_f_up_lp:
            //BugReportController.SaveLogFile("do not send MotionState : "+motionstate);
            break;

        // 存在しないものエラー.
        default:
            BugReportController.SaveLogFile("unknown MotionState : " + motionstate);
            break;
        }
    }
    #endregion

    #region タイミング処理

    public bool IsFixedAttackRotation { get; private set; }
    public List<IEnumerator> BulletKeepProcessFiberList { get; private set; }

    #region 弾丸発射処理
    public List<IEnumerator> SetBulletFiber(SkillMasterData skill, ObjectBase target) {
        List<IEnumerator> fiberList = new List<IEnumerator>();
        this.BulletKeepProcessFiberList = new List<IEnumerator>();
        if (skill.BulletSetList != null) {
            foreach (var t in skill.BulletSetList) {
                SkillBulletMasterData bullet = t.Bullet;
                if (bullet == null)
                    continue;
                IEnumerator fiber = this.SkillMotion_BulletCoroutine(t.ShotTiming, t.TargetPositionTming, target, null, t, bullet);
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
                var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
                if (com != null && com.attackMotionResetConfig.isEnable) {
                    this.BulletKeepProcessFiberList.Add(fiber);
                    continue;
                }
#endif
                // 処理を中断するかどうか
                // キャンセルポイント以降に設定されている場合があるのでそのフラグ
                if (t.AbortFlag)
                    fiberList.Add(fiber);
                else
                    this.BulletKeepProcessFiberList.Add(fiber);
            }
        }

        return fiberList;
    }
    IEnumerator SkillMotion_BulletCoroutine(float shotTimer, float targetPositionTimer, ObjectBase target, Vector3? targetPosition, SkillBulletSetMasterData bulletSet, SkillBulletMasterData bullet) {
        // ターゲット変更
        // エイミングマーカー時にターゲットが変更されているかどうか
        if (this.SkillMotionParam.IsAimingSkill) {
            target = GUIObjectUI.LockonObject;
        }
        // 近接自動ターゲットフラグ時は強制変更
        if (bullet.Type == SkillBulletMasterData.BulletType.Nearest)
            target = GameController.SearchGrappleTarget(this, GameConstant.BulletNearestSearchRange);

        // 発射ヌル取得
        Transform attachNull = this.transform;
        {
            string name = bulletSet.Bullet.AttachNull;
            if (!string.IsNullOrEmpty(name)) {
                attachNull = this.transform.Search(name);
                if (attachNull == null) {
                    attachNull = this.transform;

                    Debug.LogWarning(string.Format("[{0}]が見つからない", name));
                }
            }
        }

        // キャストマーカー
        SkillCastMarkerMasterData castMarkerData;
        MasterData.TryGetCastMarker(bullet.ID, out castMarkerData);

        // 発生弾は発生位置を求める
        Vector3 birthPosition = Vector3.zero;
        {
            bool isTargetUpdate = true;
            while (shotTimer > 0f) {
                // 発射タイミング
                shotTimer -= Time.deltaTime;

                // ターゲット位置更新
                if (targetPositionTimer <= 0f) {
                    if (isTargetUpdate) {
                        MarkerBase.CalculateBirth(this, target, targetPosition, bullet.Range, attachNull.position, bulletSet, out birthPosition);
                        isTargetUpdate = false;
                        // キャストマーカー生成
                        if (castMarkerData != null) {
                            EffectManager.CreateCastMarker(this, GUIObjectUI.LockonObject, targetPosition, bullet.Range, bulletSet, castMarkerData);
                        }
                    }
                } else {
                    targetPositionTimer -= Time.deltaTime;
                }

                yield return 0;
            }
            if (isTargetUpdate) {
                MarkerBase.CalculateBirth(this, target, targetPosition, bullet.Range, attachNull.position, bulletSet, out birthPosition);
                // キャストマーカー生成
                if (castMarkerData != null) {
                    EffectManager.CreateCastMarker(this, GUIObjectUI.LockonObject, targetPosition, bullet.Range, bulletSet, castMarkerData);
                }
            }
        }

        // 発生弾とそれ以外で処理を分ける
        switch (bullet.Type) {
        case SkillBulletMasterData.BulletType.Birth:
        case SkillBulletMasterData.BulletType.Nearest:
            this.BirthShot(birthPosition, target, bulletSet, bullet);
            break;
        default:
            this.DefaultShot(attachNull, target, bulletSet);
            break;
        }
    }
    void DefaultShot(Transform attachNull, ObjectBase target, SkillBulletSetMasterData bulletSet) {
        // 発射位置と角度補正
        Vector3 position = attachNull.position;
        Quaternion rotation = this.transform.rotation;

        // 上下角補正.
        if (target != null && bulletSet.Bullet != null) {
            float limitRotX = bulletSet.Bullet.VerticalAngle;
            float lookRotX = Quaternion.LookRotation(target.transform.position - this.transform.position).eulerAngles.x;
            lookRotX = Mathf.Clamp((lookRotX + 180f) % 360f - 180f, -limitRotX, +limitRotX);

            // 最初からrotationXが傾いている状況は考慮していない.
            Vector3 eulerRot = rotation.eulerAngles;
            eulerRot.x = lookRotX;
            rotation = Quaternion.Euler(eulerRot);
        }

        // 弾丸補正.
        GameGlobal.AddOffset(bulletSet, ref position, ref rotation, Vector3.one);

        // 弾丸生成
        this.CreateBullet(target, null, position, rotation, bulletSet.SkillID, bulletSet);
    }
    void BirthShot(Vector3 birthPosition, ObjectBase target, SkillBulletSetMasterData bulletSet, SkillBulletMasterData bullet) {
        Vector3 position = birthPosition;
        Quaternion rotation = this.transform.rotation;

        // 弾丸生成
        this.CreateBullet(target, null, position, rotation, bulletSet.SkillID, bulletSet);
    }
    protected override void CreateBulletStart(SkillBulletMasterData bullet) {
        this.CreateBulletStartBase(bullet);

        this.PersonSkillMotionParam.AddBackMoveFiber(bullet);
    }
    public IEnumerator BackMoveCoroutine(SkillBulletMasterData bullet) {
        float timer = bullet.BackTime;
        float timeLag = bullet.BackTimeLag;
        float direction = bullet.BackDirection;
        float speed = bullet.BackSpeed;
        float accel = bullet.BackAccel;

        // 反動開始ラグ
        while (timeLag > 0f) {
            timeLag -= Time.deltaTime;
            yield return 0;
        }

        // 反動方向を求める
        Vector3 forward;
        {
            Quaternion dir = Quaternion.Euler(0f, direction, 0f);
            Quaternion rotation = this.transform.rotation * dir;
            forward = rotation * Vector3.forward;
        }

        // 反動処理
        while (timer > 0f) {
            timer -= Time.deltaTime;

            // 反動
            Vector3 movement;
            this.CharacterMove.CalculateMoveImpulse(forward * speed, Time.deltaTime, out movement);
            base.MovePosition(movement);

            // 加速
            speed += accel * Time.deltaTime;
            speed = Mathf.Max(0f, speed);
            yield return 0;
        }
    }
    #endregion

    #region キャンセル、リンク、フィニッシュ、チャージスキル処理
    public Fiber SetCancelFiber(float acceptTimer, float pointTimer, float endTimer, ObjectBase target) {
        // チャージスキルの場合はボタンから離したら即発動のため
        // キャンセルと言う概念はそもそもない.
        if (this.SkillMotionParam.IsChargeSkill)
            return null;
        return new Fiber(SkillMotion_CancelCoroutine(acceptTimer, acceptTimer + pointTimer, acceptTimer + pointTimer + endTimer, target));
    }
    IEnumerator SkillMotion_CancelCoroutine(float acceptTimer, float pointTimer, float endTimer, ObjectBase target) {
        SkillMasterData skill = this.SkillMotionParam.Skill;
        float acceptTime = Time.time + acceptTimer;
        float pointTime = Time.time + pointTimer;
        float endTime = Time.time + endTimer;

        // １．受付開始まで待機
        // 受付開始まではキャンセル攻撃を受け付けない
        this.PersonSkillMotionParam.IsCancelAccept = false;
        while (Time.time < acceptTime) {
            yield return null;
        }

        // ２．受付開始
        // キャンセルポイントまでキャンセル攻撃を受け付ける
        this.PersonSkillMotionParam.IsCancelAccept = true;
        while (Time.time < pointTime) {
            yield return null;
        }

        // ３．キャンセルポイント
        // キャンセルポイント前にキャンセルしているかチェック
        if (this.IsCancelSkill(target)) {
            yield break;	// キャンセル成功の場合はこれ以降実行されない.
        }

        // キャンセルをしなかったので受付終了まで
        // 毎フレームキャンセルチェックをする
        while (Time.time < endTime) {
            if (this.IsCancelSkill(target)) {
                yield break;	// キャンセル成功の場合はこれ以降実行されない.
            }
            yield return null;
        }

        // キャンセル受付終了直前に全ボタンを強制チェック.
        GUISkill.ForceCheckPress();
        if (this.IsCancelSkill(target)) {
            yield break;	// キャンセル成功の場合はこれ以降実行されない.
        }

        // ４．受付終了
        // 受付中にキャンセルをしなかったら
        // 最後にフィニッシュスキルチェックをする
        if (this.SetFinishSkill(skill.FinishSkillID, target)) {
            yield break;	// フィニッシュスキル使用成功の場合はこれ以降実行されない.
        }

        // キャンセル攻撃を受け付けない
        this.PersonSkillMotionParam.IsCancelAccept = false;

        // クールタイムを設定する
        this.SetCoolTimer();
    }
    void SetCoolTimer() {
        this.PersonSkillMotionParam.SetCoolTime();
    }
    bool IsCancelSkill(ObjectBase target) {
        if (this.PersonSkillMotionParam.IsForceLink) {
            return this.SetLinkSkill(this.PersonSkillMotionParam.ForceLinkSkillID, target, false);
        } else if (this.PersonSkillMotionParam.IsCancel) {
            return this.SetCancelSkill();
        } else if (this.PersonSkillMotionParam.IsLink) {
            return this.SetLinkSkill(this.PersonSkillMotionParam.LinkSkillID, target, false);
        }

        return false;
    }
    bool SetCancelSkill() {
        var cancelSkill = this.PersonSkillMotionParam.CancelSkill;

        if (!cancelSkill.TryGetSkill(this))
            return false;

        if (!this.IsSkillUsable(cancelSkill.Skill))
            return false;

        //        DetectRay.Instance.Detect();
        CharacterCamera.Rota = true;
        ObjectBase target = this.GetSkillTarget(cancelSkill.Skill);

        // キャンセルスキル成功可否
        if (!this.PersonSkillMotionParam.SetCancelSkill(cancelSkill, target, false))
            return false;

        this.SkillMotion_Init();

        if (cancelSkill.Button.SkillButtonType == SkillButtonType.SpecialSkill) {
            // SPスキル演出表示
            GUISpSkillCutIn.PlayCutIn(this.AvatarType, this.SkinId, cancelSkill.Skill.DisplayName);
        } else {
            // スキル名表示
            GUIEffectMessage.SetSkill(cancelSkill.Skill.DisplayName);
        }

        return true;
    }
    bool SetLinkSkill(int linkSkillID, ObjectBase target, bool isInterrupt) {
        SkillMasterData skillData;
        if (!MasterData.TryGetSkill(linkSkillID, out skillData))
            return false;

        if (!this.IsSkillUsable(skillData))
            return false;

        //TODO Lee Change Target When Rota
        //        // 近接自動ターゲット
        //        if (skillData.IsNearTarget && target == null)
        //        {
        //            target = GameController.SearchNearTarget(this, skillData);
        //            if (target)
        //            {
        //                // ターゲットに向かせる
        //                this.SetLookAtTarget(target.transform);
        //            }
        //        }
        //        DetectRay.Instance.Detect();
        CharacterCamera.Rota = true;
        target = this.GetSkillTarget(skillData);

        // 攻撃パラメータを次の派生先スキルIDに変更する
        if (!this.PersonSkillMotionParam.SetLinkSkill(linkSkillID, target, isInterrupt))
            return false;

        this.SkillMotion_Init();

        if (this.PersonSkillMotionParam.UsingSkill.Button.SkillButtonType == SkillButtonType.SpecialSkill) {
            // SPスキル演出表示
            GUISpSkillCutIn.PlayCutIn(this.AvatarType, this.SkinId, skillData.DisplayName);
        } else if (skillData.DisplayNameAtLinkFlag) {
            // スキル名表示
            GUIEffectMessage.SetSkill(skillData.DisplayName);
        }

        return true;
    }
    bool SetFinishSkill(int finishSkillID, ObjectBase target) {
        if (finishSkillID == GameConstant.InvalidID) {
            return false;
        }

        this.PersonSkillMotionParam.Step = int.MaxValue;
        return this.SetLinkSkill(finishSkillID, target, false);
    }
    // TODO: require?
    /*
    protected override IEnumerator SkillMotion_MotionChargeCoroutine(float chargeTimer, int nextSkillID, int abortSkillID, ObjectBase target) {
        bool isChargeFinish = false;

        // ボタンを押している間.
        while (this.UISkillButton != null && this.UISkillButton.IsDown) {
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
            var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
            if (com != null && com.attackMotionResetConfig.isEnable) {
                chargeTimer = 0f;

                var skill = this.SkillMotionParam.Skill;
                if (skill != null) {
                    if (skill.SkillType == SkillType.Guard) {
                        yield return 0;
                        continue;
                    }
                    if (skill.ID == nextSkillID) {
                        isChargeFinish = false;
                        break;
                    }
                }
            }
#endif
            // チャージ時間
            if (chargeTimer > 0f) {
                // 時間カウント中
                chargeTimer -= Time.deltaTime;
                yield return 0;
            } else {
                // チャージ完了
                isChargeFinish = true;
                break;
            }
        }

        // リンク先設定
        bool isSuccess = false;
        if (isChargeFinish)
            isSuccess = this.SetLinkSkill(nextSkillID, target, true);
        else
            isSuccess = this.SetLinkSkill(abortSkillID, target, false);
        if (!isSuccess) {
            // 設定に失敗していたらチャージ終了
            // チャージパケットを送信する
            BattlePacket.SendSkillCharge(GameConstant.InvalidID, target, false);
            this.SkillMotion_SetDelay();
        }
    }*/
    #endregion

    #region 向き固定処理
    public IEnumerator SetFixedRotationFiber(float timer) {
        if (timer < 0f)
            return null;
        return SkillMotion_FixedRotationCoroutine(timer);
    }
    IEnumerator SkillMotion_FixedRotationCoroutine(float timer) {
        while (timer > 0f) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        this.IsFixedAttackRotation = true;
    }
    #endregion

    #region 移動処理
    public FiberSet SetMoveFiber(SkillMasterData skill) {
        FiberSet fiberSet = new FiberSet();
        List<SkillMoveSetMasterData> moveSetList;
        if (MasterData.TryGetMoveSet(skill.ID, out moveSetList)) {
            foreach (var moveSet in moveSetList) {
                fiberSet.AddFiber(this.SkillMotion_MoveCoroutine(moveSet));
            }
        }
        return fiberSet;
    }
    IEnumerator SkillMotion_MoveCoroutine(SkillMoveSetMasterData moveSet) {
        // 開始待ち
        yield return new WaitSeconds(moveSet.StartTiming);

        // 移動方向を求める
        Vector3 forward;
        {
            Quaternion dir = Quaternion.Euler(moveSet.RotateX, moveSet.RotateY, 0f);
            Quaternion rotation = this.transform.rotation * dir;
            forward = rotation * Vector3.forward;
        }

        // 移動処理
        float durationTime = moveSet.Time;
        float speed = moveSet.Speed;
        float accel = moveSet.Accel;
        while (durationTime > 0f) {
            durationTime -= Time.deltaTime;

            // 移動
            Vector3 movement;
            this.CharacterMove.CalculateMoveImpulse(forward * speed, Time.deltaTime, out movement);
            base.MovePosition(movement);
            // 加速
            speed += accel * Time.deltaTime;
            speed = Mathf.Max(0f, speed);
            yield return null;
        }
    }
    #endregion

    #region 重力処理
    public FiberSet SetGravityFiber(SkillMasterData skill) {
        FiberSet fiberSet = new FiberSet();
        List<SkillGravitySetMasterData> gravitySetList;
        if (MasterData.TryGetGravityrSet(skill.ID, out gravitySetList)) {
            foreach (var gravitySet in gravitySetList) {
                fiberSet.AddFiber(this.SkillMotion_GravityCoroutine(gravitySet));
            }
        }
        return fiberSet;
    }
    IEnumerator SkillMotion_GravityCoroutine(SkillGravitySetMasterData gravitySet) {
        // 開始待ち
        yield return new WaitSeconds(gravitySet.StartTiming);

        // 重力処理
        float durationTime = gravitySet.Time;
        float rate = gravitySet.Rate;
        while (durationTime > 0f) {
            durationTime -= Time.deltaTime;
            // 重力設定
            this.CharacterMove.GravityMag = rate;

            yield return null;
        }
    }
    #endregion

    /// <summary>
    /// 現在スキルの使用が可能か？.
    /// </summary>
    public bool IsSkillUsable(SkillMasterData skillData) {
        // ジャンプ中ならAerialFlagオンのスキル以外失敗.
        if ((this.State == Character.StateProc.UserJump || !this.CharacterMove.IsGrounded) && !skillData.AerialFlag) {
            return false;
        }
        return true;
    }

    ObjectBase GetSkillTarget(SkillMasterData skill) {
        // スキルがない
        if (skill == null)
            return null;

        // タイプ別
        ObjectBase target = null;
        switch (skill.Targettype) {
        case SkillMasterData.TargetType.None:
            // ターゲットしない
            target = null;
            break;
        case SkillMasterData.TargetType.Lockon:
            // ロックオン準拠
            target = GUIObjectUI.LockonObject;
            break;
        case SkillMasterData.TargetType.Self:
            // 自分自身
            target = this;
            break;
        }
        // ターゲットが指定されている
        if (target != null)
            return target;

        // 近接自動ターゲットフラグがオフ
        if (!skill.IsNearTarget)
            return null;

        // 近接自動ターゲットを取得する
        target = GameController.SearchNearTarget(this, skill);

        return target;
    }

    #endregion

    #endregion
}
