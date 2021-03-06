/// <summary>
/// 認証
/// 
/// 2013/00/00
/// </summary>
using UnityEngine;
using System.Collections;

using Asobimo.Auth;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class AuthRequest
{
    #region 定義
    /// <summary>
    /// 状態管理
    /// </summary>
    public enum AuthState
    {
        Unknown = 0,	// Start を呼んでいない

        Start = 1,
        Request = 2,
        WaitResponse = 3,

        Complete = 10,	// これ以上の数値は終了扱い

        Fail = 100,		// これ以上の数値は失敗扱い
    };

    /// <summary>
    /// AuthReq パケットを送る間隔
    /// 
    /// Xigncode の Seed の有効時間でチェックする
    /// 2016/07/13 では有効時間は1時間
    /// </summary>
    const float AuthReqInterval = 3600.0f;		// 秒単位
    #endregion

    #region フィールド&プロパティ
    /// <summary>
    /// 現在の状態
    /// </summary>
    static AuthState _state = AuthState.Unknown;
    public static AuthState State { get { return _state; } private set { _state = value; } }

    /// <summary>
    /// AuthRes.Seed 値保存
    /// </summary>
    static string _seed = string.Empty;
    public static string Seed
    {
        get { return _seed; }
        private set
        {
            _seed = value;

            // 時間を保存する
            SeedTime = Time.time;
        }
    }

    /// <summary>
    /// AuthRes.Seed 値を保存した時間
    /// </summary>
    static float _seedTime = 0f;
    static float SeedTime { get { return _seedTime; } set { _seedTime = value; } }
    #endregion

    #region 認証パケットリクエスト開始
    /// <summary>
    /// 認証パケットリクエスト開始
    /// 
    /// 内部でAuthReqのインターバルが過ぎていたら勝手に呼ぶ
    /// </summary>
    public static void StartRequest()
    {
        if (Time.time >= SeedTime + AuthReqInterval)
        {
            AuthRequest.Start();
        }
    }
    /// <summary>
    /// 認証パケットリクエスト強制開始
    /// </summary>
    public static void StartForceRequest()
    {
        GUIDebugLog.AddMessage(2, "Auth Request ForceStart");
        AuthRequest.Start();
    }
    static void Start()
    {
        //		AuthRequest.Start(
        //			ApplicationController.PlatformType,
        //			Scm.Common.GameParameter.MarketType.Unknown,
        //			Scm.Common.Utility.Language,
        //			AsobimoAuthCreateType.Android,
        //            AuthEntry.Instance.AuthMethod.distributionCode,
        //			AuthEntry.Instance.AuthMethod.openID);
#if EJPL
        byte code = DistributionCode.GetByName(PlayerPrefs.GetString("XYPlatformCode", "xiaoyoupf"));
#else
        byte code = DistributionCode.GetByName("xiaoyoupf");
#endif
        AuthRequest.Start(
            ApplicationController.PlatformType,
            Scm.Common.GameParameter.MarketType.Unknown,
            Scm.Common.Utility.Language,
            (AsobimoAuthCreateType)ApplicationController.PlatformType,
            code,
#if EJPL
            PluginController.AuthInfo.EJInfo
#else
            PluginController.AuthInfo.token
#endif
            );
    }
    static void Start(PlatformType platformType, MarketType marketType, Language language, AsobimoAuthCreateType authType, byte distributionCode, string asobimoToken)
    {
        FiberController.AddFiber(AuthRequestCoroutinue(platformType, marketType, language, authType, distributionCode, asobimoToken));
    }
    static IEnumerator AuthRequestCoroutinue(PlatformType platformType, MarketType marketType, Language language, AsobimoAuthCreateType authType, byte distributionCode, string asobimoToken)
    {
        var authManager = new AuthRequestManager(platformType, marketType, language, authType, distributionCode, asobimoToken);
        while (authManager.Update())
        {
            // 状態更新
            State = authManager.State;
            yield return null;
        }

        // 最終的な状態を取得
        State = authManager.State;
        // Xigncode で使用する Seed を保存しておく
        Seed = authManager.Seed;
        GUIDebugLog.AddMessage(2, string.Format("Auth State:{0}", State));
        GUIDebugLog.AddMessage(2, string.Format("Auth SeedTime:{0}", SeedTime));
        GUIDebugLog.AddMessage(2, string.Format("Auth Seed:{0}", Seed));
        GUIDebugLog.AddMessage(2, "Auth Request End");
    }
#endregion
}

