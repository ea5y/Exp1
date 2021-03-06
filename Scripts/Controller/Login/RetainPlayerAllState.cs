/// <summary>
/// 全保有プレイヤー情報状態クラス
/// 
/// 2015/12/24
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全保有プレイヤー情報状態クラス
/// </summary>
public class RetainPlayerAllState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// 全保有プレイヤーパケット要求
	/// </summary>
	private IRetainPlayerAllRequest retainPlayerAllRequest = null;

	/// <summary>
	/// 次の状態への遷移開始イベント
	/// </summary>
	public event EventHandler<StateBeginExitEventArgs> OnBeginExit = (sender, e) => { };

	/// <summary>
	/// 状態の実行フラグ
	/// </summary>
	private bool isExecute = true;

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public RetainPlayerAllState()
	{
		this.retainPlayerAllRequest = new RetainPlayerAllRequest();
		this.isExecute = true;

		// 各イベント登録
		this.retainPlayerAllRequest.ResponseEvent += RetainPlayerAllResponse;
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
	/// 全保有プレイヤー情報レスポンス
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void RetainPlayerAllResponse(object sender, RetainPlayerAllEventArgs e)
	{
		List<RetainPlayerInfo> playerInfo = e.RetainPlayerInfoList;
		if(playerInfo.Count == 0)
		{
			// プレイヤーが存在しない

			// プレイヤー作成状態へ
			var eventArgs = new StateBeginExitEventArgs(new CreatePlayerState());
			OnBeginExit(this, eventArgs);
		}
		else
		{
			// プレイヤーが存在

			// プレイヤー選択状態へ(大会版では1プレイヤーのみ)
			var eventArgs = new StateBeginExitEventArgs(new SelectPlayerState(playerInfo));
            PlayerPrefs.SetString("player_uid", playerInfo[0].PlayerID.ToString());
			OnBeginExit(this, eventArgs);
		}

		// 状態実行終了
		this.isExecute = false;
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始
	/// </summary>
	public void Start()
	{
		// 全保有プレイヤー情報パケット送信
		this.retainPlayerAllRequest.Send();

		GUIDebugLog.AddMessage("全保有プレイヤー情報取得中・・・");

	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		// 保有プレイヤーパケットの応答があるまで待機
		while(this.isExecute)
		{
			yield return null;
		}
	}
	#endregion

	#region 状態終了
	/// <summary>
	/// 状態の終了
	/// </summary>
	public void Finish() { }
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
