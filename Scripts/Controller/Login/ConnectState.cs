/// <summary>
/// 接続状態クラス
/// 
/// 2015/12/16
/// </summary>
using System;
using System.Collections;
using UnityEngine;
using Scm.Client;

public class ConnectState : IGameLoginState
{
	#region フィールド&プロパティ
	// サーバーに接続レスポンスタイムアウト
	public const float ConnectRequestTimeout = 10f;

	// 接続タイムアウト
	private float connectTimeou = 0;

	/// <summary>
	/// 次の状態への遷移開始イベント
	/// </summary>
	public event EventHandler<StateBeginExitEventArgs> OnBeginExit = (sender, e) => { };

	/// <summary>
	/// TODO: ゲームサーバ接続時によるタイムアウト処理が本実装になるまでの仮処理
	/// 切断処理呼び出し用
	/// </summary>
	public Action OnDiscoonect = () => { };

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

    public ConnectState() {
        
    }

	#region 破棄
	/// <summary>
	/// 破棄
	/// </summary>
	public void Dispose()
	{
		this.OnBeginExit = null;
		this.OnDiscoonect = null;
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始の処理
	/// </summary>
	public void Start()
	{
		// ゲームサーバに接続
		NetworkController.Connect();

		// 接続中メッセージ表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX041_Connecting));
		GUIDebugLog.AddMessage("サーバ接続中・・・");

		// 接続待ちタイムセット
		this.connectTimeou = Time.time + ConnectRequestTimeout;
	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		while(true)
		{
			if(GameListener.ConnectFlg && AuthRequest.State == AuthRequest.AuthState.Complete)
			{
				// 接続完了 ログイン状態に遷移させる
				var eventArgs = new StateBeginExitEventArgs(new LoginState(0));
				OnBeginExit(this, eventArgs);
				yield break;
			}
			else
			{
				if(this.connectTimeou <= Time.time)
				{
					// タイムアウトしたので切断処理を呼び出す
					// TODO: ゲームサーバ接続時によるタイムアウト処理が本実装になるまでの仮処理
					OnDiscoonect();
					yield break;
				}
			}

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
		// 切断されたら接続状態に遷移させる
		var eventArgs = new StateBeginExitEventArgs(new ConnectState());
		OnBeginExit(this, eventArgs);

		return true;
	}
	#endregion
}
