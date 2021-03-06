/// <summary>
/// プレイヤー生成状態
/// 
/// 2015/12/24
/// </summary>
using System;
using System.Collections;
using Scm.Common.GameParameter;
using UnityEngine;

/// <summary>
/// プレイヤー生成状態
/// </summary>
public class CreatePlayerState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// プレイヤー名最大数
	/// </summary>
	private const int PlayerNameMax = 8;

	/// <summary>
	/// プレイヤー生成パケット要求
	/// </summary>
	private ICreatePlayerRequest createPlayerRequest = null;

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

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public CreatePlayerState()
	{
		this.createPlayerRequest = new CreatePlayerRequest();
		this.isExecute = true;

		// 各イベント登録
		this.createPlayerRequest.ResponseEvent += CreatePlayerResponse;
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
	/// プレイヤー生成レスポンス
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void CreatePlayerResponse(object sender, CreatePlayerEventArgs e)
	{
		// 接続中メッセージを閉じる
		GUISystemMessage.Close();

		switch(e.Result)
		{
			// 成功
			case CreatePlayerResult.Success:
				Success(e.PlayerId);
				break;
			// 保有プレイヤー数オーバー
			case CreatePlayerResult.Over:
				Over();
				break;
			// NGワードが含まれている
			case CreatePlayerResult.NgWordError:
				NgWordError();
				break;
			// 重複エラー
			case CreatePlayerResult.DuplicateNameError:
				DuplicateNameError();
				break;
			// 失敗
			case CreatePlayerResult.Fail:
			default:
				Fail(e.Result);
				break;
		}
	}

	/// <summary>
	/// 成功
	/// </summary>
	private void Success(int playerId)
	{
#if EJPL && !UNITY_EDITOR
        PlayerPrefs.SetString("player_uid", playerId.ToString());
        APaymentHelperDemo.Instance.CreateRole(playerId, ScmParam.Net.UserName, System.DateTime.Now.Second);
#endif

        //TalkingData
#if ANDROID_XY && !UNITY_EDITOR
        AndroidTalkingDataSDKController.Instance.OnCreateRole(ScmParam.Net.UserName);
#endif
        // 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// メッセージウィンドウ表示 OKボタンが押されたらプレイヤー選択セットアップを行う
		GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX046_CreateNameSuccess),
			() =>
			{
				this.isDisconnectExecute = true;
				if(Scm.Client.GameListener.ConnectFlg)
				{
					var eventArgs = new StateBeginExitEventArgs(new SelectPlayerState(playerId));
					OnBeginExit(this, eventArgs);
					this.isExecute = false;
				}
				else
				{
					// 切断されていたら切断処理を実行
					Disconnected();
				}
			});
	}

	/// <summary>
	/// 保有プレイヤー数オーバー
	/// </summary>
	private void Over()
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// 現在の仕様では来ない　送られてきた場合はタイトル情報画面へ遷移
		GUISystemMessage.SetModeOK
			(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX047_ReturnTitleInfo),
			 () =>
			 {
				 GUITitle.OpenInfo();
				 // 実行終了
				 this.isExecute = false;
			 }
			);
		BugReportController.SaveLogFile(string.Format("LoginRes PacketParameterError. LoginResult={0}", CreatePlayerResult.Over));
		GUIDebugLog.AddMessage(string.Format("CreatePlayerRes PacketParameterError. CreatePlayerResult={0}", CreatePlayerResult.Over));
	}

	/// <summary>
	/// NGワードが含まれている
	/// </summary>
	private void NgWordError()
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		GUIDebugLog.AddMessage("入力失敗 名前にNGワードが含まれています");
		GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX044_NGWrod),
			() =>
			{
				this.isDisconnectExecute = true;
				if (Scm.Client.GameListener.ConnectFlg)
				{
					CreatePlayerWindowSetup();
				}
				else
				{
					// 切断されていたら切断処理を実行
					Disconnected();
				}
			}
		);
	}

	/// <summary>
	/// 重複エラー
	/// </summary>
	private void DuplicateNameError()
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		GUIDebugLog.AddMessage("入力失敗 名前が重複しています");
		GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX045_DuplicateName),
			() =>
			{
				this.isDisconnectExecute = true;
				if (Scm.Client.GameListener.ConnectFlg)
				{
					CreatePlayerWindowSetup();
				}
				else
				{
					// 切断されていたら切断処理を実行
					Disconnected();
				}
			}
		);
	}

	/// <summary>
	/// 失敗
	/// </summary>
	private void Fail(CreatePlayerResult result)
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// メッセージ表示後 タイトル情報画面へ遷移
		GUISystemMessage.SetModeOK
			(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX047_ReturnTitleInfo),
			 () =>
			 {
				 GUITitle.OpenInfo();
				 this.isExecute = false;
			 }
			);
		BugReportController.SaveLogFile(string.Format("LoginRes PacketParameterError. LoginResult={0}", result));
		GUIDebugLog.AddMessage(string.Format("CreatePlayerRes PacketParameterError. CreatePlayerResult={0}", result));
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始
	/// </summary>
	public void Start()
	{
		if(string.IsNullOrEmpty(ScmParam.Net.UserName))
		{
            // プレイヤー名が登録されていなければプレイヤー作成画面表示
            UnityEngine.Debug.Log("Input your user name...");
			CreatePlayerWindowSetup();
		}
		else
		{
			// プレイヤー名がすでに登録されていればプレイヤー作成パケット送信
			// 入力ウィンドウのYesボタンが押された時にサーバとの接続が切れ再度接続を行なった時にここの処理が行われる
			SendCreatePlayer();
		}
	}
	#endregion

	#region プレイヤー生成画面
	/// <summary>
	/// プレイヤー生成画面のセットアップ処理s
	/// </summary>
	private void CreatePlayerWindowSetup()
	{
		// システムメッセージを閉じる
		GUISystemMessage.Close();
		// デバッグログを閉じる
		GUIDebugLog.Close();

		this.isDisconnectExecute = false;
		GUIMessageWindow.SetModeInput(MasterData.GetText(TextType.TX043_InputPlayerName),
							  ScmParam.Net.UserName, string.Empty, false,
							  () => {
                                  if (FilterWordController.Instance.IsNeedFilter(ScmParam.Net.UserName)) {
                                      GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX044_NGWrod),
                                          () => {
                                              CreatePlayerWindowSetup();
                                          }
                                      );
                                      return;
                                  }
                                  SendCreatePlayer();
                              },
                              CreatePlayerFail, SetPlayerName, (name) => { }); 
	}

	/// <summary>
	/// 作成したプレイヤー情報送信
	/// 入力ウィンドウのYesボタンが押された時に呼ばれる
	/// </summary>
	private void SendCreatePlayer()
	{
        Debug.Log("SendCreatePlayer...");
		//if (string.IsNullOrEmpty(ScmParam.Net.UserName))
		//	return;

		this.isDisconnectExecute = true;

		// プレイヤー入力ウィンドウを閉じる
		GUIMessageWindow.Close();

		if(Scm.Client.GameListener.ConnectFlg)
		{
			// 作成したプレイヤー情報送信
			this.createPlayerRequest.Send();
			// サーバ接続メッセージ表示
			GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX041_Connecting));
			// デバッグログを表示
			GUIDebugLog.SetActive(true);
			GUIDebugLog.AddMessage("プレイヤー生成中・・・");
		}
		else
		{
			// サーバとの接続が切れていたら切断時処理を実行
			Disconnected();
		}
	}

	/// <summary>
	/// プレイヤー作成失敗
	/// </summary>
	private void CreatePlayerFail()
	{
		// プレイヤー入力ウィンドウを閉じる
		GUIMessageWindow.Close();
		// 失敗後はタイトル情報画面へ遷移
		GUITitle.OpenInfo();
		// 実行終了
		this.isExecute = false;
		this.isDisconnectExecute = true;
	}

	/// <summary>
	/// 入力したプレイヤー名をセット
	/// </summary>
	private void SetPlayerName(string name)
	{
		if (name.Length > PlayerNameMax)
		{
			// 制限を超えていた場合は超えている分の文字列を削除する
			UIInput.current.value = name.Substring(PlayerNameMax, name.Length);
		}
		ScmParam.Net.UserName = name;
        var curPhone = PlayerPrefs.GetString("Name", "");
        var playerName = curPhone + ":" + name;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("SetUserName..");
	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		while(this.isExecute)
		{
			yield return null;
		}
	}
	#endregion

	#region 状態終了
	/// <summary>
	/// 状態の終了処理
	/// </summary>
	public void Finish() { }
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
