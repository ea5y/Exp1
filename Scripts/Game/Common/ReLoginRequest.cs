/// <summary>
/// ゲームに再接続を行うためのクラス
/// 
/// 2015/05/29
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class ReLoginRequst
{
	private static Fiber reLoginFiber;

	/// <summary>
	/// 再ログインリクエスト開始
	/// </summary>
	public static void StartRequest()
	{
		// 再ログイン中なら登録しない
		if(FiberController.Contains(reLoginFiber)) return;
		// 再ログイン開始
		reLoginFiber = FiberController.AddFiber(ReLoginRequestCoroutinue());
	}
	static IEnumerator ReLoginRequestCoroutinue()
	{
		GUIDebugLog.AddMessage("StartReLoginRequst");
		var loginManager = new ReLoginRequestManager();
		while (loginManager.Update())
		{
			yield return null;
		}
		GUIDebugLog.AddMessage("EndReLoginRequst");
	}
}


/// <summary>
/// 再ログイン制御/管理クラス
/// </summary>
public class ReLoginRequestManager : IOnNetworkDisconnect, IOnNetworkDisconnectByServer, IPacketResponse<LoginRes>, IPacketResponse<RetainPlayerAllRes>,
									IPacketResponse<SelectPlayerRes>, IPacketResponse<ServerStatusRes>, IPacketResponse<ReEnterFieldRes>
{
	#region 定数
	/// <summary>
	/// 最大接続リトライ回数
	/// </summary>
	private const int ReConnectMax = 5;
	#endregion

	#region フィールド&プロパティ
	/// <summary>
	/// ログイン状態
	/// </summary>
	private StateType State { get; set; }
	private enum StateType
	{
		Connecting,				// 接続中
		Login,					// ログイン中
		RetainPlayer,			// 保有プレイヤー取得中
		SelectPlayer,			// プレイヤー選択
		ServerStatus,			// サーバ情報取得中
		ReEntry,				// 再参戦取得中
		LoginEnd,				// ログイン終了
	}
	
	/// <summary>
	/// 選択されたプレイヤーID
	/// </summary>
	private int selectPlayerId;
	
	/// <summary>
	/// 再接続回数
	/// </summary>
	private int reConnectCount;
	#endregion
	
	#region 初期化
	public ReLoginRequestManager()
	{
		GUIDebugLog.SetActive(true);
		ChangeState(StateType.Connecting);
		// 切断処理登録
		SceneController.AddDisconnect(this);
		SceneController.AddDisconnectByServer(this);
		this.reConnectCount = 0;
		// フェードアウト開始
		GUIFade.FadeOut(true);
	}
	#endregion
	
	#region 更新
	/// <summary>
	/// 更新
	/// </summary>
	public bool Update()
	{
		switch(this.State)
		{
			case StateType.Connecting:
				WaitConnectUpdate();
				break;
		}
		
		return (this.State != StateType.LoginEnd);
	}
	#endregion
	
	#region 接続
	/// <summary>
	/// ゲームサーバに接続を行う
	/// </summary>
	private void ConnectSetup()
	{
		// 接続
		this.reConnectCount++;
		NetworkController.Connect();
		// 再接続開始メッセージ表示
		GUISystemMessage.SetModeMessage(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX062_ReConnect));
		GUIDebugLog.AddMessage("サーバ接続中・・・");
	}
	
	/// <summary>
	/// 接続待機中
	/// </summary>
	private void WaitConnectUpdate()
	{
		if (Scm.Client.GameListener.ConnectFlg)
		{
			// 接続完了 ログイン開始
			ChangeState(StateType.Login);
		}
	}
	#endregion
	
	#region ログイン
	/// <summary>
	/// ログイン開始
	/// </summary>
	private void LoginSetup()
	{
		this.reConnectCount = 0;
		NetworkController.SendLogin(this);
		GUIDebugLog.AddMessage("ログイン中・・・");
	}

	/// <summary>
	/// ログインパケット受信
	/// </summary>
	public void Response(LoginRes packet)
	{
		switch(packet.LoginResult)
		{
			// ログイン成功
			case LoginResult.Success:
			{
				// 保有プレイヤー情報取得へ
				ChangeState(StateType.RetainPlayer);
				break;
			}
			// アプリのバージョンが違う
			case LoginResult.VersionMismatch:
			{
				this.isReconnect = false;

				// アプリのダウンロード処理へ
				GUIAuth.ApplicationUpgradeMessage();
				break;
			}
			// サーバがメンテナンス
			case LoginResult.Maintenance:
			{
				this.isReconnect = false;

				// タイトルシーンへ
				GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX048_MainteTitle), MasterData.GetText(TextType.TX049_Mainte), NextTitleScene);
				break;
			}
			// サーバが満員
			case LoginResult.ServerFull:
			{
				this.isReconnect = false;

				// Yes時タイトルシーンへ No時はアプリ終了
				GUISystemMessage.SetModeYesNo(MasterData.GetText(TextType.TX154_Infomation_ScreenTitle), MasterData.GetText(TextType.TX061_ServerFull),
			                              MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX158_Common_QuitButton), true, NextTitleScene, Application.Quit);
				break;
			}
			// 失敗
			// 認証失敗
			case LoginResult.Fail:
			case LoginResult.AuthFail:
			case LoginResult.XFail:
			{
				this.isReconnect = false;

				// 認証シーンに飛ばす
				GUISystemMessage.SetModeOK
					(
					MasterData.GetText(TextType.TX155_Error_ScreenTitle), MasterData.GetText(TextType.TX148_Auth_Error), true,
						() =>
						{
							NextAuthScene();
						}
				);
				break;
			}
			default:
			{
				GUISystemMessage.SetModeOK
				(
						MasterData.GetText(TextType.TX029_DisconnectTitle),
						MasterData.GetText(TextType.TX047_ReturnTitleInfo),
						NextTitleScene
				);
				GUIDebugLog.AddMessage(string.Format("ログイン失敗 LoginResult={0}", packet.LoginResult));
				BugReportController.SaveLogFile(string.Format("LoginRes PacketParameterError. LoginResult={0}", packet.LoginResult));
				break;
			}
		}
	}
	#endregion
	
	#region 保有プレイヤー情報
	/// <summary>
	/// 保有プレイヤー情報取得開始
	/// </summary>
	private void RetainPlayerAllSetup()
	{
		NetworkController.SendRetainPlayerAll(this);
		GUIDebugLog.AddMessage("全保有プレイヤー情報取得中・・・");
	}
	
	/// <summary>
	/// 保有プレイヤー情報受信
	/// </summary>
	public void Response(RetainPlayerAllRes packet)
	{
		// パラメータ変換
		var retainPlayerList = new List<RetainPlayerInfo>();
		foreach(var info in packet.GetPlayerPackets())
		{
			retainPlayerList.Add(new RetainPlayerInfo(info));
		}
		
		if(retainPlayerList.Count != 0)
		{
			// プレイヤー選択状態へ
			// UNDONE: 大会版では1プレイヤーのみ
			this.selectPlayerId = retainPlayerList[0].PlayerID;
			ChangeState(StateType.SelectPlayer);
		}
		else
		{
			this.isReconnect = false;

			// バトルシーン中ではすでにプレイヤーが作成されているはずなのでプレイヤー作成処理に移ることは本来はありえない
			// もし作成処理が来た場合はタイトル画面に移る
			GUISystemMessage.SetModeOK
			(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX047_ReturnTitleInfo),
					NextTitleScene
			);
			GUIDebugLog.AddMessage(string.Format("プレイヤーが作成されていません"));
			BugReportController.SaveLogFile(string.Format("RetainPlayerAllRes PacketParameterError. NotPlayerInfo={0}", retainPlayerList.Count));
		}
	}
	#endregion
	
	#region プレイヤー選択
	/// <summary>
	/// プレイヤー選択開始
	/// </summary>
	private void SelectPlayerSetup()
	{
		// サーバに選択したプレイヤー情報を送信
		NetworkController.SendSelectPlayer(this.selectPlayerId, this);
		GUIDebugLog.AddMessage("プレイヤー情報取得中・・・");
	}
	
	/// <summary>
	/// プレイヤー選択受信
	/// </summary>
	public void Response(SelectPlayerRes packet)
	{
		if(packet.Result)
		{
			// 成功時はサーバ情報取得へ
			ChangeState(StateType.ServerStatus);
		}
		else
		{
			this.isReconnect = false;

			// 失敗時はタイトル画面へ
			GUISystemMessage.SetModeOK
			(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX047_ReturnTitleInfo),
					NextTitleScene
			);
			GUIDebugLog.AddMessage(string.Format("プレイヤー情報取得失敗 Result={0}={0}", packet.Result));
			BugReportController.SaveLogFile(string.Format("SelectPlayerRes PacketParameterError. Result={0}", packet.Result));
		}
	}
	#endregion
	
	#region サーバ状態取得
	/// <summary>
	/// サーバ状態取得
	/// </summary>
	private void ServerStatusSetup()
	{
		// 再参戦可能かどうかサーバに問い合わせる
		NetworkController.SendServerStatus(this);
		GUIDebugLog.AddMessage("サーバステータス取得中・・・");
	}
	
	public void Response(ServerStatusRes packet)
	{
		var array = packet.GetServerStatusPackets();
		foreach (var v in array)
		{
			switch(v.Status)
			{
				// サーバの状態に問題なし
				case ServerStatus.Ready:
				{
					this.isReconnect = false;

					// バトルが終了しているのでロビーシーンへ
					GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX066_ReConnectBattleEnd_Title), MasterData.GetText(TextType.TX067_ReConnectBattleEnd),
					                           MasterData.GetText(TextType.TX057_Common_YesButton), NextLobby);
					break;
				}
				// 再参戦可能
				case ServerStatus.ReEntry:
				{
					// 再参戦送信
					ChangeState(StateType.ReEntry);
					break;
				}
				// メンテナンス
				case ServerStatus.Maintenance:
				{
					this.isReconnect = false;

					// タイトルシーンへ
					GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX048_MainteTitle),
					                           MasterData.GetText(TextType.TX049_Mainte),
					                           NextTitleScene);
					break;
				}
			}
		}
	}
	#endregion
	
	#region 再参戦
	/// <summary>
	/// 再参戦セットアップ
	/// </summary>
	private void ReEntrySetup()
	{
		// 再参戦を行うのでサーバに送信
		BattlePacket.SendReEnterField(this);
		GUIDebugLog.AddMessage("再参戦情報取得中・・・");
	}

	/// <summary>
	/// 再参戦情報受信
	/// </summary>
	public void Response(ReEnterFieldRes packet)
	{
		if(packet.ReEnterFieldResult == ReEnterFieldResult.Success)
		{
			// 再参戦成功
			GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX064_ReConnectSuccess_Title), MasterData.GetText(TextType.TX065_ReConnectSuccess), ()=>{});
			// ログイン処理終了
			ChangeState(StateType.LoginEnd);
		}
		else if(packet.ReEnterFieldResult == ReEnterFieldResult.BattleEnd)
		{
			this.isReconnect = false;

			// 再参戦終了していた
			// 接続中のメッセージウィンドウを閉じる
			// 再参戦終了していた ロビーシーンへ
			GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX066_ReConnectBattleEnd_Title), MasterData.GetText(TextType.TX067_ReConnectBattleEnd),
			                           MasterData.GetText(TextType.TX057_Common_YesButton), NextLobby);
		}
		else
		{
			this.isReconnect = false;

			// 失敗時はタイトル画面へ
			GUISystemMessage.SetModeOK
			(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX047_ReturnTitleInfo),
					NextTitleScene
			);
			GUIDebugLog.AddMessage(string.Format("再参戦情報取得失敗 ReEnterFieldResult={0}", packet.ReEnterFieldResult));
			BugReportController.SaveLogFile(string.Format("ReEnterFieldRes PacketParameterError. ReEnterFieldResult={0}", packet.ReEnterFieldResult));
		}
	}
	#endregion
	
	#region ログイン終了
	/// <summary>
	/// ログイン終了時処理
	/// </summary>
	private void LoginEndSetup()
	{
		// デバッグログ非表示
		GUIDebugLog.Close();
		// 切断処理削除
		SceneController.RemoveDisconnect(this);
		SceneController.RemoveDisconnectByServer(this);
	}
	#endregion
	
	#region 状態変更
	/// <summary>
	/// 状態の変更を行う
	/// </summary>
	private void ChangeState(StateType nextState)
	{
		switch(nextState)
		{
			case StateType.Connecting:
				ConnectSetup();
				break;
			case StateType.Login:
				LoginSetup();
				break;
			case StateType.RetainPlayer:
				RetainPlayerAllSetup();
				break;
			case StateType.SelectPlayer:
				SelectPlayerSetup();
				break;
			case StateType.ServerStatus:
				ServerStatusSetup();
				break;
			case StateType.ReEntry:
				ReEntrySetup();
				break;
			case StateType.LoginEnd:
				LoginEndSetup();
				break;
		}
		
		this.State = nextState;
	}
	#endregion
	
	#region IOnNetworkDisconnect
	/// <summary>
	/// 再接続中に通信が切れた時に再度接続処理を行うかのフラグ
	/// </summary>
	private bool isReconnect = true;

	public void Disconnect()
	{
		if (!this.isReconnect) { return; }

		if(this.reConnectCount > ReConnectMax)
		{
			// 最大再接続回数を超えた場合は再度再接続を行うかどうかメッセージを表示
			GUISystemMessage.SetModeYesNo
			(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX027_Disconnect),
					MasterData.GetText(TextType.TX159_Common_RetryButton),
					MasterData.GetText(TextType.TX063_ReConnectRetry_ButtonTitle),
					()=>{
						// 再接続開始
						ChangeState(StateType.Connecting);
					},
					()=>{
						// タイトルシーンへ
						NextTitleScene();
					}
			);
		}
		else
		{
			// 再接続開始
			ChangeState(StateType.Connecting);
		}
	}	
	#endregion

	#region IOnNetwrokDisconnectByServer
	public void DisconnectByServer()
	{
		// サーバから強制切断された場合は再接続処理を終了させる
		ChangeState(StateType.LoginEnd);
	}
	#endregion

	#region シーン変更
	/// <summary>
	/// 認証シーンに遷移させる
	/// </summary>
	private void NextAuthScene()
	{
		// ログイン処理終了
		ChangeState(StateType.LoginEnd);

		// BGM停止
		SoundController.StopBGM();
		AuthMain.LoadScene();
	}
	
	/// <summary>
	/// タイトルシーンに遷移させる
	/// </summary>
	private void NextTitleScene()
	{
		// ログイン処理終了
		ChangeState(StateType.LoginEnd);

		if(BattleMain.Instance != null)
		{
			BattleMain.Instance.OnLogout();
		}
	}
	
	/// <summary>
	/// ロビーシーンに遷移させる
	/// </summary>
	private void NextLobby()
	{
		// ログイン処理終了
		ChangeState(StateType.LoginEnd);

		if(BattleMain.Instance != null)
		{
			BattleMain.Instance.NextLobby();
		}
	}
	#endregion
}