/// <summary>
/// 認証パケットリクエストマネージャ
/// </summary>
public class AuthRequestManager : IOnNetworkDisconnect, IPacketResponse<AuthRes>
{
#region フィールド＆プロパティ
    // サーバーに接続する試行回数
    const int ConnectRequestCount = 3;
    // サーバーに接続レスポンスタイムアウト
    const float ConnectRequestTimeout = 10f;
    // 認証パケットリクエストの試行回数
    const int AuthRequestCount = 3;
    // 認証パケットのレスポンスタイムアウト
    const float AuthRequestTimeout = 10f;

    /// <summary>
    /// 認証ステータス
    /// </summary>
    public AuthRequest.AuthState State { get; private set; }

    // 接続ステータス
    ConnectStatus ConnectState { get; set; }
    public enum ConnectStatus
    {
        ConnectMasterServer,
        WaitMasterServerConnect,
        GetServerMapping,
        SetConnect,
        WaitConnect,
        Connect,
    };

    /// <summary>
    /// Xigncode で使用する Seed
    /// </summary>
    public string Seed { get; private set; }

    // 現在のプラットフォームタイプ
    PlatformType PlatformType { get; set; }
    // 配信マーケットタイプ
    MarketType MarketType { get; set; }
    // 言語設定
    Language Language { get; set; }
    // 認証タイプ
    AsobimoAuthCreateType AuthType { get; set; }
    // DistributionCode
    byte DistributionCode { get; set; }
    // アソビモトークン
    string AsobimoToken { get; set; }

    // 接続試行回数
    int ConnectCount { get; set; }
    // 接続タイムアウト
    float ConnectTimeout { get; set; }

