/// <summary>
/// キャラクター
/// 
/// 2012/12/03
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public abstract class Character : ObjectBase
{
	#region 宣言
	[System.Serializable]
	public enum StateProc
	{
		Move,
		SkillMotion,
		Dead,
		Down,
		Wake,
		Recoil,
		Jump,		// 空中スキル使えない.
		UserJump,	// 空中スキル使える.
		Grapple,
		Max,
	}
	#endregion

	#region フィールド＆プロパティ
	public AvatarType AvatarType { get { return (AvatarType)Id; } }

    /// <summary>
    /// Skin id
    /// </summary>
    public int SkinId { get; private set; }

    /// <summary>
    /// Star Id
    /// </summary>
    public int StarId { get; private set; }

	/// <summary>
	/// 走るスピード
	/// </summary>
	[SerializeField]
	private float runSpeed = 5.0f;
	public float RunSpeed { get { return runSpeed + BuffRunSpeed; } private set { runSpeed = value; } }
	private float runAnimDefaultSpeed = 5.0f;
	public float RunAnimDefaultSpeed { get { return runAnimDefaultSpeed; } }

    private Vector3 _position;
	public Vector3 Position {
        get {
            return _position;
        }
        protected set {
            _position = value;
        }
    }
	public Quaternion Rotation { get; protected set; }

	/// <summary>
	/// 無敵かどうか
	/// </summary>
	public override bool IsInvincible
	{
		get
		{
			return (this.StatusType == StatusType.Dead || 0f < this.InvincibleCounter);
		}
	}
	/// <summary>
	/// 無敵時間
	/// </summary>
	public float InvincibleCounter { get; protected set; }

	/// <summary>
	/// スキル使用時の合成移動モーションID.
	/// </summary>
	protected int moveMotionID = 0;
	/// <summary>
	/// スキル使用時の移動スピード倍率.
	/// </summary>
	protected float moveSpeedRatio = 1.0f;

	public CharacterMove CharacterMove { get; protected set; }
	public ScmAnimation ScmAnimation { get; protected set; }
	/// <summary>
	/// 強制的にスーパーアーマーかどうか
	/// </summary>
	public bool IsSuperArmor { get; protected set; }

	public abstract StateProc State { get; }

	public CharaMasterData CharaData { get; private set; }
	// 投げ.
	protected GrappleAttach grappleAttach;

	// ボイス.
	private CharacterVoice characterVoice;
	public CharacterVoice CharacterVoice
	{
		get
		{
			if(characterVoice == null)
			{
				characterVoice = CharacterVoice.Create(this);
			}
			return characterVoice;
		}
	}

	// EntrantInfo -> AvatarInfo のキャッシュ.
	private AvatarInfo avatarInfo;
	private AvatarInfo AvatarInfo
	{
		get
		{
			if(avatarInfo == null)
			{
				avatarInfo = this.EntrantInfo as AvatarInfo;
			}
			return avatarInfo;
		}
	}

	// キル数.
	public  int KillCount
	{
		get { return this.AvatarInfo != null ? this.AvatarInfo.KillCount : 0; }
		set
		{
			if(this.AvatarInfo != null)
			{
				this.AvatarInfo.KillCount = value;
			}
			// UI更新
			if (ObjectUIRoot != null) ObjectUIRoot.UpdateKill();
		}
	}

	// 戦闘中ののスコア順位
	public int ScoreRank
	{
		get{ return this.AvatarInfo != null ? this.AvatarInfo.ScoreRank : -1; }
		set
		{
			if(this.AvatarInfo != null)
			{
				this.AvatarInfo.ScoreRank = value;
			}
			// UI更新
			if(ObjectUIRoot != null) ObjectUIRoot.UpdateScoreRank();
		}
	}

	#endregion

	#region 初期化
	protected virtual void Awake()
	{
		this.CharacterMove = this.gameObject.GetSafeComponent<CharacterMove>();
		this.ScmAnimation = this.gameObject.GetSafeComponent<ScmAnimation>();
	}
	protected void SetupCharacter(Manager manager, EntrantInfo info)
	{
		base.SetupBase(manager, info);

		// キャラマスター設定
		{
			CharaMasterData chara = null;
			MasterData.TryGetChara(this.Id, out chara);
			this.CharaData = chara;
            this.SkinId = info.SkinId;
            this.StarId = info.StarId;
        }
		// キャラクター設定
		this.SetLevelParameter(this.Level, info.MaxHitPoint);

		this.LoadModelData(info);

		SendMessage("SetupMarker");

		this.Respawn(this.transform.localPosition, this.transform.localRotation);

		// Respawn処理でヒットポイントにMaxHitPointがセットされているのでEntrantInfoパケットのヒットポイントにセットし直す
		this.HitPoint = info.HitPoint;
	}

	public override void LoadModelCompleted(GameObject model, AnimationReference animationData)
	{
		this.ScmAnimation.Setup(model.GetComponent<Animation>());
		this.ScmAnimation.LoadAnimationAssetBundle(this, animationData);
		this.ScmAnimation.ResetAnimation();
		// 既に死んでいる場合.死亡モーションにする.
		if(this.StatusType == StatusType.Dead &&
		   MapManager.Instance.AreaType == AreaType.Field)
		{
			this.Motion(MotionState.dead_end);
		}
		base.LoadModelCompleted(model, animationData);
	}
	#endregion

	#region 破棄
	//protected override void Destroy() { base.Destroy(); }
	#endregion

	#region 更新
	protected virtual void Update()
	{
		this.ScmAnimation.UpdateCombinedAnimation();
		this.InvincibleCounter -= Time.deltaTime;
		if (this.InvincibleCounter < 0f)
			this.InvincibleCounter = 0f;
	}
	protected void LateUpdate()
	{
		// ダメージ表現.
		//UpdateDamageTexture();
	}
	#endregion

	#region 位置
	public void SetPosition(Vector3 position)			{ this.Position = position; this.transform.position = this.Position; }
	public void SetRotation(Quaternion rotation)		{ this.Rotation = rotation;	this.transform.rotation = this.Rotation; }
	public void SetNextPosition(Vector3 position)		{ this.NextPosition = position; }
	public void SetNextRotation(Quaternion rotation)	{ this.NextRotation = rotation; }

	public virtual void MovePosition(Vector3 movement, bool useCharacterMove = true)
	{
		if(useCharacterMove && this.CharacterMove != null)
		{
			this.CharacterMove.MovePosition(movement);
			this.Position = this.transform.position;
		}
		else
		{
			this.Position += movement;
			this.transform.position = this.Position;
        }
	}
	/// <summary>
	/// 回転角度更新
	/// Rotation から NextRotation に回転する
	/// </summary>
	protected void UpdateRotation()
	{
		this.SetRotation(this.NextRotation);
	}

	protected void ResetTransform(Vector3 position, Quaternion rotation)
	{
		// 位置
		this.SetPosition(position);
		this.SetNextPosition(position);
		// 回転
		this.SetRotation(rotation);
		this.SetNextRotation(rotation);
	}
	#endregion

	#region モーション
	protected virtual void ResetAnimation()
	{
		if (this.ScmAnimation)
		{
			this.ScmAnimation.UpdateMoveAnimation(MotionState.wait, 1f, (int)MotionLayer.Move, 0f, PlayMode.StopAll);
			this.ScmAnimation.UpdateAnimation(string.Empty, (int)MotionLayer.Action);
			this.ScmAnimation.UpdateCombinedAnimation();
		}
	}
	#endregion

	#region ObjectBase Override
	#region 移動パケット
	/// <summary>
	/// 移動アニメーションを設定する
	/// </summary>
	/// <param name="velocity">移動ベクトル</param>
	protected void UpdateMoveAnimation(Vector3 velocity, int moveAnimationID)
	{
		velocity.y = 0.0f;

		// 移動速度によってモーションを変更する
		float len = velocity.magnitude * this.moveSpeedRatio;
		float animationSpeedRatio = 1.0f;
		MotionState motionState = MotionState.wait;
		if (moveAnimationID == 0 || len <= GameConstant.MoveMinThreshold)
		{
			// 停止モーションに設定する
			motionState = MotionState.wait;
		}
		else
		{
			if(this.State == StateProc.SkillMotion)
			{
				// 走りモーションに設定する
				Vector3 forward = this.gameObject.transform.forward;
				float value_fb = forward.z * velocity.z + forward.x * velocity.x;
				float value_rl = forward.z * velocity.x - forward.x * velocity.z;
				if(Mathf.Abs(value_rl) < Mathf.Abs(value_fb))
				{
					if(value_fb < 0)
					{
						motionState = MotionState.run_001_b;
					}
					else
					{
						motionState = MotionState.run_001_f;
					}
				}
				else
				{
					if(value_rl < 0)
					{
						motionState = MotionState.run_001_l;
					}
					else
					{
						motionState = MotionState.run_001_r;
					}
				}
			}
			else
			{
				motionState = MotionState.run_001_f;
			}

			// 移動スピードの比率を算出.
			// アニメーション速度に使用する.
			if (this.RunAnimDefaultSpeed != 0.0f)
			{
				animationSpeedRatio = len / (this.RunAnimDefaultSpeed * Time.deltaTime);
			}
		}


		// 移動モーション更新
		this.ScmAnimation.UpdateMoveAnimation(motionState, animationSpeedRatio, (int)MotionLayer.Move);
	}
	#endregion

	#region スキルモーションパケット

	#region 宣言
	public abstract SkillMotionParam SkillMotionParam { get; }
	#endregion

	#region 初期化&終了
	protected void SkillMotionCharacter(Vector3 position, Quaternion rotation)
	{
		this.ResetTransform(position, rotation);
	}

	protected void SkillMotionFinishCharacter()
	{
		// スーパーアーマーを解除
		this.IsSuperArmor = false;
		// 移動パラメータを初期化
		this.moveMotionID = 0;
		this.moveSpeedRatio = 1.0f;
	}
	#endregion

	#region スキルモーション処理
	protected void SkillMotionProcCharacter()
	{
		switch (this.SkillMotionParam.Progress)
		{
			case SkillMotionParam.SkillMotionProg.Init:		this.SkillMotion_Init();	break;
			case SkillMotionParam.SkillMotionProg.Marker:	this.SkillMotion_Marker();	break;
			case SkillMotionParam.SkillMotionProg.Motion:	this.SkillMotion_Motion();	break;
			case SkillMotionParam.SkillMotionProg.Delay:	this.SkillMotion_Delay();	break;
			case SkillMotionParam.SkillMotionProg.End:		this.SkillMotion_End();		break;
		}

		// スキル使用で変化した移動スピードを反映するためこの順番に.
		this.SkillMoveProc();
	}
	/// <summary>
	/// スキル使用中移動処理
	/// </summary>
	protected abstract void SkillMoveProc();
	protected void SkillMotion_Init()					{ this.SkillMotion_SetMarker(); }
	protected virtual void SkillMotion_SetMarker()		{ this.SkillMotion_SetMarkerCharacter(); }
	protected void SkillMotion_SetMarkerCharacter()		{ this.SkillMotion_SetMotion(); }
	protected virtual void SkillMotion_Marker()			{}
	protected virtual void SkillMotion_SetMotion()		{ this.SkillMotion_SetMotionCharacter(); }
	protected void SkillMotion_SetMotionCharacter()
	{
		// アニメーション設定
		SkillMasterData skill = this.SkillMotionParam.Skill;
		if (skill != null)
		{
			// ボイス.
			if(skill.CharaVoice != null)
			{
				this.CharacterVoice.Play(skill.CharaVoice.QueueName);
			}

			// 慣性フラグ.
			if(!skill.InertialFlag)
			{
				this.CharacterMove.DirectionReset();
			}

			if (skill.Motion != null)
			{
				// アニメーション設定
				this.ScmAnimation.UpdateAnimation(skill.Motion.File, (int)MotionLayer.Action, 0f);
				// 途中再生
				if (this.SkillMotionParam.IsInterrupt)
				{
					AnimationState state = this.ScmAnimation.Animation[skill.Motion.File];
					if (state != null)
					{
						state.time = skill.InterruptTiming;
					}
				}
			}
		}

		this.SkillMotionParam.Progress = SkillMotionParam.SkillMotionProg.Motion;
	}
	protected void SkillMotion_Motion()
	{
		this.SkillMotionParam.UpdateMotionFiber();
	}
	protected void SkillMotion_SetDelay()
	{
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
		var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
		if (com != null && com.attackMotionResetConfig.isEnable)
		{
			this.SkillMotion_SetEnd();
			return;
		}
#endif
		if (!this.SkillMotionParam.IsDelay())
		{
			this.SkillMotion_SetEnd();
			return;
		}

		// モーション再生
		this.ScmAnimation.UpdateAnimation(MotionState.wait, (int)MotionLayer.Action);
		this.ScmAnimation.UpdateCombinedAnimation();
		// チャージスキルキャンセル
		// Locus エフェクトで判断しているためここでオフにして
		// エフェクトを消滅させる
		this.SkillMotionParam.IsChargeSkill = false;

		this.SkillMotionParam.Progress = SkillMotionParam.SkillMotionProg.Delay;
	}
	void SkillMotion_Delay()
	{
		this.SkillMotionParam.UpdateDelayFiber();
	}
	void SkillMotion_SetEnd()
	{
		this.SkillMotionParam.Progress = SkillMotionParam.SkillMotionProg.End;
	}
	void SkillMotion_End()
	{
		this.StateCheck();
	}
	#endregion

	#region タイミング処理
	public IEnumerator SetMotionFiber(SkillMasterData skill, SkillMotionMasterData motion, ObjectBase target)
	{
		// 軸となる処理を設定する
		// チャージスキルかどうか
		if (this.SkillMotionParam.IsChargeSkill)
			return this.SkillMotion_MotionChargeCoroutine(skill.ChargeTiming, skill.ChargeSkillId, skill.ChargeAbortSkillId, target);
		// 自分弾丸かどうか
		if (this.SkillMotionParam.IsSelfBullet)
			return this.SkillMotion_MotionSelfBulletCoroutine(motion, skill.ClampTiming);
		// それ以外ならモーション時間
		return this.SkillMotion_MotionCoroutine(motion.Length);
	}
	IEnumerator SkillMotion_MotionCoroutine(float motionTimer)
	{
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
		var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
		if (com != null && com.attackMotionResetConfig.isEnable)
		{
			motionTimer = 0f;
		}
#endif
		while (motionTimer > 0f)
		{
			motionTimer -= Time.deltaTime;
			yield return 0;
		}

		this.SkillMotion_SetDelay();
	}
	IEnumerator SkillMotion_MotionSelfBulletCoroutine(SkillMotionMasterData motion, float clampTimer)
	{
		float motionTimer = motion.Length;
		while(this.ScmAnimation.Animation == null)
		{
			yield return 0;
		}
		AnimationState state = this.ScmAnimation.Animation[motion.File];
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
		var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
		if (com != null && com.attackMotionResetConfig.isEnable)
		{
			motionTimer = 0f;
			clampTimer = 0f;
		}
#endif
		if (state == null)
		{
			Debug.LogWarning(string.Format("モーションが存在しない({0}:File={1})", this.UserName, motion.File));
		}
		else
		{
			state.speed = 1f;
		}
		while (clampTimer > 0f)
		{
			clampTimer -= Time.deltaTime;
			motionTimer -= Time.deltaTime;
			yield return 0;
		}

		// 自分弾丸が残っている場合はモーションを停止させる
		while (true)
		{
			if (this.SkillMotionParam.SelfBulletList == null)
				break;
			if (this.SkillMotionParam.SelfBulletList.Count <= 0)
				break;

			// 自分弾丸があるかどうか
			// (BulletSelf.cs 内で追加と削除をしている)
			if (state != null)
				state.speed = 0f;
			while (this.SkillMotionParam.SelfBulletList.Count > 0)
				yield return 0;
			break;
		}

		// モーション再生再開
		if (state != null)
			state.speed = 1f;
		while (motionTimer > 0f)
		{
			motionTimer -= Time.deltaTime;
			yield return 0;
		}

		this.SkillMotion_SetDelay();
	}
	protected virtual IEnumerator SkillMotion_MotionChargeCoroutine(float chargeTimer, int nextSkillID, int abortSkillID, ObjectBase target)
	{
		// チャージ時間
		while (chargeTimer > 0f)
		{
			chargeTimer -= Time.deltaTime;
			yield return 0;
		}

		this.SkillMotion_SetDelay();
	}
	public List<IEnumerator> SetMoveRatioFiber(SkillMotionMasterData motion)
	{
		List<IEnumerator> fiberList = new List<IEnumerator>();
		if (motion.MoveSetList != null)
		{
			foreach (var t in motion.MoveSetList)
			{
				IEnumerator fiber = this.SkillMotion_MoveRatioCoroutine(t.Timing, t);
				fiberList.Add(fiber);
			}
		}

		return fiberList;
	}
	IEnumerator SkillMotion_MoveRatioCoroutine(float timer, SkillMotionMoveSetMasterData moveSet)
	{
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}

		this.moveMotionID = moveSet.MoveID;
		this.moveSpeedRatio = moveSet.MoveSpeed * 0.01f;
	}
	public List<IEnumerator> SetSuperArmorFiber(SkillMasterData skill)
	{
		List<IEnumerator> fiberList = new List<IEnumerator>();
		if (skill.SuperArmorSetList != null)
		{
			foreach (var t in skill.SuperArmorSetList)
			{
				fiberList.Add(this.SkillMotion_SuperArmorCoroutine(t.StartTiming, t.Time, t));
			}
		}

		return fiberList;
	}
	IEnumerator SkillMotion_SuperArmorCoroutine(float startTimer, float timer, SkillSuperArmorSetMasterData superArmorSet)
	{
		// スーパーアーマー開始待機
		while (startTimer > 0f)
		{
			startTimer -= Time.deltaTime;
			yield return 0;
		}

		// 開始
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			this.IsSuperArmor = true;
			yield return 0;
		}

		// 終了
		this.IsSuperArmor = false;
	}
	public List<IEnumerator> SetEffectFiber(SkillMasterData skill)
	{
		List<IEnumerator> fiberList = new List<IEnumerator>();
		if (skill.MotionEffectSetList != null)
		{
			foreach (var t in skill.MotionEffectSetList)
			{
				fiberList.Add(this.SkillMotion_EffectCoroutine(t.Timig, t));
			}
		}

		return fiberList;
	}
	IEnumerator SkillMotion_EffectCoroutine(float timer, SkillMotionEffectSetMasterData effectSet)
	{
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}

		// 軌跡エフェクト
		EffectManager.CreateLocus(this, effectSet.Effect);
	}
	public IEnumerator SetDelayFiber(float timer)
	{
		if (0f >= timer)
			return null;
		return this.SkillMotion_DelayCoroutine(timer);
	}
	IEnumerator SkillMotion_DelayCoroutine(float timer)
	{
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}

		this.SkillMotion_SetEnd();
	}
	#endregion
	#endregion

	#region ヒットパケット
	public override void Hit(HitInfo hitInfo)
	{
		this.HitBase(hitInfo);

		if(hitInfo.inFieldAttackerId == NetworkController.ServerValue.InFieldId)
		{
			if(hitInfo.damage > 0)
			{
				// プレイヤーが攻撃者でダメージ値が入っている場合のみヒットメッセージを表示する
				GUIEffectMessage.SetHit();
			}
		}
	}
	#endregion

	#region 状態異常パケット
	public override void Status(HitInfo hitInfo)
	{
		base.StatusBase(hitInfo.statusType);

		// 状態異常処理
		switch(this.StatusType)
		{
		case StatusType.Normal:
			break;
		case StatusType.Dead:
			this.Dead(hitInfo);
			break;
		case StatusType.Invincible:
			break;
		}
	}
	public abstract void Dead(HitInfo hitInfo);
	#endregion

	#region 効果パケット
	public void Guard()
	{
		if( this.State == StateProc.Dead || 
			this.State == StateProc.Wake || 
			this.State == StateProc.Down || 
			this.State == StateProc.Recoil ||
			this.State == StateProc.Grapple){
			return;
		}
		// スキル中断なし(中断してしまうと連続ガードも不可能になるため).
		this.ScmAnimation.UpdateAnimation(MotionState.guard_damage, (int)MotionLayer.ReAction, 0f, PlayMode.StopSameLayer);
	}
	#endregion

	#region リスポーンパケット
	protected void RespawnCharacter(Vector3 position, Quaternion rotation)
	{
		base.RespawnBase(position, rotation);

		// 位置、回転
		ResetTransform(position, rotation);
		// アニメーション
		ResetAnimation();
	}
	#endregion

	#region ワープパケット
	protected void WarpCharacter(Vector3 position, Quaternion rotation)
	{
		base.WarpBase(position, rotation);

		// 位置、回転
		ResetTransform(position, rotation);
		// アニメーション
		ResetAnimation();
	}
	#endregion

	#region レベルアップパケット
	/// <summary>
	/// レベルアップ
	/// </summary>
	/// <param name="level"></param>
	public override void LevelUp(int level, int hitPoint, int maxHitPoint)
	{
		this.LevelUpCharacter(level, hitPoint, maxHitPoint);
	}
	/// <summary>
	/// レベルアップ.
	/// </summary>
	/// <param name='level'>
	/// 新しいレベル.
	/// </param>
	protected void LevelUpCharacter(int level, int hitPoint, int maxHp)
	{
		// レベルパラメータ設定
		this.SetLevelParameter(level, maxHp);
		// 現在レベルが下がることは考慮していない.
		this.CreateLevelUpEffect();

		base.LevelUpBase(level, hitPoint);
	}
	protected void SetLevelParameter(int level, int maxHitPoint)
	{
		CharaLevelMasterData charaLv = null;
		if (!MasterData.TryGetCharaLv(this.CharaData, level, out charaLv))
		{
			BugReportController.SaveLogFileWithOutStackTrace("Fail LevelUP : " + this.AvatarType + "," + level);
			return;
		}

		this.SetLevelParameterCharacter(charaLv, maxHitPoint);
	}
	protected void SetLevelParameterCharacter(CharaLevelMasterData charaLv, int maxHitPoint)
	{
		if (charaLv == null)
			{ return; }

        ApplyStar(charaLv, maxHitPoint);
	}
	/// <summary>
	/// レベルアップエフェクト作成
	/// </summary>
	void CreateLevelUpEffect()
	{
		EffectManager.CreateLevelUp(this, true);
	}

    private void ApplyStar(CharaLevelMasterData charaLv, int maxHitPoint) {

        // レベルごとの設定
        this.Level = charaLv.Level;
        this.MaxHitPoint = maxHitPoint;
        this.RunSpeed = charaLv.Speed;
        this.runAnimDefaultSpeed = charaLv.Speed;   // 移動アニメーション用の基準速度

        // Apply star id and level
        CharaStarMasterData masterData;
        if (CharaStarMaster.Instance.TryGetMasterData(StarId, out masterData)) {
            this.RunSpeed = charaLv.Speed * masterData.SPDBase;
        }
    }
    #endregion
    #endregion

    #region ステートチェック
    /// <summary>
    /// 現在のバフ状況などから次のステートを決める.
    /// </summary>
    abstract protected void StateCheck();
	#endregion

	#region 投げ.
	public void GrappleStart(GrappleAttach grappleAttach)
	{
		if(this.grappleAttach != null)
		{
			// 投げは後出し優先.
			// HIT受信からの投げ移行なので端末毎に順番が変わることは無いはず.
			// 先出し優先だと,直前の投げが終了しているかどうかが端末によりズレる.
			this.grappleAttach.Destroy();
		}
		this.grappleAttach = grappleAttach;

		if(grappleAttach.Caster == this)
		{
			this.PlayAnimationWithEffect(grappleAttach.GrappleData.OffenseSkillMotionID, MotionLayer.ReAction);
		}
		else if(grappleAttach.Target == this)
		{
			this.DeleteAllBullets();	// 投げられた場合は怯み同様弾丸消滅する.
			this.PlayAnimationWithEffect(grappleAttach.GrappleData.DefenseSkillMotionID, MotionLayer.ReAction);
		}
	}
	
	#endregion

	#region モーション&エフェクト再生.
	/// <summary>
	/// スキルモーションIDからモーションとエフェクトの再生を行う.
	/// </summary>
	protected void PlayAnimationWithEffect(int skillMotionID, MotionLayer layer = MotionLayer.Action)
	{
		SkillMotionMasterData motionData;
		if(SkillMotionMaster.Instance.TryGetMasterData(skillMotionID, out motionData))
		{
			// アニメーション再生
			this.ScmAnimation.UpdateAnimation(motionData.File, (int)layer, 0f);
			// 軌跡エフェクト
			List<SkillMotionEffectSetMasterData> effectList;
			if(MasterData.TryGetEffectSetList(motionData.ID, out effectList))
			{
				foreach (var t in effectList)
				{
					this.StartCoroutine(this.SkillMotion_EffectCoroutine(t.Timig, t));
				}
//				EffectManager.CreateLocus(this, effectList);
			}
		}
	}
	#endregion

	#region 弾丸消滅.
	protected void DeleteAllBullets()
	{
		// 弾丸消滅.
		if(BattleMain.Instance != null)
		{
			BattleMain.Instance.BulletMonitor.DestroyBullets(this);
		}

		// エフェクト中断処理.
		var compArray = this.GetComponentsInChildren(typeof(IInterrupt));
		foreach(var comp in compArray)
		{
			IInterrupt interrupt = comp as IInterrupt;
			if(interrupt != null)
			{
				interrupt.Interrupt();
			}
		}
	}
	#endregion

	#region ダメージ表現
	/* 仕様変更により現在使用していない.
	private SkinnedMeshRenderer smRenderer;
	private MaterialPropertyBlock damageProperty = new MaterialPropertyBlock();
	float hpRatio = -1;
	const string AbsPropertyName = "_Value";
	private void UpdateDamageTexture()
	{
		if(smRenderer == null)
		{
			smRenderer = getSkinnedMeshRenderer();
		}
		else
		{
			float ratio = (float)HitPoint / (float)MaxHitPoint;
			// SetPropertyBlockが微妙に重いためhpRatio更新時のみ実行.
			if(hpRatio != ratio)
			{
				hpRatio = ratio;
				damageProperty.AddFloat(AbsPropertyName, hpRatio);
				smRenderer.SetPropertyBlock(damageProperty);
			}
		}
	}
	private SkinnedMeshRenderer getSkinnedMeshRenderer()
	{
		foreach(Transform child in transform)
		{
			foreach(Transform grandChild in child)
			{
				// 二階層下のはず
				//if(grandChild.name.EndsWith("_body"))
				if(grandChild.name.Equals(MeshObjectName))
				{
					var renderer = grandChild.GetComponent<SkinnedMeshRenderer>();
					if(renderer != null)
					{
						return renderer;
					}
				}
			}
		}
		return null;
	}
	*/
	#endregion
}
