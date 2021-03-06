/// <summary>
/// 再参戦パケット要求クラス
/// 
/// 2015/12/22
/// </summary>
using System;
using System.Collections;
using Scm.Common.GameParameter;

/// <summary>
/// 再参戦パケット要求クラス
/// </summary>
public class ReEntryState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// 再参戦パケット要求
	/// </summary>
	private ReEnterFieldRequest reEnterFieldRequest = null;

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
    public GameLoginController.ErrorType ErrorState { get; private set; }
    #endregion

    #region 初期化
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReEntryState()
	{
		this.reEnterFieldRequest = new ReEnterFieldRequest();
		this.isExecute = true;

		// 各イベント登録
		this.reEnterFieldRequest.ResponseEvent += ReEntryResponse;
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
	/// 再参戦レスポンス
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ReEntryResponse(object sender, ReEnterFieldEventArgs e)
	{
		switch(e.Result)
		{
			// 成功
			case ReEnterFieldResult.Success:
				// 成功時はBattlePacket側でバトルシーンに遷移する処理が呼ばれるので
				// 状態をそのまま終了させる
				// 接続中メッセージを閉じる
				GUISystemMessage.Close();
				break;

			// バトルが終了していた
			case ReEnterFieldResult.BattleEnd:
				BattleEnd();
				break;

			// 失敗
			case ReEnterFieldResult.Fatal:
			case ReEnterFieldResult.Fail:
			default:
				Fail(e.Result);
				break;
		}

		this.isExecute = false;
	}

	/// <summary>
	/// バトル終了していた時処理
	/// </summary>
	private void BattleEnd()
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// 接続中メッセージを閉じる
		GUISystemMessage.Close();

		// バトル終了メッセージ表示後ロビーシーンに遷移
		GUIMessageWindow.SetModeOK
			(MasterData.GetText(TextType.TX060_ReEnterField_BattleEnd),
			() =>
				{
					if (Scm.Client.GameListener.ConnectFlg)
					{
						TitleMain.NextScene();
					}
					else
					{
						// サーバが切断されていたら切断処理を行う
						this.isDisconnectExecute = true;
						Disconnected();
					}
				}
			);
	}

	/// <summary>
	/// 失敗時処理
	/// </summary>
	private void Fail(ReEnterFieldResult result)
	{
		// 決定ボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// バグレポートとメッセージ表示 タイトルへ戻る
		GUISystemMessage.SetModeOK
			(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX047_ReturnTitleInfo),
			 () => { GUITitle.OpenInfo(); }
			 );
		BugReportController.SaveLogFile(string.Format("ReEnterFieldRes PacketParameterError. ReEnterFieldResult={0}", result));
		GUIDebugLog.AddMessage(string.Format("ReEnterFieldRes PacketParameterError. ReEnterFieldResult={0}", result));
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始
	/// </summary>
	public void Start()
	{
		// 接続中メッセージを閉じる
		GUISystemMessage.Close();

		// メッセージのOKボタンが押されるまで切断されても切断処理を行わない
		this.isDisconnectExecute = false;

		// 再参戦行うかどうかのメッセージウィンドウ表示 ボタンが押されたらパケット送信
		GUIMessageWindow.SetModeOK
			(MasterData.GetText(TextType.TX042_ReEnter),
			()=>
				{
					this.isDisconnectExecute = true;
					if(Scm.Client.GameListener.ConnectFlg)
					{
						SendReEntry();
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
	/// 再参戦情報送信
	/// </summary>
	private void SendReEntry()
	{
		// 再参戦を行うのでサーバに送信
		this.reEnterFieldRequest.Send();
		// サーバ接続中メッセージを表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX041_Connecting));
		GUIDebugLog.AddMessage("再参戦情報取得中・・・");
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