    // 認証試行回数
    int AuthCount { get; set; }
    // 認証タイムアウト
    float AuthTimeout { get; set; }
    // メンバー初期化
    void MemberInit()
    {
        this.State = AuthRequest.AuthState.Start;
        this.ConnectState = ConnectStatus.ConnectMasterServer;

        this.PlatformType = PlatformType.Android;
        this.MarketType = MarketType.Unknown;
        this.Language = Scm.Common.GameParameter.Language.English;
        this.AuthType = AsobimoAuthCreateType.Android;
        this.AsobimoToken = "";
        this.DistributionCode = Scm.Common.GameParameter.DistributionCode.Asobimo;

        this.ConnectCount = 0;
        this.ConnectTimeout = 0f;

        this.AuthCount = 0;
        this.AuthTimeout = 0f;
    }
#endregion

#region 初期化
    public AuthRequestManager(PlatformType platformType, MarketType marketType, Language language, AsobimoAuthCreateType authType, byte distributionCode, string asobimoToken)
    {
        this.MemberInit();

        this.PlatformType = platformType;
        this.MarketType = marketType;
        this.Language = language;
        this.AuthType = authType;
        this.DistributionCode = distributionCode;
        this.AsobimoToken = asobimoToken;
    }
#endregion

#region IOnNetworkDisconnect
    /// <summary>
    /// 通信切断された時
    /// </summary>
    public void Disconnect()
    {
        GUIDebugLog.AddMessage(2, string.Format("Disconnect by Photon({0})", this.ConnectCount));
        // ステート変更
        if (this.ConnectState == ConnectStatus.GetServerMapping 
            || this.ConnectState == ConnectStatus.WaitMasterServerConnect
            || this.ConnectState == ConnectStatus.ConnectMasterServer) {
            this.ConnectState = ConnectStatus.ConnectMasterServer;	// リトライ
        } else {
            this.ConnectState = ConnectStatus.SetConnect;   // リトライ
        }
    }
#endregion

#region IPacketResponse
    /// <summary>
    /// 送信したパケットのレスポンス
    /// </summary>
    public void Response(AuthRes packet)
    {
        // 認証パケットのレスポンス
        this.AuthResponse(packet.AuthResult, packet.Seed);
    }
#endregion

#region 更新
    /// <summary>
    /// 更新処理
    /// </summary>
    /// <returns>true = 継続中、false = 終了</returns>
    public bool Update()
    {
        return this.UpdateConnect();
    }
#endregion

#region 接続処理
    /// <summary>
    /// 接続更新処理
    /// </summary>
    bool UpdateConnect()
    {
        switch (this.ConnectState)
        {
            case ConnectStatus.ConnectMasterServer: return this.ConnectState_ConnectMasterServer();
            case ConnectStatus.WaitMasterServerConnect: return this.ConnectState_WaitMasterServerConnect();
            case ConnectStatus.GetServerMapping: return this.ConnectState_GetServerMapping();
            case ConnectStatus.SetConnect: return this.ConnectState_SetConnect();
            case ConnectStatus.WaitConnect: return this.ConnectState_WaitConnect();
            case ConnectStatus.Connect: return this.UpdateAuth();
        }

        // 不明なステートなので失敗扱いで終了する
        GUIDebugLog.AddMessage(2, string.Format("Invalid Connect State:{0}", this.ConnectState));
        this.State = AuthRequest.AuthState.Fail;
        return false;	// 処理終了
    }

    bool ConnectState_ConnectMasterServer() {
        if (this.ConnectCount < ConnectRequestCount) {
            // サーバーに接続
            this.ConnectCount++;
            this.ConnectTimeout = Time.time + ConnectRequestTimeout;
            Scm.Client.GameListener.Connect(ScmParam.MasterHost);
            GUIDebugLog.AddMessage(2, string.Format("Connect({0})", this.ConnectCount));
            GUIDebugLog.AddMessage(2, string.Format("Host:{0}", ScmParam.MasterHost));

            // 切断処理登録
            SceneController.AddDisconnect(this);

            // ステート変更
            this.ConnectState = ConnectStatus.WaitMasterServerConnect;
            return true;	// 処理継続中...
        } else {
            // ステート変更
            this.State = AuthRequest.AuthState.Fail;
            return false;	// 処理終了
        }
    }

    bool ConnectState_WaitMasterServerConnect() {
        if (Scm.Client.GameListener.ConnectFlg) {
            // ステート変更
            this.ConnectState = ConnectStatus.GetServerMapping; // 接続した

#if EJPL
            this.ConnectState = ConnectStatus.Connect; 
#elif XW_DEBUG
            if (ScmParam.Debug.File.Environment == EnvironmentType.Any) {
                // Do not send server mapping, use the host in the debug file directly
                this.ConnectState = ConnectStatus.Connect;      // this connect is ok
            } else {
                this.ConnectTimeout = Time.time + ConnectRequestTimeout;
                CommonPacket.SendServerMapping((res) => {
                    string hostPort = string.Empty;
                    foreach (var server in res.GetServerMappingParameters()) {
                        if (server.IsEnterable) {
                            hostPort = server.Address;
                        }
                    }

                    ScmParam.ConnectHost = hostPort;
                    if (ScmParam.MasterHost != hostPort) {
                        Scm.Client.GameListener.Disconnect();
                        this.ConnectCount = 0;
                        this.ConnectTimeout = 0;
                        this.ConnectState = ConnectStatus.SetConnect;   // Should reconnect
                    } else {
                        this.ConnectState = ConnectStatus.Connect;      // this connect is ok
                    }
                });
            }
#elif BETA_SERVER
            CommonPacket.SendServerMapping((res) => {
                string hostPort = string.Empty;
                foreach (var server in res.GetServerMappingParameters()) {
                    if (server.IsEnterable) {
                        hostPort = server.Address;
                    }
                }

                ScmParam.ConnectHost = hostPort;
                if (ScmParam.MasterHost != hostPort) {
                    Scm.Client.GameListener.Disconnect();
                    this.ConnectState = ConnectStatus.SetConnect;   // Should reconnect
                } else {
                    this.ConnectState = ConnectStatus.Connect;	    // this connect is ok
                }
            });
#else
            this.ConnectState = ConnectStatus.Connect;      // this connect is ok
#endif
        } else {
            // タイムアウトチェック
            if (this.ConnectTimeout <= Time.time) {
                GUIDebugLog.AddMessage(2, string.Format("Connect Timeout({0})", this.ConnectCount));
                // ステート変更
                this.ConnectState = ConnectStatus.ConnectMasterServer;	// リトライ
            }
        }

        return true;	// 処理継続中...
    }

