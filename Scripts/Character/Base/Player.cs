/// <summary>
/// プレイヤー
/// 
/// 2012/12/04
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class Player : Character
{
    #region PlayerStateAdapter
    /// <summary>
    /// PlayerStateからPlayerへアクセスするためのアダプタ.
    /// </summary>
    public class PlayerStateAdapter
    {
        readonly Player player;
        public Player Player { get { return player; } }

        public PlayerStateAdapter(Player player)
        {
            this.player = player;
        }

        public void CalculateMove(Vector3 moveVector, out Vector3 movement)
        {
            player.CalculateMove(moveVector, out movement);
        }
        public void SetPosition(Vector3 position)
        {
            player.Position = position;
        }
        public void ForceSendMovePacket()
        {
            player.ForceSendMovePacket();
        }
        public void SendMotion(MotionState motionstate)
        {
            player.SendMotion(motionstate);
        }
        public IEnumerator OrderAnimCoroutine(MotionState first, MotionState end)
        {
            return player.OrderAnimCoroutine(first, end);
        }
        public IEnumerator JumpProcAnimUp()
        {
            return player.JumpProcAnimUp();
        }
        public IEnumerator JumpProcAnimDown()
        {
            return player.JumpProcAnimDown();
        }
        public void SetFallProcCoroutine(MotionState first, MotionState end)
        {
            SetFallProcCoroutine((player.OrderAnimCoroutine(first, end)));
        }
        public void SetFallProcCoroutine(IEnumerator fallAnimFiber)
        {
            player.SetFallProcCoroutine(fallAnimFiber);
        }
        public void ResetAnimation()
        {
            player.ResetAnimation();
        }
        public void PlayActionAnimation(MotionState motion)
        {
            player.PlayActionAnimation(motion);
        }
        public void SetAbsoluteGuardCounter(float time)
        {
            player.SetAbsoluteGuardCounter(time);
        }

        // ボイス.
        public void PlayVoice(string cueName)
        {
            player.CharacterVoice.Play(cueName);
        }

        public void SetModelRotation(Quaternion localRotation)
        {
            player.SetModelRotation(localRotation);
        }
        // 応急処置.
        public void MoveProc()
        {
            player.StateMove_Update();
        }
        public void SkillMotionProc()
        {
            player.StateSkillMotion_Update();
        }
        public void StateSkillMotion_Finish()
        {
            player.StateSkillMotion_Finish();
        }
        //
    }
    #endregion

    #region フィールド＆プロパティ
    private PlayerState.PlayerState pState;
    private PlayerState.PlayerState PState
    {
        get
        {
            if (pState == null)
            {
                pState = new PlayerState.Move(this.StateAdapter);
            }
            return pState;
        }
        set
        {
            if (pState != null)
            {
                pState.Finish();
            }
            pState = value;
        }
    }
    public override StateProc State { get { return PState.StateProc; } }

#if UNITY_EDITOR && XW_DEBUG
    public string GetStateInfoStr()
    {
        if (this.PState != null)
        {
            return this.PState.GetStateInfoStr();
        }
        return "Error";
    }
#endif
    private PlayerStateAdapter stateAdapter;
    private PlayerStateAdapter StateAdapter
    {
        get
        {
            if (stateAdapter == null)
            {
                stateAdapter = new PlayerStateAdapter(this);
            }
            return stateAdapter;
        }
    }

    public void AddExp(int exp) { PlayerManager.Instance.AddExp(exp); }

    public int Money { get; private set; }
    public void AddMoney(int money) { this.Money += money; }

    private Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if (this.mainCamera == null)
            {
                this.mainCamera = Camera.main; ;
            }
            return this.mainCamera;
        }
    }

    const float MoveMinThresholdSqr = GameConstant.MoveMinThreshold * GameConstant.MoveMinThreshold;
    Vector3 OldSendPosition { get; set; }
    public float SendMoveNextTime { get; private set; }
    public float BlockSendMoveTime { get; private set; }

    public bool IsFixedAttackRotation { get; private set; }
    public List<IEnumerator> BulletKeepProcessFiberList { get; private set; }

    public GUISkillButton UISkillButton
    {
        get
        {
            return this.PlayerSkillMotionParam.UsingSkill != null ? this.PlayerSkillMotionParam.UsingSkill.Button : null;
        }
    }

    private float airTechDelayCounter = 0f;
    public GrappleAttach GrappleAttach { get { return this.grappleAttach; } }

    public PostureType Posture
    {
        get
        {
            switch (this.State)
            {
                case StateProc.SkillMotion:
                    if (this.SkillMotionParam.Skill != null && this.SkillMotionParam.Skill.SkillType == SkillType.Guard)
                    {
                        // スキルモーション中 & ガード使用中
                        return PostureType.Guard;
                    }
                    break;
                case StateProc.Down:
                    return PostureType.Down;
            }
            return PostureType.Normal;
        }
    }

    /// <summary>
    /// 絶対防御状態かどうか(旧無敵.無敵状態の上位版.当たった弾を無効化する)
    /// </summary>
    public bool IsAbsoluteGuard
    {
        get
        {
            switch (this.State)
            {
                case StateProc.Wake:
                    return true;
                case StateProc.Grapple:
                    if (this.grappleAttach != null && this.grappleAttach.Target == this)
                    {
                        return true;
                    }
                    break;
                default:
                    if (0f < this.AbsoluteGuardCounter)
                    { return true; }
                    break;
            }
            return false;
        }
    }
    /// <summary>
    /// 絶対防御時間
    /// </summary>
    public float AbsoluteGuardCounter { get; protected set; }

    /// <summary>
    /// モデルの描画を行うか否か.
    /// </summary>
    public override bool IsDrawEnable { get { return !IsDisappear; } }

    #region プロパティ変更時に実行するメソッド.
    protected override void OnSetAvatarType()
    {
        base.OnSetAvatarType();
        // UIセット
        GUIPlayerInfo.SetAvatarType(this.AvatarType, this.SkinId);
    }
    protected override void OnSetUserName()
    {
        base.OnSetUserName();
        // UIセット
        GUIBattlePlayerInfo.SetName(this.UserName);
        GUILobbyResident.SetPlayerName(this.UserName);
        GUILobbyResident.SetPlayerWin(this.Win);
        GUILobbyResident.SetPlayerLose(this.Lose);
        GUIPlayerInfo.SetName(this.UserName);
    }
    protected override void OnSetHP()
    {
        base.OnSetHP();
        // UI更新
        GUIBattlePlayerInfo.UpdateHP(base.HitPoint, base.MaxHitPoint);
    }
    protected override void OnSetLevel()
    {
        base.OnSetLevel();
        // UIセット
        GUIBattlePlayerInfo.SetLevel(this.Level);
        GUISkill.ChangeButtonName(this.AvatarType, this.Level);
    }
    #endregion
    #endregion

    #region セットアップ
    public static void Setup(GameObject go, Manager manager, PlayerInfo info)
    {
        // コンポーネント取得
        Player player = go.GetSafeComponent<Player>();
        if (player == null)
        {
            manager.Destroy(go);
            return;
        }

        // 初期設定
        player.Init();
        player.SetupCharacter(manager, info);

        // HPゲージの初期化
        GUIBattlePlayerInfo.SetupHP(info.HitPoint, info.MaxHitPoint);
        // バフアイコンの初期化
        GUIBattlePlayerInfo.BuffIconClear();

        // カメラをプレイヤーにアタッチする
        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                cc.SetCharacter(player);
            }
        }

        // トランスポータで移動のみを行った場合以外,リキャストリセット.
        if (info.ResetCoolTime)
        {
            player.ResetCoolTime();
        }

        player.SetupCompleted();

        // Change symbol after respawned
        if (null != GUILobbyResident.Instance) {
            ScmParam.Battle.CharaIcon.GetIcon(player.AvatarType, player.SkinId, false, GUILobbyResident.SetIcon);
        }
    }
    public void ResetCoolTime()
    {
        // 20150324現在.超必含めて全快.
        GUISkill.ResetCoolTime(this.AvatarType, this.Level);
    }
    protected override string GetObjectUIPath()
    {
        return GameGlobal.GetObjectUIPlayerPath();
    }
    public override void LoadModelCompleted(GameObject model, AnimationReference animationData)
    {
        base.LoadModelCompleted(model, animationData);
        CharacterCamera cc = GameController.CharacterCamera;
        if (cc)
        {
            cc.SetCharacter(this);
        }
    }
    #endregion

    #region 初期化
    protected override void Awake()
    {
        base.Awake();

        this.PlayerSkillMotionParam = new PlayerSkillMotionParam(this);
        this.PState = new PlayerState.Move(this.StateAdapter);
    }
    public void Init()
    {
        this.Money = 0;
    }
    #endregion

    #region 更新
    protected override void Update()
    {
        CheckFall();

        this.UpdateProc_State();
        base.Update();

        this.UpdateCounter();

        // 慣性移動.
        this.CharacterMove.MoveInertia(Time.deltaTime);
        this.Position = this.transform.position;
    }
    private void UpdateCounter()
    {
        this.airTechDelayCounter -= Time.deltaTime;
        this.AbsoluteGuardCounter -= Time.deltaTime;
    }
    #endregion

    #region UpdateProc
    /// <summary>
    /// 移動処理
    /// </summary>
    private void StateMove_Update()
    {
        this.InputMove();

        Vector3 velocity;
        this.UpdatePosition(1f, out velocity);
        this.UpdateRotation();
        this.UpdateMoveAnimation(velocity, 1);
        // 落下判定.
        if (!this.CharacterMove.IsGrounded)
        {
            this.Fall();
        }
    }
    private void StateSkillMotion_Update()
    {
        this.SkillMotionProcCharacter();

        SkillMasterData skill = this.SkillMotionParam.Skill;
        if (skill == null)
            return;
        // 回避スキルかどうか
        if (skill.SkillType == SkillType.Avoid)
            return;
        // 自分弾丸かどうか
        if (this.SkillMotionParam.IsSelfBullet)
            return;
        // エイミングマーカー中かどうか
        if (this.SkillMotionParam.Progress == SkillMotionParam.SkillMotionProg.Marker)
            return;
        // ロックオンしていない
        if (GUIObjectUI.NowLockonType == OUILockon.Type.None)
        {
            // 近接自動ターゲットフラグが立っている
            if (this.SkillMotionParam.Target != null && skill.IsNearTarget)
                return;
        }
        // 回転を固定するかどうか
//        if (this.IsFixedAttackRotation)
            return;

        // カメラ方向にキャラを向かせる
        // ガンナーの通常攻撃など弾丸を連写するときにカメラ方向に打たせるため
        this.SetCameraRotation();
    }
    private void UpdateProc_State()
    {
        if (!this.PState.Update())
        {
            this.StateCheck();
        }
        this.SendMovePacket();
    }
    #endregion
    #region FinishProc
    private void StateSkillMotion_Finish()
    {
        base.SkillMotionFinishCharacter();

        // クールタイム設定
        this.SetCoolTimer();
        // キャラ回転固定解除
        this.IsFixedAttackRotation = false;
    }
    #endregion

    #region AvaterModel.
    private void SetModelRotation(Quaternion localRotation)
    {
        if (AvaterModel.ModelTransform)
        {
            AvaterModel.ModelTransform.localRotation = localRotation;
        }
    }
    #endregion

    #region Animation.
    private void PlayActionAnimation(MotionState motionState)
    {
        this.ScmAnimation.UpdateAnimation(motionState, (int)MotionLayer.ReAction);
        this.SendMotion(motionState);
    }
    protected override void ResetAnimation()
    {
        base.ResetAnimation();
        this.SendMotion(MotionState.wait);
    }
    #endregion

    #region ObjectBase Override
    #region 移動
    /// <summary>
    /// スキル使用中移動処理
    /// </summary>
    protected override void SkillMoveProc()
    {
        this.InputMove();

        Vector3 velocity;
        this.UpdatePosition(this.moveSpeedRatio, out velocity);
        if (this.SkillMotionParam.Progress == SkillMotionParam.SkillMotionProg.Marker)
        {
            this.UpdateRotation();
            this.UpdateMoveAnimation(velocity, 1);
        }
        else
        {
            this.UpdateMoveAnimation(velocity, moveMotionID);
        }
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
        Vector3 moveDirection = this.NextPosition - this.Position;

        // 一回に進む距離を求める
        // 一回に進む距離が一定以上なら補正する
        // Y軸移動はそのまま使う
        float moveY = moveDirection.y;
        moveDirection.y = 0.0f;
        Vector3 movement;
        if (moveDirection.magnitude < this.RunSpeed * Time.deltaTime * speedRatio)
        {
            movement = moveDirection;
        }
        else
        {
            movement = (moveDirection.normalized * this.RunSpeed) * Time.deltaTime * speedRatio;
        }
        movement.y = moveY;

        // 移動
        this.MovePosition(movement);

        // 移動ベクトルを返す
        velocity = this.Position - prevPosition;
    }
    public override void MovePosition(Vector3 movement, bool useCharacterMove = true)
    {
        base.MovePosition(movement, useCharacterMove);

        // 移動送信
        this.SendMovePacket();
    }
    void SendMovePacket()
    {
        // パケット送信間隔がまだ来てない
        if (this.SendMoveNextTime >= Time.time)
        { return; }
        ForceSendMovePacket();
    }
    void ForceSendMovePacket()
    {
        this.SendMoveNextTime = Time.time + GameConstant.MovePacketInterval;

        if (this.BlockSendMoveTime < Time.time)
        {
            CommonPacket.SendMove(this.OldSendPosition, this.Position, this.Rotation);
            this.OldSendPosition = this.Position;
        }
    }
    public void SetBlockSendMoveTime(float blockTime)
    {
        this.BlockSendMoveTime = Time.time + blockTime;
    }

    /// <summary>
    /// 入力して移動する
    /// </summary>
    public Vector3 InputDirection;
    private void InputMove()
    {
        // ジョイパッドの方向入力
        InputDirection = this.GetMoveStickRotation();

        // キャラクターの移動
        Vector3 movement;
        CalculateMoveGround(InputDirection * (this.RunSpeed * this.moveSpeedRatio), out movement);

        // キャラクターの回転角度
        Vector3 rotation = new Vector3(movement.x, 0.0f, movement.z);
        if (rotation.magnitude > GameConstant.MoveMinThreshold)
        {
            this.SetNextRotation(Quaternion.LookRotation(rotation));
        }
    }

    private void StopCameraAutoFollow()
    {
        _isCameraAutoFollowing = false;
    }

    private void StartCameraAutoFollow()
    {
        if (IsCameraAutoFollowing())
        {
            return;
        }
        if (!_finalRotationExists)
        {
            return;
        }
        _cameraAutoFollowValue = 0;
        _isCameraAutoFollowing = true;
        CharacterCamera cc = GameController.CharacterCamera;
        if (cc)
        {
            _initialRotation = cc.LookAtRotation;
            _finalRotationExists = true;
        }
    }

    private void UpdateCameraAutoFollow()
    {
        if (!_isCameraAutoFollowing)
        {
            return;
        }
        if (GUIObjectUI.NowLockonType != OUILockon.Type.None)
        {
            StopCameraAutoFollow();
            return;
        }
        //_cameraAutoFollowValue = Mathf.Clamp01(_cameraAutoFollowValue + Time.deltaTime * _cameraAutoFollowSpeed);
        //_cameraAutoFollowValue = Mathf.Clamp01(_cameraAutoFollowValue + Time.deltaTime * _rotateSpeed);
        _cameraAutoFollowValue = Mathf.Clamp01(_cameraAutoFollowValue + Time.deltaTime *
            Mathf.Lerp(_rotateSpeed, _cameraAutoFollowSpeed, _cameraAutoFollowValue));
        CharacterCamera cc = GameController.CharacterCamera;
        if (cc)
        {
            Quaternion r2 = Quaternion.Lerp(_initialRotation, _finalRotation, _cameraAutoFollowValue);
            cc.LookAtRotation = r2;
        }
        if (_cameraAutoFollowValue >= 1.0f)
        {
            StopCameraAutoFollow();
        }
    }

    private bool IsCameraAutoFollowing()
    {
        return _isCameraAutoFollowing;
    }

    private float _rotateSpeed = 0.7f;
    private float _cameraAutoFollowSpeed = 1.5f;
    private Quaternion _initialRotation;
    private Quaternion _finalRotation;
    private float _cameraAutoFollowValue = 0.0f;
    private bool _isCameraAutoFollowing = false;
    private bool _finalRotationExists = false;
    private float ROTATION_THRESHOLD = 0.01f;

    private bool CanRotateNow(Quaternion r1, Quaternion r2)
    {
        float delta = Mathf.Abs(r1.eulerAngles.y - r2.eulerAngles.y);
        if (delta > 180)
        {
            delta = 360 - delta;
        }
        return delta < 120;
    }

    /// <summary>
    /// キャラクターの歩行移動.地面についていない時は慣性移動になる.
    /// </summary>
    void CalculateMoveGround(Vector3 moveVector, out Vector3 movement)
    {
        this.CharacterMove.CalculateMoveGround(
            moveVector,
            out movement);
        this.NextPosition = this.Position + movement;
    }
    /// <summary>
    /// キャラクターの移動.空中でも有効.
    /// </summary>
    void CalculateMove(Vector3 moveVector, out Vector3 movement)
    {
        this.CharacterMove.CalculateMove(
            moveVector,
            Time.deltaTime,
            out movement);
        this.NextPosition = this.Position + movement;
    }

    /// <summary>
    /// カメラから見た移動スティックの方向の回転角度を取得する
    /// </summary>
    Vector3 GetMoveStickRotation()
    {
        Vector3 inputDirection = InputPad();
        if (inputDirection != Vector3.zero)
        {
            // キー入力の遊びをなくす.
            inputDirection.Normalize();
            // キー入力をゲーム内の速度に変換
            Vector3 angle = this.MainCamera.transform.rotation.eulerAngles;
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, Quaternion.Euler(0.0f, angle.y, 0.0f), Vector3.one);
            inputDirection = matrix.MultiplyVector(inputDirection);
        }
        // Spバフ パニックの効果で入力を反転.
        if (this.IsPanic)
        {
            inputDirection = -inputDirection;
        }
        return inputDirection;
    }
    Vector3 InputPad()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // キー入力
        if (!GUIMoveStick.IsDrag)
        {
            Vector2 delta;
            if (GUIChat.IsInput)
            {
                // チャット入力中は移動不可.
                delta = Vector2.zero;
            }
            else
            {
                delta = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            if (0f < delta.sqrMagnitude)
            {
                const float keySpeed = 1000f;
                delta *= Time.deltaTime * keySpeed;
                GUIMoveStick.Drag(delta);
            }
            else
            {
                GUIMoveStick.Reset();
            }
        }
#endif
        Vector3 inputDirection = Vector3.zero;
        inputDirection.x = GUIMoveStick.Delta.x;
        inputDirection.z = GUIMoveStick.Delta.y;
        return inputDirection;
    }
    #endregion

    #region スキルモーションパケット

    #region 宣言
    public PlayerSkillMotionParam PlayerSkillMotionParam { get; protected set; }
    public override SkillMotionParam SkillMotionParam { get { return PlayerSkillMotionParam; } }
    #endregion

    #region スキルモーション
    /// <summary>
    /// スキルボタン押下時 使用するスキルをセットする.
    /// </summary>
    public bool TrySetSkill(GUISkillButton uiSkillButton)
    {
        // SPバフ 封印.
        if (IsSealSkillButton(uiSkillButton.SkillButtonType))
        {
            return false;
        }

        // State判定.
        if (this.PState.IsSkillUsable())
        {
            if (uiSkillButton.StoreSkill)//StoreSkill
            {
                return this.TryUseSkill(uiSkillButton);
            }
            else
            if (this.State == Character.StateProc.SkillMotion)
            {
                // キャンセルタイミングまで待って実行.
                return this.PlayerSkillMotionParam.SetCancelOrLink(uiSkillButton);
            }
            else
            {
                // 即時実行.
                return this.TryUseSkill(uiSkillButton);
            }
        }
        return false;
    }

    /// <summary>
    /// Lee add, for releasebutton in guiskillbutton.cs
    /// </summary>
    public SkillMasterData GetSkillData(GUISkillButton uiSkillButton)
    {
        int skillID = GameGlobal.GetSkillID(this.AvatarType, uiSkillButton.SkillButtonType, this.Level);

        SkillMasterData skillData;
        if (!MasterData.TryGetSkill(skillID, out skillData))
        {
            Debug.LogError("===> Can not get the skill data, skill id is " + skillID);
        }
        return skillData;
    }

    public static SkillMasterData GetSkillData(GUISkillButton uiSkillButton, AvatarType avatarType, int lv) {
        int skillID = GameGlobal.GetSkillID(avatarType, uiSkillButton.SkillButtonType, lv);

        SkillMasterData skillData;
        if (!MasterData.TryGetSkill(skillID, out skillData)) {
            Debug.LogError("===> Can not get the skill data, skill id is " + skillID);
        }
        return skillData;
    }

    public bool IsSealSkillButton(SkillButtonType skillButtonType)
    {
        if (this.IsSeal)
        {
            switch (skillButtonType)
            {
                case SkillButtonType.Skill1:
                case SkillButtonType.Skill2:
                case SkillButtonType.SpecialSkill:
                    return true;
            }
        }
        return false;
	}

    float GetRunSpeed(ObjectBase target) {
        if (target is Npc) {
            return (target as Npc).Velocity.magnitude;
        } else if (target is Character) {
            return (target as Character).RunSpeed;
        } else {
            return 1.0f;
        }
    }

    /// <summary>Calculates angle between 2 vectors. Angle increases counter-clockwise.</summary>
    /// <param name="p1">1st point.</param>
    /// <param name="p2">2nd point.</param>
    /// <param name="o">Starting position of vectors.</param>
    /// <returns>Angle between -PI and PI.</returns>
    public static float Angle360Between(Vector2 p1, Vector2 p2, Vector2 o = default(Vector2)) {
        Vector2 v1, v2;
        if (o == default(Vector2)) {
            v1 = p1.normalized;
            v2 = p2.normalized;
        } else {
            v1 = (p1 - o).normalized;
            v2 = (p2 - o).normalized;
        }
        float angle = Vector2.Angle(v1, v2) * Mathf.Deg2Rad;
        return Mathf.Sign(Vector3.Cross(v1, v2).z) < 0 ? -angle : angle;
    }

    /// <summary>
    /// Angle between a and b, in -PI and PI
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static float SignedAngleBetween(Vector2 a, Vector2 b) {
        // angle in [0,180]
        float angle = Vector2.Angle(a, b);
        float sign = Vector3.Cross(a, b).z < 0 ? -1 : 1;

        // angle in [-179,180]
        float signed_angle = angle * sign;
        return signed_angle * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Predicate rotation by current caster's position and target's position and skill's data
    /// </summary>
    /// <param name="position"></param>
    /// <param name="target"></param>
    /// <param name="currentRotation"></param>
    /// <param name="skillData"></param>
    /// <returns></returns>
    bool PredicateRotation(Vector3 position, ObjectBase target, Quaternion currentRotation, SkillMasterData skillData, out Quaternion rotation, out Vector3 targetPosition) {
        rotation = currentRotation;
        targetPosition = target != null ? target.transform.position : Vector3.zero;
        if (skillData.BulletSetList == null || skillData.BulletSetList.Count == 0) {
            return true;
        }
        SkillBulletMasterData bullet = skillData.BulletSetList[0].Bullet;
        if (bullet == null) {
            return true;
        }
        float bulletSpeed = bullet.Speed;
        if (bulletSpeed < 0.01f) {
            return true;
        }
        if (target == null || target.transform == null) {
            return true;
        }
        float runSpeed = GetRunSpeed(target);
        
        // The target position and rotation will be on the bullet's shot timing
        Vector3 targetSpeed = (target.NextPosition - target.transform.position).normalized * runSpeed;
        
        if (targetSpeed.magnitude < 0.05f) {
            return true;
        }
        targetPosition = target.transform.position + targetSpeed * skillData.BulletSetList[0].ShotTiming;
        Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x,
                                    Quaternion.LookRotation(targetPosition - position, Vector3.up).eulerAngles.y,
                                    currentRotation.eulerAngles.z);
        
        float d = Vector3.Distance(position, targetPosition);
        //  A: position   B: target position  C: assumed hit position
        //  alpha: angle ABC   theta: angle CAB
        //  t is the hit timing
        //  so: BC = bulletSpeed * t   AB = d    BC = targetSpeed * t
        //  According to sin algorithm, AC / sin(alpha) = BC / sin(theta)
        //  So: sin(theta) / sin(alpha) = targetSpeed / bulletSpeed
        float alpha = SignedAngleBetween(new Vector2(targetSpeed.x, targetSpeed.z),
                                    new Vector2(position.x - targetPosition.x, position.z - targetPosition.z));
        float delta = 0;
        //float sign = 1;

        float sign = 1;
        /*
        if (alpha < -Mathf.PI/2 || alpha >= Mathf.PI/2) {
            delta = Mathf.PI;
        }*/
        float sinTheta = targetSpeed.magnitude * Mathf.Sin(alpha) / bulletSpeed;
        // if abs(alpha) >= PI / 2, abs(theta) will be < PI / 2
        float theta = Mathf.Asin(sinTheta);
        //
        rotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y - sign * theta * Mathf.Rad2Deg + delta, targetRotation.eulerAngles.z);
        Matrix2x2 rot = new Matrix2x2(theta);
        Vector2 org = new Vector2(targetPosition.x, targetPosition.z) - new Vector2(position.x, position.z);
        Vector2 rotated = rot * org;
        targetPosition = position + new Vector3(rotated.x, 0, rotated.y);

        return false;
    }

    private bool TryUseSkill(GUISkillButton uiSkillButton)
    {
        int skillID = GameGlobal.GetSkillID(this.AvatarType, uiSkillButton.SkillButtonType, this.Level);

        SkillMasterData skillData;
        if (!MasterData.TryGetSkill(skillID, out skillData))
            return false;

        if (!this.IsSkillUsable(skillData))
            return false;

        //DetectRay.Instance.Detect();
        CharacterCamera.Rota = true;
        ObjectBase target = this.GetSkillTarget(skillData);
        Quaternion rotation;
        Vector3 targetPosition;
        bool isTracingTarget = PredicateRotation(this.transform.position, target, this.transform.rotation, skillData, out rotation, out targetPosition);
        Vector3? targetPos = null;
        if (!isTracingTarget) {
            targetPos = targetPosition;
        }
        // スキル発動
        if (this.PlayerSkillMotionParam.SetFirstSkill(uiSkillButton, target, isTracingTarget, targetPos))
        {
            this.SkillMotionSetup(uiSkillButton, target);
            
            base.SkillMotionCharacter(this.transform.position, rotation);
            //Todo Lee
            //打补丁，现在pstate被替换的时候可能是技能释放state,所以会删掉已经设置好的UsingSkill
            PlayerSkillMotionParam.UseSkillParam tUseSkillParam = this.PlayerSkillMotionParam.UsingSkill;
            this.PState = new PlayerState.SkillMotion(this.StateAdapter);
            if (tUseSkillParam.Button.SkillButtonType == SkillButtonType.SpecialSkill)
            {
                // SPスキル演出表示
                GUISpSkillCutIn.PlayCutIn(this.AvatarType, this.SkinId, skillData.DisplayName);
            }
            else
            {
                // スキル名表示
                GUIEffectMessage.SetSkill(skillData.DisplayName);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 現在スキルの使用が可能か？.
    /// </summary>
    public bool IsSkillUsable(SkillMasterData skillData)
    {
        // ジャンプ中ならAerialFlagオンのスキル以外失敗.
        if ((this.State == Character.StateProc.UserJump || !this.CharacterMove.IsGrounded) && !skillData.AerialFlag)
        {
            return false;
        }
        return true;
    }
    ObjectBase GetSkillTarget(SkillMasterData skill)
    {
        // スキルがない
        if (skill == null)
            return null;

        // タイプ別
        ObjectBase target = null;
        switch (skill.Targettype)
        {
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
    protected void SkillMotionSetup(GUISkillButton uiSkillButton, ObjectBase target)
    {
        // エイミングスキルかどうかでタッチフィルターをオンオフする
        {
            var com = uiSkillButton.GetComponent<GUIGlobalTouchFilter>();
            if (com != null)
            {
                if (this.SkillMotionParam.IsAimingSkill)
                    com.enabled = false;
                else
                    com.enabled = true;
            }
        }
    }
    void SetCoolTimer()
    {
        this.PlayerSkillMotionParam.SetCoolTime();
    }
    #endregion

    #region スキルモーション処理
    #region マーカー処理
    protected override void SkillMotion_SetMarker()
    {
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
        var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
        if (com != null && com.attackMotionResetConfig.isEnable)
        {
            this.SkillMotion_SetMotion();
            return;
        }
#endif

        bool isMarker = false;
        // エイミングスキルでボタンを押しっぱなしにしている
        SkillMasterData skill = this.SkillMotionParam.Skill;
        if (this.SkillMotionParam.IsAimingSkill &&
            this.UISkillButton != null && this.UISkillButton.IsDown &&
            skill != null && skill.BulletSetList != null)
        {
            // エイミングマーカーを出すタイプかどうか
            foreach (var bulletSet in skill.BulletSetList)
            {
                // エイミングマーカー設定があるかどうか
                SkillBulletMasterData bullet = bulletSet.Bullet;
                SkillAimingMarkerMasterData markerData;
                if (bullet != null && MasterData.TryGetAimingMarker(bullet.ID, out markerData))
                {
                    // エイミングマーカー作成
                    Vector3 targetPosition;
                    Quaternion rotation;
                    bool isTracingTarget = PredicateRotation(this.transform.position, GUIObjectUI.LockonObject, this.transform.rotation, skill, out rotation, out targetPosition);
                    Vector3? targetPos = null;
                    if (!isTracingTarget) {
                        targetPos = targetPosition;
                    }
                    EffectManager.CreateAimingMarker(this.UISkillButton, this, GUIObjectUI.LockonObject, targetPosition, bullet.Range, bulletSet, markerData);
                    isMarker = true;
                }
            }
        }

        if (isMarker)
        {
            // マーカー処理へ移行する
            // 移動パラメータを初期化
            this.moveMotionID = 0;
            this.moveSpeedRatio = 1.0f;
            // モーションを初期化する
            if (this.PlayerSkillMotionParam.FirstSkill != this.SkillMotionParam.Skill)
                this.ResetAnimation();
            // プロセスをマーカーに設定する
            this.SkillMotionParam.Progress = SkillMotionParam.SkillMotionProg.Marker;
        }
        else
        {
            this.SkillMotion_SetMotion();
        }
    }
    protected override void SkillMotion_Marker()
    {
        if (this.UISkillButton != null && this.UISkillButton.IsDown)
        {
            // ボタンを押している最中は
            // マーカーを出しっぱなしにする
            return;
        }

        // モーション設定
        this.SkillMotion_SetMotion();
    }
    #endregion

    #region モーション処理
    protected override void SkillMotion_SetMotion()
    {
        // キャラ回転固定解除
        this.IsFixedAttackRotation = false;

        // 回避スキルかどうか.
        if (this.SkillMotionParam.Skill.SkillType == SkillType.Avoid)
        {
            // 入力方向にキャラを向かせる
            // 入力してなければ何もしない
            this.SetInputRotation();

            // HACK: 無敵時間仮実装.
            // スキル中断時に無敵時間も終わらせる必要があるため,DB対応次第Sアーマーと同じ実装にする.
            this.InvincibleCounter = 0.5f;
        }
        else
        {
            if (this.SkillMotionParam.IsTracingTarget) {
                if (this.SkillMotionParam.Target != null) {
                    // ターゲットにキャラを向かせる
                    this.SetLookAtTarget(this.SkillMotionParam.Target.transform);
                } else {
                    if (OUILockon.Instance.NowLockonType != OUILockon.Type.None) {
                        // カメラの方向にキャラを向かせる
                        this.SetCameraRotation();
                    }
                }
            } else if (this.SkillMotionParam.TargetPosition.HasValue) {
                this.SetLookAtTarget(this.SkillMotionParam.TargetPosition.Value);
            }
        }

        // 弾丸発射中断フラグでコルーチン制御を変える
        foreach (var fiber in this.BulletKeepProcessFiberList)
        {
            this.StartCoroutine(fiber);
        }
        this.BulletKeepProcessFiberList.Clear();

        // スキルモーションパケット送信
        SkillMasterData skill = this.SkillMotionParam.Skill;
        if (skill != null)
        {
            BattlePacket.SendSkillMotion(skill.ID, this.transform.position, this.transform.rotation);
        }
        else
        {
            string elog = "skill missing " + this.AvatarType + ":" + this.UISkillButton.SkillButtonType;
            Debug.LogWarning(elog);
            BugReportController.SaveLogFile(elog);
        }
        this.SkillMotion_SetMotionCharacter();
    }
    #endregion
    #endregion

    #region タイミング処理
    #region 弾丸発射処理
    public List<IEnumerator> SetBulletFiber(SkillMasterData skill, ObjectBase target, Vector3? targetPosition)
    {
        List<IEnumerator> fiberList = new List<IEnumerator>();
        this.BulletKeepProcessFiberList = new List<IEnumerator>();
        if (skill.BulletSetList != null)
        {
            foreach (var t in skill.BulletSetList)
            {
                SkillBulletMasterData bullet = t.Bullet;
                if (bullet == null)
                    continue;
                IEnumerator fiber = this.SkillMotion_BulletCoroutine(t.ShotTiming, t.TargetPositionTming, target, targetPosition, t, bullet);
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
                var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
                if (com != null && com.attackMotionResetConfig.isEnable)
                {
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
    IEnumerator SkillMotion_BulletCoroutine(float shotTimer, float targetPositionTimer, ObjectBase target, Vector3? targetPosition, SkillBulletSetMasterData bulletSet, SkillBulletMasterData bullet)
    {
        // ターゲット変更
        // エイミングマーカー時にターゲットが変更されているかどうか
        if (this.SkillMotionParam.IsAimingSkill)
        {
            target = GUIObjectUI.LockonObject;
        }
        // 近接自動ターゲットフラグ時は強制変更
        if (bullet.Type == SkillBulletMasterData.BulletType.Nearest)
            target = GameController.SearchGrappleTarget(this, GameConstant.BulletNearestSearchRange);

        // 発射ヌル取得
        Transform attachNull = this.transform;
        {
            string name = bulletSet.Bullet.AttachNull;
            if (!string.IsNullOrEmpty(name))
            {
                attachNull = this.transform.Search(name);
                if (attachNull == null)
                {
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
            while (shotTimer > 0f)
            {
                // 発射タイミング
                shotTimer -= Time.deltaTime;

                // ターゲット位置更新
                if (targetPositionTimer <= 0f)
                {
                    if (isTargetUpdate)
                    {
                        MarkerBase.CalculateBirth(this, target, targetPosition, bullet.Range, attachNull.position, bulletSet, out birthPosition);
                        isTargetUpdate = false;
                        // キャストマーカー生成
                        if (castMarkerData != null)
                        {
                            EffectManager.CreateCastMarker(this, GUIObjectUI.LockonObject, targetPosition, bullet.Range, bulletSet, castMarkerData);
                        }
                    }
                }
                else
                {
                    targetPositionTimer -= Time.deltaTime;
                }

                yield return 0;
            }
            if (isTargetUpdate)
            {
                MarkerBase.CalculateBirth(this, target, targetPosition, bullet.Range, attachNull.position, bulletSet, out birthPosition);
                // キャストマーカー生成
                if (castMarkerData != null)
                {
                    EffectManager.CreateCastMarker(this, GUIObjectUI.LockonObject, targetPosition, bullet.Range, bulletSet, castMarkerData);
                }
            }
        }

        // 発生弾とそれ以外で処理を分ける
        switch (bullet.Type)
        {
            case SkillBulletMasterData.BulletType.Birth:
            case SkillBulletMasterData.BulletType.Nearest:
                this.BirthShot(birthPosition, target, targetPosition, bulletSet, bullet);
                break;
            default:
                this.DefaultShot(attachNull, target, targetPosition, bulletSet);
                break;
        }
    }
    void DefaultShot(Transform attachNull, ObjectBase target, Vector3? targetPosition, SkillBulletSetMasterData bulletSet)
    {
        // 発射位置と角度補正
        Vector3 position = attachNull.position;
        Quaternion rotation = this.transform.rotation;

        // 上下角補正.
        if (target != null && bulletSet.Bullet != null)
        {
            float limitRotX = bulletSet.Bullet.VerticalAngle;
            float lookRotX = Quaternion.LookRotation((targetPosition.HasValue ? targetPosition.Value : target.transform.position) - this.transform.position).eulerAngles.x;
            lookRotX = Mathf.Clamp((lookRotX + 180f) % 360f - 180f, -limitRotX, +limitRotX);

            // 最初からrotationXが傾いている状況は考慮していない.
            Vector3 eulerRot = rotation.eulerAngles;
            eulerRot.x = lookRotX;
            rotation = Quaternion.Euler(eulerRot);
        }

        // 弾丸補正.
        GameGlobal.AddOffset(bulletSet, ref position, ref rotation, Vector3.one);
        // 弾丸生成
        this.CreateBullet(target, targetPosition, position, rotation, bulletSet.SkillID, bulletSet);
    }
    void BirthShot(Vector3 birthPosition, ObjectBase target, Vector3? targetPosition, SkillBulletSetMasterData bulletSet, SkillBulletMasterData bullet)
    {
        Vector3 position = birthPosition;
        Quaternion rotation = this.transform.rotation;

        // 弾丸生成
        this.CreateBullet(target, targetPosition, position, rotation, bulletSet.SkillID, bulletSet);
    }
    protected override void CreateBulletStart(SkillBulletMasterData bullet)
    {
        this.CreateBulletStartBase(bullet);

        this.PlayerSkillMotionParam.AddBackMoveFiber(bullet);
    }
    public IEnumerator BackMoveCoroutine(SkillBulletMasterData bullet)
    {
        float timer = bullet.BackTime;
        float timeLag = bullet.BackTimeLag;
        float direction = bullet.BackDirection;
        float speed = bullet.BackSpeed;
        float accel = bullet.BackAccel;

        // 反動開始ラグ
        while (timeLag > 0f)
        {
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
        while (timer > 0f)
        {
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
    public Fiber SetCancelFiber(float acceptTimer, float pointTimer, float endTimer, ObjectBase target)
    {
        // チャージスキルの場合はボタンから離したら即発動のため
        // キャンセルと言う概念はそもそもない.
        if (this.SkillMotionParam.IsChargeSkill)
            return null;
        return new Fiber(SkillMotion_CancelCoroutine(acceptTimer, acceptTimer + pointTimer, acceptTimer + pointTimer + endTimer, target));
    }
    IEnumerator SkillMotion_CancelCoroutine(float acceptTimer, float pointTimer, float endTimer, ObjectBase target)
    {
        SkillMasterData skill = this.SkillMotionParam.Skill;
        float acceptTime = Time.time + acceptTimer;
        float pointTime = Time.time + pointTimer;
        float endTime = Time.time + endTimer;

        // １．受付開始まで待機
        // 受付開始まではキャンセル攻撃を受け付けない
        this.PlayerSkillMotionParam.IsCancelAccept = false;
        while (Time.time < acceptTime)
        {
            yield return null;
        }

        // ２．受付開始
        // キャンセルポイントまでキャンセル攻撃を受け付ける
        this.PlayerSkillMotionParam.IsCancelAccept = true;
        while (Time.time < pointTime)
        {
            yield return null;
        }

        // ３．キャンセルポイント
        // キャンセルポイント前にキャンセルしているかチェック
        if (this.IsCancelSkill(target))
        {
            yield break;	// キャンセル成功の場合はこれ以降実行されない.
        }

        // キャンセルをしなかったので受付終了まで
        // 毎フレームキャンセルチェックをする
        while (Time.time < endTime)
        {
            if (this.IsCancelSkill(target))
            {
                yield break;	// キャンセル成功の場合はこれ以降実行されない.
            }
            yield return null;
        }

        // キャンセル受付終了直前に全ボタンを強制チェック.
        GUISkill.ForceCheckPress();
        if (this.IsCancelSkill(target))
        {
            yield break;	// キャンセル成功の場合はこれ以降実行されない.
        }

        // ４．受付終了
        // 受付中にキャンセルをしなかったら
        // 最後にフィニッシュスキルチェックをする
        if (this.SetFinishSkill(skill.FinishSkillID, target))
        {
            yield break;	// フィニッシュスキル使用成功の場合はこれ以降実行されない.
        }

        // キャンセル攻撃を受け付けない
        this.PlayerSkillMotionParam.IsCancelAccept = false;

        // クールタイムを設定する
        this.SetCoolTimer();
    }
    bool IsCancelSkill(ObjectBase target)
    {
        if (this.PlayerSkillMotionParam.IsForceLink)
        {
            return this.SetLinkSkill(this.PlayerSkillMotionParam.ForceLinkSkillID, target, false);
        }
        else if (this.PlayerSkillMotionParam.IsCancel)
        {
            return this.SetCancelSkill();
        }
        else if (this.PlayerSkillMotionParam.IsLink)
        {
            return this.SetLinkSkill(this.PlayerSkillMotionParam.LinkSkillID, target, false);
        }

        return false;
    }
    bool SetCancelSkill()
    {
        var cancelSkill = this.PlayerSkillMotionParam.CancelSkill;

        if (!cancelSkill.TryGetSkill(this))
            return false;

        if (!this.IsSkillUsable(cancelSkill.Skill))
            return false;

//        DetectRay.Instance.Detect();
        CharacterCamera.Rota = true;
        ObjectBase target = this.GetSkillTarget(cancelSkill.Skill);

        // キャンセルスキル成功可否
        if (!this.PlayerSkillMotionParam.SetCancelSkill(cancelSkill, target, false))
            return false;

        this.SkillMotion_Init();

        if (cancelSkill.Button.SkillButtonType == SkillButtonType.SpecialSkill)
        {
            // SPスキル演出表示
            GUISpSkillCutIn.PlayCutIn(this.AvatarType, this.SkinId, cancelSkill.Skill.DisplayName);
        }
        else
        {
            // スキル名表示
            GUIEffectMessage.SetSkill(cancelSkill.Skill.DisplayName);
        }

        return true;
    }
    bool SetLinkSkill(int linkSkillID, ObjectBase target, bool isInterrupt)
    {
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
        if (!this.PlayerSkillMotionParam.SetLinkSkill(linkSkillID, target, isInterrupt))
            return false;

        this.SkillMotion_Init();

        if (this.PlayerSkillMotionParam.UsingSkill.Button.SkillButtonType == SkillButtonType.SpecialSkill)
        {
            // SPスキル演出表示
            GUISpSkillCutIn.PlayCutIn(this.AvatarType, this.SkinId, skillData.DisplayName);
        }
        else if (skillData.DisplayNameAtLinkFlag)
        {
            // スキル名表示
            GUIEffectMessage.SetSkill(skillData.DisplayName);
        }

        return true;
    }
    bool SetFinishSkill(int finishSkillID, ObjectBase target)
    {
        if (finishSkillID == GameConstant.InvalidID)
        {
            return false;
        }

        this.PlayerSkillMotionParam.Step = int.MaxValue;
        return this.SetLinkSkill(finishSkillID, target, false);
    }
    protected override IEnumerator SkillMotion_MotionChargeCoroutine(float chargeTimer, int nextSkillID, int abortSkillID, ObjectBase target)
    {
        bool isChargeFinish = false;

        // ボタンを押している間.
        while (this.UISkillButton != null && this.UISkillButton.IsDown)
        {
#if XW_DEBUG && (UNITY_EDITOR || UNITY_STANDALONE)
            var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
            if (com != null && com.attackMotionResetConfig.isEnable)
            {
                chargeTimer = 0f;

                var skill = this.SkillMotionParam.Skill;
                if (skill != null)
                {
                    if (skill.SkillType == SkillType.Guard)
                    {
                        yield return 0;
                        continue;
                    }
                    if (skill.ID == nextSkillID)
                    {
                        isChargeFinish = false;
                        break;
                    }
                }
            }
#endif
            // チャージ時間
            if (chargeTimer > 0f)
            {
                // 時間カウント中
                chargeTimer -= Time.deltaTime;
                yield return 0;
            }
            else
            {
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
        if (!isSuccess)
        {
            // 設定に失敗していたらチャージ終了
            // チャージパケットを送信する
            BattlePacket.SendSkillCharge(GameConstant.InvalidID, target, false);
            this.SkillMotion_SetDelay();
        }
    }
    #endregion

    #region 向き固定処理
    public IEnumerator SetFixedRotationFiber(float timer)
    {
        if (timer < 0f)
            return null;
        return SkillMotion_FixedRotationCoroutine(timer);
    }
    IEnumerator SkillMotion_FixedRotationCoroutine(float timer)
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }
        this.IsFixedAttackRotation = true;
    }
    #endregion

    #region 移動処理
    public FiberSet SetMoveFiber(SkillMasterData skill)
    {
        FiberSet fiberSet = new FiberSet();
        List<SkillMoveSetMasterData> moveSetList;
        if (MasterData.TryGetMoveSet(skill.ID, out moveSetList))
        {
            foreach (var moveSet in moveSetList)
            {
                fiberSet.AddFiber(this.SkillMotion_MoveCoroutine(moveSet));
            }
        }
        return fiberSet;
    }
    IEnumerator SkillMotion_MoveCoroutine(SkillMoveSetMasterData moveSet)
    {
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
        while (durationTime > 0f)
        {
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
    public FiberSet SetGravityFiber(SkillMasterData skill)
    {
        FiberSet fiberSet = new FiberSet();
        List<SkillGravitySetMasterData> gravitySetList;
        if (MasterData.TryGetGravityrSet(skill.ID, out gravitySetList))
        {
            foreach (var gravitySet in gravitySetList)
            {
                fiberSet.AddFiber(this.SkillMotion_GravityCoroutine(gravitySet));
            }
        }
        return fiberSet;
    }
    IEnumerator SkillMotion_GravityCoroutine(SkillGravitySetMasterData gravitySet)
    {
        // 開始待ち
        yield return new WaitSeconds(gravitySet.StartTiming);

        // 重力処理
        float durationTime = gravitySet.Time;
        float rate = gravitySet.Rate;
        while (durationTime > 0f)
        {
            durationTime -= Time.deltaTime;
            // 重力設定
            this.CharacterMove.GravityMag = rate;

            yield return null;
        }
    }
    #endregion
    #endregion
    #endregion

    #region 攻撃パケット
    public override void Attack(int bulletSetID, ObjectBase target, Vector3 position, Quaternion rotation, Vector3? casterPos, Quaternion casterRot)
    {
    }
    /// <summary>
    /// 弾丸タイプ「弾丸」生成
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="bulletData"></param>
    protected override void CreateBulletAmmo(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
    {
        // 攻撃パケット送信
        BattlePacket.SendAttack(target, position, rotation, skillID, bulletSet);
        // 弾丸生成
        this.CreateBulletAmmoBase(target, targetPosition, position, rotation, skillID, bulletSet);
    }
    /// <summary>
    /// 弾丸タイプ「発生」生成
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="bulletData"></param>
    protected override void CreateBulletBirth(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
    {
        // 攻撃パケット送信
        BattlePacket.SendAttack(target, position, rotation, skillID, bulletSet);
        // 弾丸生成
        this.CreateBulletBirthBase(target, targetPosition, position, rotation, skillID, bulletSet);
    }
    /// <summary>
    /// 弾丸タイプ「自分弾丸」生成
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="bulletData"></param>
    protected override void CreateBulletSelf(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
    {
        // 攻撃パケット送信
        BattlePacket.SendAttack(target, position, rotation, skillID, bulletSet, this);
        // 弾丸生成
        this.CreateBulletSelfBase(target, targetPosition, position, rotation, skillID, bulletSet);
    }
    /// <summary>
    /// 弾丸タイプ「壁沿い弾丸」生成
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="bulletData"></param>
    protected override void CreateBulletAlong(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
    {
        // 攻撃パケット送信
        BattlePacket.SendAttack(target, position, rotation, skillID, bulletSet);
        // 弾丸生成
        this.CreateBulletAlongBase(target, targetPosition, position, rotation, skillID, bulletSet);
    }
    #endregion

    #region ヒットパケット
    public override void Hit(HitInfo hitInfo)
    {
        this.HitBase(hitInfo);

        // ヒット画面エフェクト
        if (0 < hitInfo.damage)
        {
            if (hitInfo.inFieldAttackerId != this.InFieldId)
            {
                // HPゲージ揺れ演出.
                GUIBattlePlayerInfo.ShakeHpBar();
            }
        }
        if (hitInfo.statusType == StatusType.Dead)
        {
            // リスポーンUI設定
            //GUIRespawnInfo.SetRespawnTime(GameConstant.RespawnTime, GameConstant.RespawnTime);
            GUIRespawnInfo.SetRespawnTime(hitInfo.respawnTime, hitInfo.respawnTime);
        }
    }
    #endregion

    #region 状態異常パケット＆効果パケット
    // 吹き飛びスピード.
    const float BlowSpeedS = 0f;
    const float BlowSpeedM = 2f;
    const float BlowSpeedL = 3.75f;
    public override void Effect(HitInfo hitInfo)
    {
        this.EffectBase(hitInfo.effectType);

        // スーパーアーマー中,投げ＆投げられ中は怯み系無効.
        if (this.IsSuperArmor ||
            this.State == StateProc.Grapple)
        {
            return;
        }

        // 効果処理
        switch (this.EffectType)
        {
            case EffectType.None:
                break;
            case EffectType.Guard:
                this.Guard();
                break;
            case EffectType.SuperArmor:
                break;
            case EffectType.Down:
                this.Down(hitInfo.blownAngleType, hitInfo.bulletDirection);
                break;
            case EffectType.FalterMicro:
                break;
            case EffectType.FalterSmall:
                this.Falter(MotionState.damage_s, BlowSpeedS, hitInfo.bulletDirection);
                break;
            case EffectType.FalterMedium:
                this.Falter(MotionState.damage_m, BlowSpeedM, hitInfo.bulletDirection);
                break;
            case EffectType.FalterLarge:
                this.Falter(MotionState.damage_l, BlowSpeedL, hitInfo.bulletDirection);
                break;
        }
    }
    public void Falter(MotionState motion, float blowSpeed, float bulletDirection)
    {
        if (this.PState.CanFalter())
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, bulletDirection + 180, 0));
            this.SetNextRotation(this.transform.rotation);

            this.PState = new PlayerState.Falter(this.StateAdapter, this.transform.forward * -1, blowSpeed, motion);
        }
    }

    public void Bind()
    {
        if (this.PState.CanBind())
        {
            if (!CharacterMove.IsGrounded)
            {
                // 空中.
                this.Down(0, this.gameObject.transform.rotation.eulerAngles.y + 180f);
            }
            else
            {
                // 処理設定
                this.PState = new PlayerState.Bind(this.StateAdapter, MotionState.dizzy);
            }
        }
    }

    public override void Dead(HitInfo hitInfo)
    {
        // ユニークモーション取得.
        bool isUniqueMotion = false;
        if (hitInfo.inFieldAttackerId == this.InFieldId)
        {
            string motionName;
            if (UniqueMotion.TryGetDeadSelfMotionName(hitInfo.bulletID, out motionName))
            {
                this.SetUniqueDeadCoroutine(motionName);
                isUniqueMotion = true;
            }
        }
        if (!isUniqueMotion)
        {
            // ユニークモーションではない.
            this.SetDownBlownCoroutine(hitInfo.blownAngleType, hitInfo.bulletDirection, true);
        }

        // カメラを揺らす
        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                cc.Shake();
            }
        }
        // ロックオン解除
        GUIObjectUI.NowLockonType = OUILockon.Type.None;
    }
    public void Down(int blownAngleType, float bulletDirection)
    {
        if (this.State == StateProc.Wake)
        {
            return;
        }
        // 処理設定
        if (this.State == StateProc.Dead)
        {
            // Deadの場合,Stateはそのままで吹き飛びなおす(投げの場合とか).
            this.SetDownBlownCoroutine(blownAngleType, bulletDirection, true);
        }
        else
        {
            this.SetDownBlownCoroutine(blownAngleType, bulletDirection, false);
        }
        // カメラを揺らす
        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                cc.Shake();
            }
        }
    }
    public void Wake()
    {
        // 処理設定
        IEnumerator wakeMotionFiber = this.OrderAnimCoroutine(MotionState.wake);
        this.PState = new PlayerState.Wake(this.StateAdapter, wakeMotionFiber);
    }
    private void SetAbsoluteGuardCounter(float time)
    {
        this.AbsoluteGuardCounter = time;
    }
    #endregion

    #region リスポーンパケット
    public override void Respawn(Vector3 position, Quaternion rotation)
    {
        base.RespawnCharacter(position, rotation);
        this.Fall();

        // 慣性リセット.
        if (CharacterMove)
        {
            CharacterMove.DirectionReset();
        }

        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                // カメラを追従モードにする
                cc.IsFollow = true;
                // カメラを振り向かせる
                cc.IsCharaForward = true;
            }
        }
        // 落下タイム初期化.
        sendFallTime = 0;
        // 出撃ボイス.
        if (MapManager.Instance.AreaType == AreaType.Field)
        {
            this.CharacterVoice.Play(CharacterVoice.CueName_start);
        }
    }
    #endregion

    #region ワープパケット
    public override void Warp(Vector3 position, Quaternion rotation)
    {
        base.WarpCharacter(position, rotation);
        this.PState = new PlayerState.Move(this.StateAdapter);

        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                // カメラを振り向かせる
                cc.IsCharaForward = true;
            }
        }
        this.StateCheck();
    }
    #endregion

    #region ジャンプパケット
    public override void Jump(Vector3 position, Quaternion rotation)
    {
        if (this.PState.CanJump())
        {
            // ジャンプ移動量.
            Vector3 vec = position - this.Position;

            // キャラを目的地に向ける.
            Quaternion rot = Quaternion.LookRotation(new Vector3(vec.x, 0, vec.z));
            ResetTransform(this.transform.position, rot);

            // State変更、コルーチン始動.
            this.PState = new PlayerState.ForceJump(this.StateAdapter, position, rot, vec);
        }
    }

    public override void Wire(Vector3 position, Quaternion rotation)
    {
        this.Wire(position, PlayerState.WireDash.WireSpeed);
    }
    public void Wire(Vector3 targetPos, float wireSpeed)
    {
        if (this.PState.CanWire())
        {
            // 目標地点へのベクトル.
            Vector3 vec = targetPos - this.transform.position;

            // 距離が離れていれば発動.
            if (PlayerState.WireDash.WireApproachLength * PlayerState.WireDash.WireApproachLength < vec.sqrMagnitude)
            {
                // キャラを目的地に向ける.
                Quaternion rot = Quaternion.LookRotation(new Vector3(vec.x, 0, vec.z));
                ResetTransform(this.transform.position, rot);

                // State変更、コルーチン始動.
                this.PState = new PlayerState.WireDash(this.StateAdapter, targetPos, rot, vec, wireSpeed);
            }
        }
    }

    // 吸い寄せられ.
    public override void Captured(Vector3 position, Quaternion rotation)
    {
        if (this.PState.CanFalter())
        {
            // スーパーアーマーは吸われない.
            if (this.IsSuperArmor)
            { return; }

            // 目標地点計算.
            Vector3 targetPos = position - (position - this.Position).normalized * 2;
            // ワイヤー移動量.
            Vector3 vec = targetPos - this.Position;

            // State変更
            this.PState = new PlayerState.Captured(this.StateAdapter, vec);

            // キャラを目的地の逆に向ける.
            Quaternion rot = Quaternion.LookRotation(new Vector3(-vec.x, 0, -vec.z));
            ResetTransform(this.transform.position, rot);
        }
    }

    void Fall()
    {
        this.SetFallProcCoroutine(JumpProcAnimDown());
    }
    void SetFallProcCoroutine(IEnumerator fallAnimFiber)
    {
        // State変更、コルーチン始動.
        this.PState = new PlayerState.Fall(this.StateAdapter, fallAnimFiber);
        this.CharacterMove.IsGrounded = false;
        this.PState.Update();
    }
    #endregion

    #region レベルアップパケット
    /// <summary>
    /// レベルアップ
    /// </summary>
    /// <param name="level"></param>
    public override void LevelUp(int level, int hitPoint, int maxHitPoint)
    {
        // レベルアップ時の経験値セット
        PlayerManager.Instance.SetLevelUpParameter(this.AvatarType, this.Level, level);

        base.LevelUpCharacter(level, hitPoint, maxHitPoint);

        // [LevelUP]UI演出エフェクト表示
        GUIEffectMessage.SetLevelUp();
    }

    #endregion

    #region モーションパケット
    private void SendMotion(MotionState motionstate)
    {
        switch (motionstate)
        {
            // 送信のみ.
            case MotionState.wait:	// モーションリセット.
            case MotionState.down_end:
            case MotionState.dead_end:
            case MotionState.wake:
            case MotionState.jump_dw_sta:
            case MotionState.jump_end:
            case MotionState.maneuver_up_sta:
            case MotionState.maneuver_dw_sta:
            case MotionState.maneuver_f_up_sta:
            case MotionState.maneuver_f_dw_sta:
                BattlePacket.SendMotion(motionstate);
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
                BattlePacket.SendMotion(motionstate);
                this.DeleteAllBullets();
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

    /// <summary>
    /// モーションを順に再生する.
    /// </summary>
    IEnumerator OrderAnimCoroutine(params MotionState[] motionStates)
    {
        foreach (MotionState motionState in motionStates)
        {
            this.SendMotion(motionState);
            this.ScmAnimation.UpdateAnimation(motionState, (int)MotionLayer.ReAction);
            float motionTime = Time.time + this.ScmAnimation.GetAnimationLength(motionState.ToString());
            while (Time.time < motionTime) { yield return null; }
        }
    }
    /// <summary>
    /// 上昇アニメーション制御.
    /// </summary>
    IEnumerator JumpProcAnimUp()
    {
        return this.OrderAnimCoroutine(MotionState.jump_up_sta, MotionState.jump_up_lp);
    }
    /// <summary>
    /// 下降アニメーション制御.
    /// </summary>
    IEnumerator JumpProcAnimDown()
    {
        return this.OrderAnimCoroutine(MotionState.jump_dw_sta, MotionState.jump_dw_lp);
    }
    /// <summary>
    /// ユニークモーションファイバー(現在は自爆のみ使用).
    /// </summary>
    private IEnumerator UniqueMotionFiber(params string[] motionNames)
    {
        List<MotionState> motionStateList = new List<MotionState>();
        foreach (string motionName in motionNames)
        {
            MotionState motionState;
            try
            {
                // HACK : 本来はEnum.TryParseを使うべきだが.Net4からなので無理.
                motionState = (MotionState)System.Enum.Parse(typeof(MotionState), motionName);
                motionStateList.Add(motionState);
            }
            catch (System.Exception e)
            {
                BugReportController.SaveLogFileWithOutStackTrace(motionName + e.ToString());
            }
        }
        return this.OrderAnimCoroutine(motionStateList.ToArray());
    }
    #endregion

    #region 経験値処理
    protected override Vector3 ExpEffectOffsetMin { get { return new Vector3(0.0f, 1.0f, 0.0f); } }
    protected override Vector3 ExpEffectOffsetMax { get { return new Vector3(0.0f, 1.0f, 0.0f); } }
    #endregion

    #region バフ処理
    public override void ChangeBuff(BuffType[] newBuffTypes, BuffPacketParameter[] buffPacket, BuffPacketParameter[] spBuffPacket, float speed)
    {
        bool oldParalysis = this.IsParalysis;

        base.ChangeBuff(newBuffTypes, buffPacket, spBuffPacket, speed);

        if (newBuffTypes != null)
        {
            // バフ.SPバフがかかった順に並び替えて取得する
            LinkedList<BuffInfo> buffInfoList = BuffInfo.GetBuffInfoList(newBuffTypes, buffPacket, spBuffPacket);

            // プレイヤー側の状態効果アイコン表示.
            GUIBattlePlayerInfo.SetBuffIcon(buffInfoList);

            // 効果説明表示
            GUIStateMessage.SetStateMessage(buffInfoList);
        }

        // 麻痺判定.
        if (this.IsParalysis && !oldParalysis)
        {
            // ボイス.
            this.CharacterVoice.Play(CharacterVoice.CueName_damage_l);
            Bind();
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
        this.PState = new PlayerState.Move(this.StateAdapter);

        if (this.IsParalysis)
        {
            this.Bind();
        }
    }
    #endregion

    #region 吹き飛び処理
    private void SetUniqueDeadCoroutine(string motionName)
    {
        this.PState = new PlayerState.Dead(this.StateAdapter, UniqueMotionFiber(motionName));
    }
    private void SetDownBlownCoroutine(int blownAngleType, float bulletDirection, bool isDead)
    {
        // 吹き飛び方向.
        Vector3 blownVec = Vector3.zero;
        SkillBlowPatternMasterData blowData;
        if (MasterData.TryGetSkillBlowPattern(blownAngleType, out blowData))
        {
            blownVec = new Vector3(blowData.VectorX, blowData.VectorY, blowData.VectorZ);
            this.airTechDelayCounter = blowData.AirtechTiming;
        }
        else
        {
            this.airTechDelayCounter = -1;
        }
        bool isSpin = false;
        if (this.airTechDelayCounter < 0)
        {
            this.airTechDelayCounter = float.MaxValue;
            isSpin = true;
        }

        // Down or Dead.
        if (isDead)
        {
            this.CharacterVoice.Play(CharacterVoice.CueName_dead);
            this.PState = new PlayerState.Dead(this.StateAdapter, blownVec, bulletDirection, MotionState.dead_sta, MotionState.dead_mid, MotionState.dead_end);
        }
        else if (isSpin)
        {
            this.CharacterVoice.Play(CharacterVoice.CueName_down);
            this.PState = new PlayerState.Down(this.StateAdapter, blownVec, bulletDirection, MotionState.down_sta_spin, MotionState.down_mid_spin, MotionState.down_end);
        }
        else
        {
            this.CharacterVoice.Play(CharacterVoice.CueName_down);
            this.PState = new PlayerState.Down(this.StateAdapter, blownVec, bulletDirection, MotionState.down_sta, MotionState.down_mid, MotionState.down_end);
        }
    }
    #endregion
    #endregion

    #region 落下判定
    // 落下時のKillSelf連続送信防止用(boolだと送信ミス時に困るので一応Timeにしてあります).
    const float KillSelfHeight = -5f;	// KillSelfを送信するY座標.
    const float ResendTime = 5f;		// KillSelf再送信までの時間(通信エラー対策).
    float sendFallTime = 0f;
    private void CheckFall()
    {
        if (this.Position.y > KillSelfHeight)
        { return; }
        if (sendFallTime > Time.time)
        { return; }

        {
            CharacterCamera cc = GameController.CharacterCamera;
            if (cc)
            {
                // カメラの追従を切る
                cc.IsFollow = false;
            }
        }

        if (this.State != Character.StateProc.Dead)
        {
            // 復帰場所選択中は送らない
            if (!(GUIMapWindow.Mode == GUIMapWindow.MapMode.Respawn || GUIDeckEdit.NowMode != GUIDeckEdit.DeckMode.None))
            {
                // KillSelf送信
                BattlePacket.SendKillSelf();
            }

            // 復帰or一定時間経過まで送信を控える.
            sendFallTime = Time.time + ResendTime;
        }
    }
    #endregion

    #region 回転処理
    public void SetLookAtTarget(Transform target)
    {
        if (null == target)
        { return; }
        Vector3 r = new Vector3(target.position.x, 0f, target.position.z) - new Vector3(this.Position.x, 0f, this.Position.z);
        if (r == Vector3.zero)
        { return; }
        Quaternion rotation = Quaternion.LookRotation(r.normalized);
        this.SetRotation(rotation);
        this.SetNextRotation(rotation);
    }

    public void SetLookAtTarget(Vector3 targetPosition) {
        Vector3 r = new Vector3(targetPosition.x, 0f, targetPosition.z) - new Vector3(this.Position.x, 0f, this.Position.z);
        if (r == Vector3.zero) { return; }
        Quaternion rotation = Quaternion.LookRotation(r.normalized);
        this.SetRotation(rotation);
        this.SetNextRotation(rotation);
    }
    public void SetInputRotation()
    {
#if XW_DEBUG || VIEWER
        if (GetIsFixedAttackRotation())
            return;
#endif
        Vector3 inputDirection = this.GetMoveStickRotation();
        Quaternion rotation;
        if (inputDirection != Vector3.zero)
        {
            // 入力されていたら入力方向
            rotation = Quaternion.LookRotation(inputDirection);
        }
        else
        {
            // 入力されていないならキャラの向いている方向
            rotation = this.transform.rotation;
        }
        this.SetRotation(rotation);
        this.SetNextRotation(rotation);
    }
    public void SetCameraRotation()
    {
#if XW_DEBUG || VIEWER
        if (GetIsFixedAttackRotation())
            return;
#endif
        Quaternion rotation = Quaternion.Euler(this.Rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, this.Rotation.eulerAngles.z);
        this.SetRotation(rotation);
        this.SetNextRotation(rotation);
    }

#if XW_DEBUG || VIEWER
    /// <summary>
    /// デバッグモードとビュアーモードの状態を見てスキルの打つ方向のフラグを取得する
    /// ビュアーシーン以外はデバッグパラメータの情報を見て取得する
    /// ビュアーシーンでゲームモード時はデバッグパラメータの情報を見て取得する
    /// ビュアーシーンでビュアーモード時はtureを返す(キャラの向いている方向へスキルを打つ)
    /// コンパイルシンボルにXW_DEBUGがないかつVIEWERがある場合は
    /// ビュアーシーンでゲームモード時はfalseを返す それ以外のシーンはfalseを返す
    /// </summary>
    private bool GetIsFixedAttackRotation()
    {
#if VIEWER
		// コンパイルシンボルにVIEWERがある場合はビュアーシーン時のビュアーモードかどうかの判定を行う
		if(ViewerMain.Instance != null)
		{
			// 現在のシーンはビュアーシーン
			if(GUIViewer.GetIsViewerMode())
			{
				// ビュアーモード時はキャラの向いている方向へスキルを打つ
				return true;
			}
		}
#endif
        bool isFixedAttackRotation = false;
#if XW_DEBUG
        // コンパイルシンボルにデバッグがある場合
        // ビュアーシーン時でゲームモードとそれ以外のシーンではデバッグパラメータの方を参照する
        isFixedAttackRotation = DebugKeyCommand.Instance == null ? ScmParam.Debug.File.IsFixedAttackRotation : DebugKeyCommand.FixedAttackRotation;
#endif
        return isFixedAttackRotation;
    }
#endif
    #endregion

    #region 受け身
    /// <summary>
    /// 空中受身を取る.
    /// </summary>
    public bool AirTech()
    {
        const float TechVecY = 25f;
        const float TechVecXZ = 5f;

        if (this.State == Character.StateProc.Down &&
            !this.IsParalysis &&
            this.airTechDelayCounter < 0f)
        {
            // 受け身.
            this.UserJump(this.transform.forward * TechVecXZ + Vector3.up * TechVecY);
            // エフェクト.
            Transform parent = this.AvaterModel.RootTransform;
            if (parent == null)
            {
                parent = this.AvaterModel.ModelTransform;
                if (parent == null)
                {
                    parent = this.transform;
                }
            }
            EffectManager.Create(GameConstant.EffectBreakFall, parent);
            // 無敵時間設定.
            this.InvincibleCounter = GameConstant.AirTechInvincibleTimer;
            return true;
        }
        return false;
    }
    /// <summary>
    /// ユーザ入力によるジャンプ(ベクトル指定、壁貫通なし).
    /// </summary>
    protected void UserJump(Vector3 vector)
    {
        // キャラを目的地に向ける.
        Quaternion rot = Quaternion.LookRotation(new Vector3(vector.x, 0, vector.z));
        ResetTransform(this.transform.position, rot);

        // State変更、コルーチン始動.
        this.PState = new PlayerState.UserJump(this.StateAdapter, vector);
    }
    #endregion

    #region 投げ.
    public override void Grapple(GrappleAttach grappleAttach)
    {
        if (this.State == StateProc.Dead)
        {
            return;
        }

        float grappleDelay = 0f;
        // キャスターが自分.
        if (grappleAttach.Caster == this)
        {
            grappleDelay = grappleAttach.GrappleData.DelayTime;
        }
        this.PState = new PlayerState.Grapple(this.StateAdapter, grappleDelay);
        base.GrappleStart(grappleAttach);
    }
    public override void GrappleFinish(GrappleAttach grappleAttach)
    {
        if (grappleAttach.Target == this)
        {
            // ターゲットが自分.
            if (grappleAttach.IsFinish)
            {
                // 正常終了. 吹き飛ぶ.
                this.Down(grappleAttach.GrappleData.SkillBlowPatternID, grappleAttach.HitInfo.bulletDirection);
            }
            else
            {
                // 中断. その場ダウン.
                this.Down(0, grappleAttach.HitInfo.bulletDirection);
            }
        }
    }
    #endregion

    #region 立体起動.
    public void ManeuverGear3D(Skill3dManeuverGearMasterData maneuverGear3d, BulletBase bullet, bool byHit)
    {
        const float MinWireLength = 2f;
        const float MinHeight = 2f;

        switch (this.State)
        {
            case StateProc.Dead:
            case StateProc.Down:
            case StateProc.Wake:
            case StateProc.Recoil:
            case StateProc.Jump:
            case StateProc.Grapple:
                return;
        }

        if (bullet == null)
        {
            return;
        }

        Vector3 anchorPos = bullet.transform.position;
        if (maneuverGear3d.PendulumFlag)
        {
            if (!byHit)
            {
                float wireLength = anchorPos.y - this.transform.position.y - MinHeight;
                if (MinWireLength < wireLength)
                {
                    this.PState = new PlayerState.ManeuverGear3D(this.StateAdapter, anchorPos, maneuverGear3d.GravityScale, wireLength);
                }
            }
        }
        else
        {
            if (byHit)
            {
                this.Wire(anchorPos, maneuverGear3d.StraightSpeed);
            }
        }
    }

    #endregion

    #region トランスポーター
    public bool IsTransportable()
    {
        if (this.State == StateProc.Move)
        {
            // 歩くだけなら起動可能に変更.
            return true;// this.Position == this.OldPosition;
        }
        return false;
    }
    #endregion

    #region エラー処理.
    public override void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
    {
        // 自キャラのMoveパケットが飛んでくるのはエラー.
        BugReportController.SaveLogFile("Receive Player Move Packet.");
    }
    public override bool SkillMotion(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
    {
        // 自キャラのSkillMotionパケットが飛んでくるのはエラー.
        BugReportController.SaveLogFile("Receive Player SkillMotion Packet.");
        return false;
    }
    #endregion

    public void GaugeIncreased(int gaugeValue) {
        this.CreateExpEffectBase(this, gaugeValue);
    }
}
