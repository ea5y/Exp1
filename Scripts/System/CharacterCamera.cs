/// <summary>
/// キャラクターカメラ
/// 
/// 2013/02/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class CharacterCamera : MonoBehaviour
{
    #region 宣言
    /// <summary>
    /// 各種パラメータ
    /// </summary>
    [System.Serializable]
    public class Param
    {
        public float speed;
        public float maxSpeed;

        public bool isLimit;
        public float minLimit;
        public float maxLimit;

        public Param(float speed, float maxSpeed)
        {
            this.speed = speed;
            this.maxSpeed = maxSpeed;
        }
        public Param(float speed, float maxSpeed, bool isLimit, float minLimit, float maxLimit)
        {
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.isLimit = isLimit;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }
    /// <summary>
    /// 事前設定
    /// </summary>
    [System.Serializable]
    public class Configuration
    {
        public Param zoom = new Param(0.5f, 100f, true, -15f, -1.5f);
        public Param rotationX = new Param(1.5f, 300f, true, -15f, 15f);
        public Param rotationY = new Param(1.0f, 200f);
        public Param rotationZ = new Param(0.5f, 100f);
        public float worldLimitPositionY = 0.1f;
    }
    #endregion

    #region フィールド＆プロパティ
    [SerializeField]
    private Configuration config = new Configuration();
    public Configuration Config { get { return config; } private set { config = value; } }

    [SerializeField]
    private float zoom = -3f;
    public float Zoom { get { return zoom; } private set { zoom = value; } }
    [SerializeField]
    //private Vector3 rotation = new Vector3();
    private Vector3 rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value;
            //Debug.Log("Set rotation:" + _rotation);
        }
    }
    private Vector3 _rotation;
    public Vector3 Rotation { get { return rotation; } set { rotation = value; } }
    // Note: this field is not used now.
    [SerializeField]
    private string lookAtBoneName = "name_plate";
    public string LookAtBoneName { get { return lookAtBoneName; } private set { lookAtBoneName = value; } }
    [SerializeField]
    private Vector3 lookAt = new Vector3();
    public Vector3 LookAt { get { return lookAt; } private set { lookAt = value; } }
    [SerializeField]
    private Quaternion lookAtRotation = new Quaternion();
    public Quaternion LookAtRotation { get { return lookAtRotation; } set { lookAtRotation = value; } }

    [SerializeField]
    private Character character;
    public Character Character { get { return character; } private set { character = value; } }
    public bool IsCharaForward { get; set; }
    // カメラの位置をlookAtPointに追従するならtrue.
    public bool IsFollow { get; set; }

    private Transform lookAtPoint;

    private static bool _LockRotationForSkill;
    private static Vector3 _OldLockRotation;
    public static bool LockRotationForSkill
    {
        get { return _LockRotationForSkill; }
        set
        {
            if (value)
            {
                _OldLockRotation = GameController.CharacterCamera.Rotation;
            }
            else
            {
                GameController.CharacterCamera.Rotation = new Vector3(_OldLockRotation.x, GameController.CharacterCamera.Rotation.y, GameController.CharacterCamera.Rotation.z);
            }
            _LockRotationForSkill = value;
        }
    }
    #endregion

    #region キャラクター設定
    void Awake()
    {
        if (this.Character = null)
        {
            var p = GameController.GetPlayer();
            if (p != null)
            {
                this.SetCharacter(p);
            }
        }
    }
    public void SetCharacter(Character character)
    {
        this.Character = character;
        SetLookAtPoint();
        // キャラクタの向きにカメラを設定する
        this.IsCharaForward = true;
        // カメラを追従モードにする
        this.IsFollow = true;
    }
    /// <summary>
    /// Character内のlookAtBoneNameに指定されたポイントを探し、注視点としてセットする.
    /// </summary>
    private void SetLookAtPoint()
    {
        if (this.Character)
        {
            this.lookAtPoint = this.Character.transform.Search(this.lookAtBoneName);
            if (!this.lookAtPoint)
            {
                this.lookAtPoint = this.Character.transform;
            }
        }
    }
    #endregion

    #region ズーム処理
    public bool IsActive { get; private set; }
    IEnumerator Start()
    {
        this.IsActive = false;
        while (!TouchSystem.Instance)
        {
            yield return 0;
        }
        this.IsActive = true;

        this.LookAtRotation = Quaternion.identity;

        // リミット
        this.Limit(this.Config.zoom, ref this.zoom);
        this.Limit(this.Config.rotationX, ref this._rotation.x);
        this.Limit(this.Config.rotationY, ref this._rotation.y);
        this.Limit(this.Config.rotationZ, ref this._rotation.z);

        this.OnEnable();

        _initialRotation = this.rotation;
    }
    #endregion
    private Vector3 _initialRotation;

    #region ズーム処理
    void OnEnable()
    {
        if (!this.IsActive)
            return;
        TouchSystem.Instance.OnCameraZoom += OnZoom;
    }
    void OnDisable()
    {
        if (TouchSystem.Instance != null)
        {
            TouchSystem.Instance.OnCameraZoom -= OnZoom;
        }
    }
    void OnZoom(float delta)
    {
        float axisZ = delta;
        this.CalcLimit(axisZ, this.Config.zoom, ref this.zoom);
    }
    #endregion

    #region 更新
    void Update()
    {
        this.UpdateCheck();

        /*
        if (null != DetectRay.Instance)
        {
            if (DetectRay.Instance.camType == DetectRay.CamType.Normal)
            {
                RotaToSeeTargetAndPlayer();
            }
            else if (DetectRay.Instance.camType == DetectRay.CamType.Lock)
            {
                RotaToLock();
            }
        }*/

        if (this.directCamera != null)
        {
            // 演出カメラがあるときは演出カメラで映す.
            UpdateDirectCamera();
        }
        else if (this.IsFollow)
        {
            this.UpdateTransform();
        }
        else
        {
            this.UpdateOnlyLookAt();
        }
    }

    public static bool Rota = false;
    private bool RotaToSeeTargetAndPlayer()
    {
        if (!Rota)
        {
            return true;
        }

        if (null != OUILockon.Instance && null != OUILockon.Instance.LockonObject)
        {
            Vector3 target = OUILockon.Instance.LockonObject.transform.position;
            target.y = 0;
            Vector3 player = character.Position;
            player.y = 0;
            Vector3 cam = transform.position;
            cam.y = 0;
            Vector3 dir1 = (target - player).normalized;
            Vector3 dir2 = (cam - player).normalized;
            float fa = 0.67f;
            float rotaspeed = 300f * Time.deltaTime;
            if (Vector3.Dot(dir1, dir2) < 0)
            {
                float rota = Vector3.Cross(dir1, dir2).y;
                if (rota < -fa)
                {
                    this.Rotation = new Vector3(this.Rotation.x, this.Rotation.y + rota * rotaspeed, this.Rotation.z);
                    return true;
                }
                if (rota > fa)
                {
                    this.Rotation = new Vector3(this.Rotation.x, this.Rotation.y + rota * rotaspeed, this.Rotation.z);
                    return true;
                }
            }
            else
            {
                float rota = Vector3.Cross(dir1, dir2).y;
                this.Rotation = new Vector3(this.Rotation.x, this.Rotation.y + rota * rotaspeed, this.Rotation.z);
                return true;
            }

        }
        Rota = false;
        return false;
    }

    private void RotaToLock()
    {
        OUILockon.Instance.RotateCameraToLockObj();
    }

    /// <summary>
    /// カメラ操作更新
    /// </summary>
    void UpdateCheck()
    {
        if (TouchSystem.Instance == null) { return; }
        float axisX = TouchSystem.Instance.Rotation.x;
        float axisY = -TouchSystem.Instance.Rotation.y;


        // X軸回転
        this.CalcLimit(axisY, this.Config.rotationX, ref this._rotation.x);
        // Y軸回転
        this.CalcLimit(axisX, this.Config.rotationY, ref this._rotation.y);
    }


    /// <summary>
    /// Given a relation point, adjust a rotation so the current cc's Rotation * LookAtRotation will look at this point
    /// </summary>
    /// <param name="oldDir"></param>
    /// <returns></returns>
    public Quaternion AdjustLookRotation(Vector3 oldDir)
    {
        Matrix4x4 mLocal;
        {
            Matrix4x4 mRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(this.Rotation), Vector3.one);

            Matrix4x4 mZoom = Matrix4x4.identity;
            mZoom.m23 = this.Zoom;
            mLocal = mRotation * mZoom;
        }

        Vector3 newDir = mLocal.inverse.MultiplyVector(oldDir);
        newDir.y = 0;
        return Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// transform 計算
    /// </summary>
    void UpdateTransform()
    {
        if (this.Character)
        {
            // ローカル座標計算
            Matrix4x4 mLocal;
            {
                //this.Rotation 对左右上下Drag起作用
                Vector3 R = this.Rotation;
                if (LockRotationForSkill)
                {
                    R = new Vector3(_OldLockRotation.x, this.Rotation.y, this.Rotation.z);
                }
                Matrix4x4 mRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(R), Vector3.one);
                Matrix4x4 mZoom = Matrix4x4.identity;
                mZoom.m23 = this.Zoom;
                mLocal = mRotation * mZoom;
            }

            // ワールド座標計算
            Matrix4x4 mWorld;
            {
                Transform transform = this.lookAtPoint;
                if (!transform)
                {
                    SetLookAtPoint();
                    transform = this.lookAtPoint;
                }

                if (this.IsCharaForward)
                {
                    this.IsCharaForward = false;
                    Vector3 rotation = this.Rotation;
                    rotation.y = 0f;
                    this.Rotation = rotation;
                    Vector3 eulerRotation = transform.rotation.eulerAngles;
                    eulerRotation.x = 0;	// 吹き飛び中などに実行されるとカメラの向きがおかしくなるのでy軸以外を無効化.
                    eulerRotation.z = 0;
                    this.LookAtRotation = Quaternion.Euler(eulerRotation);
                }
                Matrix4x4 mAttach = Matrix4x4.TRS(this.Character.transform.position + this.LookAt + new Vector3(0, this.Character.CharaData.HeadPos, 0), 
                        this.LookAtRotation, Vector3.one);
                mWorld = mAttach * mLocal;
            }

            // transform に反映する
            {
                Vector3 position = new Vector3(mWorld.m03, Mathf.Max(this.Config.worldLimitPositionY, mWorld.m13), mWorld.m23);
                Vector3 forward = new Vector3(mWorld.m02, mWorld.m12, mWorld.m22);
                Vector3 upwards = new Vector3(mWorld.m01, mWorld.m11, mWorld.m21);
                this.transform.position = position + pos_shake;
                this.transform.rotation = Quaternion.LookRotation(forward, upwards);
            }
        }
        else
        {
            // 注視点となるキャラクターがいない場合は座標更新しない.
            return;
            /*
            Matrix4x4 mAttach = Matrix4x4.identity;
            mAttach.m03 = this.LookAt.x;
            mAttach.m13 = this.LookAt.y;
            mAttach.m23 = this.LookAt.z;
            mWorld = mAttach * mLocal;
            */
        }
    }

    /// <summary>
    /// 位置を変えずに対象の方向を向く.
    /// </summary>
    void UpdateOnlyLookAt()
    {
        if (this.Character)
        {
            Transform transform = this.lookAtPoint;
            if (!transform)
            {
                SetLookAtPoint();
                transform = this.lookAtPoint;
            }
            this.transform.LookAt(transform);
        }
    }
    #endregion

    #region 計算系
    /// <summary>
    /// リミット付き計算
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="param"></param>
    /// <param name="value"></param>
    void CalcLimit(float axis, Param param, ref float value)
    {
        value += this.Calc(axis, param);
        this.Limit(param, ref value);
    }
    /// <summary>
    /// スピードと時間を考慮した計算結果を返す
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="param"></param>
    float Calc(float axis, Param param)
    {
        float abs = Mathf.Abs(axis);
        if (abs < 0.001f)
            return 0f;

        float sign = Mathf.Sign(axis);
        float speed = Mathf.Min(abs * param.speed, param.maxSpeed);
        float result = speed * sign * Time.deltaTime;

        return result;
    }
    /// <summary>
    /// リミット
    /// </summary>
    /// <param name="param"></param>
    /// <param name="value"></param>
    void Limit(Param param, ref float value)
    {
        if (!param.isLimit)
            return;
        value = Mathf.Clamp(value, param.minLimit, param.maxLimit);
    }
    #endregion

    #region レイ
    private Transform rayTarget;
    private int rayMask = 1 << LayerNumber.MapWallCol;
    public bool getRayHitWall(ref RaycastHit[] rayHitWall)
    {
        if (Character != null)
        {
            if (rayTarget == null)
            {
                rayTarget = this.Character.AvaterModel.RootTransform;
                if (rayTarget == null)
                {
                    return false;
                }
            }

            Debug.DrawRay(this.transform.position, rayTarget.position - this.transform.position, Color.red);

            rayHitWall = Physics.RaycastAll(this.transform.position, rayTarget.position - this.transform.position, -this.Zoom, rayMask);
            return true;
        }
        return false;
    }
    #endregion

    #region カメラ振動
    const float shakingTime = 0.5f;
    static readonly Vector3 shakingValue = new Vector3(0, 0.1f, 0);

    // 与ダメージ振動パラメータ.
    private const float GiveDamageVibrateDistanceSqrMin = 10f * 10f;
    private const float GiveDamageVibrateDistanceSqrMax = 30f * 30f;

    private Vector3 pos_shake;

    static public void GiveDamageShake(Transform attackerTransform, HitInfo hitInfo)
    {
        if (0 < hitInfo.damage)	// 回復やノーダメージの場合は無視.
        {
            if (hitInfo.bulletID != 0)	// 弾丸IDが無い場合(バフダメージ想定)は無視.
            {
                CharacterCamera camera = GameController.CharacterCamera;
                if (camera)
                {
                    // 距離によって振動の仕方を変える.
                    float distance2 = (attackerTransform.position - hitInfo.position).sqrMagnitude;
                    float shakingScale = Mathf.InverseLerp(GiveDamageVibrateDistanceSqrMax, GiveDamageVibrateDistanceSqrMin, distance2);
                    if (0 < shakingScale)
                    {
                        camera.Shake(shakingScale);
                    }
                }
            }
        }
    }

    public void Shake(float shakingScale = 1f)
    {
        StartCoroutine(VibrateCoroutine(shakingScale));
    }
    IEnumerator VibrateCoroutine(float shakingScale)
    {
        float time = shakingTime;
        Vector3 vib = shakingValue * shakingScale;
        while (0 < time)
        {
            pos_shake = vib;
            vib = -vib;
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        pos_shake = Vector3.zero;
    }
    #endregion

    #region 演出カメラワーク
    /// <summary>
    /// 演出カメラ用パラメータ.
    /// </summary>
    private class DirectCameraParam
    {
        Transform position;
        public Transform Position { get { return position; } }
        Transform target;
        public Transform Target { get { return target; } }
        System.Func<bool> isAlive;
        public bool IsAlive { get { return isAlive(); } }

        public DirectCameraParam(Transform position, Transform target, System.Func<bool> isAlive)
        {
            this.position = position;
            this.target = target;
            this.isAlive = isAlive;
        }
    }
    /// <summary>
    /// 演出カメラ.
    /// </summary>
    private DirectCameraParam directCamera;

    /// <summary>
    /// 演出カメラを作成する.
    /// </summary>
    public void CreateDirectCamera(Transform position, Transform target, System.Func<bool> isAlive)
    {
        directCamera = new DirectCameraParam(position, target, isAlive);
    }
    /// <summary>
    /// 演出カメラを削除する.
    /// </summary>
    public void RemoveDirectCamera()
    {
        directCamera = null;
    }
    /// <summary>
    /// 演出カメラ有効時のUpdate.
    /// </summary>
    private void UpdateDirectCamera()
    {
        if (directCamera.IsAlive == false ||
            directCamera.Position == null ||
            directCamera.Target == null)
        {
            directCamera = null;
            return;
        }
        else
        {
            this.transform.position = directCamera.Position.position;
            this.transform.LookAt(directCamera.Target);
        }
    }
    #endregion

    #region カメラリセット
    const float CameraResetTime = 0.5f;
    private bool isResettingCamera = false;	// コルーチン制御用.別スクリプトにしてAddした方が確実に制御できるかも.
    /// <summary>
    /// カメラをプレイヤーの向きにする(補完あり).
    /// </summary>
    public void CameraReset()
    {
        if (!isResettingCamera)
        {
            Player player = GameController.GetPlayer();
            if (player == null)
            { return; }
            // 現状ありえないが,LockonCursorが無い場合はノーロックとみなす.
            if (GUIObjectUI.NowLockonType != OUILockon.Type.None)
                return;
            float rotY = Mathf.DeltaAngle(this.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.y);
            StartCoroutine(CameraResetCoroutine(rotY));
        }
    }
    IEnumerator CameraResetCoroutine(float rotateValue)
    {
        float rotStart = this.rotation.y;
        float rotEnd = this._rotation.y += rotateValue;

        isResettingCamera = true;
        if (0 < CameraResetTime)
        {
            float lerpT = 0;
            while (lerpT < 0.95f)	//Lerp値なので本来は1までだが,カメラ補完の関係かガタつくので手前で止める.
            {
                lerpT += Time.deltaTime / CameraResetTime;

                float t = (Mathf.Sin((lerpT - 0.5f) * Mathf.PI) + 1) / 2;
                this._rotation.y = Mathf.Lerp(rotStart, rotEnd, t);
                yield return new WaitForEndOfFrame();
            }
        }
        this._rotation.y = rotEnd;
        isResettingCamera = false;
    }
    #endregion

    #region 再参戦時のカメラ情報セット
    public void SetReEntryCamera(Vector3 pos, Vector3 rotate)
    {
        if (this.Character == null)
        {
            this.transform.position = pos;
            this.transform.rotation = Quaternion.Euler(rotate);
        }
    }
    #endregion

}