    bool ConnectState_GetServerMapping() {
        // No extra operation required, actual action performed in the callback of CommonPacket.SendServerMapping
        if (this.ConnectTimeout <= Time.time) {
            GUIDebugLog.AddMessage(2, string.Format("Connect Timeout({0})", this.ConnectCount));
            // ステート変更
            this.ConnectState = ConnectStatus.ConnectMasterServer;	// リトライ
        }
        return true;
    }

    /// <summary>
    /// サーバー接続
    /// </summary>
    bool ConnectState_SetConnect()
    {
        if (this.ConnectCount < ConnectRequestCount)
        {
            // サーバーに接続
            this.ConnectCount++;
            this.ConnectTimeout = Time.time + ConnectRequestTimeout;
            Scm.Client.GameListener.Connect(ScmParam.ConnectHost);
            GUIDebugLog.AddMessage(2, string.Format("Connect({0})", this.ConnectCount));
            GUIDebugLog.AddMessage(2, string.Format("Host:{0}", ScmParam.ConnectHost));

            // 切断処理登録
            SceneController.AddDisconnect(this);

            // ステート変更
            this.ConnectState = ConnectStatus.WaitConnect;
            return true;	// 処理継続中...
        }
        else
        {
            // ステート変更
            this.State = AuthRequest.AuthState.Fail;
            return false;	// 処理終了
        }
    }
    /// <summary>
    /// サーバー接続中
    /// </summary>
    bool ConnectState_WaitConnect()
    {
        if (Scm.Client.GameListener.ConnectFlg)
        {
            // ステート変更
            this.ConnectState = ConnectStatus.Connect;	// 接続した
        }
        else
        {
            // タイムアウトチェック
            if (this.ConnectTimeout <= Time.time)
            {
                GUIDebugLog.AddMessage(2, string.Format("Connect Timeout({0})", this.ConnectCount));
                // ステート変更
                this.ConnectState = ConnectStatus.SetConnect;	// リトライ
            }
        }

        return true;	// 処理継続中...
    }
    /// <summary>
    /// 切断処理
    /// </summary>
    void DisconnectServer()
    {
        // 切断処理削除
        SceneController.RemoveDisconnect(this);

        // サーバー切断
        Scm.Client.GameListener.Disconnect();
        GUIDebugLog.AddMessage(2, "Disconnect");
    }
#endregion

#region 認証パケット処理
    /// <summary>
    /// 認証処理
    /// </summary>
    bool UpdateAuth()
    {
        switch (this.State)
        {
            case AuthRequest.AuthState.Start: return this.State_Start();
            case AuthRequest.AuthState.Request: return this.State_Request();
            case AuthRequest.AuthState.WaitResponse: return this.State_WaitResponse();
            case AuthRequest.AuthState.Fail: return this.State_Fail();
            case AuthRequest.AuthState.Complete: return this.State_Complete();
        }

        // 不明なステータスのため失敗ステートに変更する
        GUIDebugLog.AddMessage(2, string.Format("Invalid Auth State:{0}", this.State));
        this.State = AuthRequest.AuthState.Fail;
        return true;	// 処理継続中...
    }
    /// <summary>
    /// 認証スタート
    /// </summary>
    bool State_Start()
    {
        // ステート変更
        this.State = AuthRequest.AuthState.Request;
        return true;	// 処理継続中...
    }
    /// <summary>
    /// 認証パケットリクエスト
    /// </summary>
    bool State_Request()
    {
        // 規定回数実行していたら失敗扱いにする
        if (this.AuthCount < AuthRequestCount)
        {
            // 認証パケットを送信
            this.AuthCount++;
            this.AuthTimeout = Time.time + AuthRequestTimeout;
            CommonPacket.SendAuth(this.PlatformType, this.MarketType, this.Language, (byte)this.AuthType, this.DistributionCode, this.AsobimoToken, this);
            GUIDebugLog.AddMessage(2, string.Format("Auth Request({0})", this.AuthCount));
            GUIDebugLog.AddMessage(2, string.Format("PlatformType:{0}({1})", this.PlatformType, (int)this.PlatformType));
            GUIDebugLog.AddMessage(2, string.Format("MarketType:{0}({1})", this.MarketType, (int)this.MarketType));
            GUIDebugLog.AddMessage(2, string.Format("Language:{0}({1})", this.Language, (int)this.Language));
            GUIDebugLog.AddMessage(2, string.Format("AuthType:{0}({1})", this.AuthType, (int)this.AuthType));
            GUIDebugLog.AddMessage(2, string.Format("AsobimoToken:{0}", this.AsobimoToken));

            // ステート変更
            this.State = AuthRequest.AuthState.WaitResponse;
        }
        else
        {
            // ステート変更
            this.State = AuthRequest.AuthState.Fail;
        }
        return true;	// 処理継続中...
    }
    /// <summary>
    /// 認証パケットレスポンスチェック中
    /// </summary>
    bool State_WaitResponse()
    {
        // タイムアウトチェック
        if (this.AuthTimeout <= Time.time)
        {
            GUIDebugLog.AddMessage(2, string.Format("Auth Request Timeout({0})", this.AuthCount));
            // ステート変更
            this.State = AuthRequest.AuthState.Request;	// リトライ
        }

        // レスポンス結果は IPacketResponse.Response で受け取る
        return true;	// 処理継続中...
    }
    /// <summary>
    /// 認証失敗
    /// </summary>
    bool State_Fail()
    {
        // 認証終了
        this.AuthEnd();
        return false;	// 処理終了
    }
    /// <summary>
    /// 認証成功
    /// </summary>
    bool State_Complete()
    {
        // 認証終了
        this.AuthEnd();
        return false;	// 処理終了
    }

    /// <summary>
    /// 認証パケットのレスポンス
    /// </summary>
    void AuthResponse(AuthResult result, string seed)
    {
        GUIDebugLog.AddMessage(2, string.Format("Auth Response({0}) Result:{1} Seed:{2}", this.AuthCount, result, seed));
        this.Seed = seed;
        switch (result)
        {
            default:
            case AuthResult.Fail:
                // ステート変更
                this.State = AuthRequest.AuthState.Request;	// リトライ
                break;
            case AuthResult.Success:
                // ステート変更
                this.State = AuthRequest.AuthState.Complete;
                break;
        }
    }
    /// <summary>
    /// 認証終了
    /// </summary>
    void AuthEnd()
    {
        GUIDebugLog.AddMessage(2, string.Format("Auth End({0}) State:{1}", this.AuthCount, this.State));

        // 全ての処理が終わったので切断する
        this.DisconnectServer();
    }
#endregion
}

