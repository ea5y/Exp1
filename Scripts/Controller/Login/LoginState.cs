/// <summary>
/// ログイン状態クラス
/// 
/// 2015/12/16
/// </summary>
using System;
using System.Collections;
using Scm.Common.GameParameter;
using Scm.Client;
using UnityEngine;

/// <summary>
/// ログイン状態クラス
/// </summary>
public class LoginState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// ログインパケット要求
	/// </summary>
	private ILoginRequest loginRequest = null;

	/// <summary>
	/// 次の状態への遷移開始イベント
	/// </summary>
	public event EventHandler<StateBeginExitEventArgs> OnBeginExit = (sender, e) => { };

	/// <summary>
	/// 状態の実行フラグ
	/// </summary>
	private bool isExecute = true;

	/// <summary>
	/// 切断処理を実行するかどうか
	/// </summary>
	private bool isDisconnectExecute = true;

    private float sendDelaySeconds = 0;

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public LoginState(float delaySeconds)
	{
		this.loginRequest = new LoginRequest();
		this.isExecute = true;
        this.sendDelaySeconds = delaySeconds;

		// 各イベント登録
		this.loginRequest.ResponseEvent += LoginResponse;
	}
	#endregion

	#region 破棄
	/// <summary>
	/// 破棄
	/// </summary>
	public void Dispose()
	{
		this.OnBeginExit = null;
	}
	#endregion

	#region イベント
	/// <summary>
	/// ログインレスポンス
	/// </summary>
	public void LoginResponse(object sender, LoginRequestEventArgs e)
	{
		switch(e.Result)
		{
			// ログイン成功
			case LoginResult.Success:
				LoginSuccess();
				break;
			// アプリのバージョンが違う
			case LoginResult.VersionMismatch:
				VersionMismatch();
				break;
			// サーバメンテナス
			case LoginResult.Maintenance:
				Maintenance();
				break;
			// サーバ満員
			case LoginResult.ServerFull:
				ServerFull();
				break;
			// 認証失敗(アソビモトークン等が失敗した場合にくる)
			case LoginResult.AuthFail:
				AuthFail();
				break;
			// Xigncodeで失敗
			case LoginResult.XFail:
				AuthFail();
				break;
			case LoginResult.Fail:
				AuthFail();
				break;
            case LoginResult.NeedWait:
                WaitRelogin();
                break;
			// 不明
			default:
			{
				// メッセージのOKボタンが押されるまで切断処理を停止
				this.isDisconnectExecute = false;

				// バグレポートとメッセージ表示 タイトルへ戻る
				GUISystemMessage.SetModeOK
					(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX047_ReturnTitleInfo),
					 () =>
					 {
						 GUITitle.OpenInfo();
						 this.isExecute = false;
					 }
					);
				BugReportController.SaveLogFile(string.Format("LoginRes PacketParameterError. LoginResult={0}", e.Result));
				GUIDebugLog.AddMessage(string.Format("LoginRes PacketParameterError. LoginResult={0}", e.Result));
				break;
			}
		}
	}

    private void WaitRelogin() {
        // 全保有プレイヤー情報取得遷移へ
        var eventArgs = new StateBeginExitEventArgs(new LoginState(3.0f));
        OnBeginExit(this, eventArgs);

        // 状態を終了させる
        this.isExecute = false;
    }

	/// <summary>
	/// ログイン成功処理
	/// </summary>
	private void LoginSuccess()
	{
		// 全保有プレイヤー情報取得遷移へ
		var eventArgs = new StateBeginExitEventArgs(new RetainPlayerAllState());
		OnBeginExit(this, eventArgs);

		// 状態を終了させる
		this.isExecute = false;
	}

	/// <summary>
	/// アプリバージョン違い時処理
	/// </summary>
	private void VersionMismatch()
	{
		// ログイン状態は終了させる
		var eventArgs = new StateBeginExitEventArgs(null);
		OnBeginExit(this, eventArgs);

		// アプリのダウンロード処理へ
		GUIAuth.ApplicationUpgradeMessage();

		// 状態を終了させる
		this.isExecute = false;
	}

	/// <summary>
	/// メンテナス処理
	/// </summary>
	private void Maintenance()
	{
		// メッセージのOKボタンが押されるまで切断処理を停止
		this.isDisconnectExecute = false;

		// タイトルシーンへ
		GUISystemMessage.SetModeOK
			(MasterData.GetText(TextType.TX048_MainteTitle), MasterData.GetText(TextType.TX049_Mainte),
			() =>
			{
				// タイトルへ戻す
				GUITitle.OpenInfo();

				// 状態を終了させる
				this.isExecute = false;
			}
		);
	}

	/// <summary>
	/// サーバ満員処理
	/// </summary>
	private void ServerFull()
	{
		// メッセージのOKボタンが押されるまで切断処理を停止
		this.isDisconnectExecute = false;

		// Yes時タイトルシーンへ No時はアプリ終了
		GUISystemMessage.SetModeYesNo
			(MasterData.GetText(TextType.TX154_Infomation_ScreenTitle), MasterData.GetText(TextType.TX061_ServerFull),
			MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX158_Common_QuitButton), true,
			() =>
				{
					// タイトルへ戻す
					GUITitle.OpenInfo();
					// 状態を終了させる
					this.isExecute = false;
				},
			()=>
				{
					// アプリ終了
					UnityEngine.Application.Quit();
					// 状態を終了させる
					this.isExecute = false;
				}
			);
	}

	/// <summary>
	/// 認証失敗処理
	/// </summary>
	private void AuthFail()
	{
		// メッセージのOKボタンが押されるまで切断処理を停止
		this.isDisconnectExecute = false;

		// 認証シーンへ
		GUISystemMessage.SetModeOK
			(MasterData.GetText(TextType.TX155_Error_ScreenTitle), MasterData.GetText(TextType.TX148_Auth_Error), true,
			() =>
			{
				// BGM停止
				SoundController.StopBGM();
				// 認証シーンへ
				AuthMain.LoadScene();
				// 状態を終了させる
				this.isExecute = false;
			}
		);
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始の処理
	/// </summary>
	public void Start()
	{
		// メッセージ表示
		GUIDebugLog.AddMessage("ログイン中・・・");
	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
        float sendTime = Time.fixedTime + sendDelaySeconds;
        while (Time.fixedTime < sendTime) {
            yield return null;
        }
        // ログインパケット送信
        this.loginRequest.Send();

        // ログインパケットの応答があるまで待機
        while (this.isExecute)
		{
			yield return null;
		}
	}
	#endregion

	#region 状態終了
	/// <summary>
	/// 状態の終了処理
	/// </summary>
	public void Finish(){}
	#endregion

	#region 切断
	/// <summary>
	/// サーバ切断時処理
	/// </summary>
	public bool Disconnected()
	{
		// 切断処理実行フラグがOFFの場合は処理を行わない
		if (this.isDisconnectExecute)
		{
			// 切断されたら接続状態に遷移させる
			var eventArgs = new StateBeginExitEventArgs(new ConnectState());
			OnBeginExit(this, eventArgs);

			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion
}