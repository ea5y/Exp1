/// <summary>
/// タイトル制御
/// 
/// 2015/12/14
/// </summary>
using UnityEngine;
using System.Collections;
using System;

namespace XUI
{
	namespace Title
	{
		/// <summary>
		/// タイトル制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 開始時状態のタイトルを開く
			/// </summary>
			void OpenStartTitle();

			/// <summary>
			/// 情報状態のタイトルを開く
			/// </summary>
			void OpenInfo();

			/// <summary>
			/// 閉じる
			/// </summary>
			void Close();
		}

		/// <summary>
		/// タイトル制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド&プロパティ
			/// <summary>
			/// タイトルの状態
			/// </summary>
			public enum State
			{
				None,
				StartTitle,
				Info,
				Connect,
			}
			private State state = State.None;

			/// <summary>
			/// モデル
			/// </summary>
			private readonly IModel model = null;

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView view = null;
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="model"></param>
			/// <param name="view"></param>
			public Controller(IModel model, IView view)
			{
				this.model = model;
				this.view = view;

				// イベント登録
				this.view.OnTouchChangeStateEvent += OnTouchChangeState;
				this.view.OnEnqueteEvent += OnEnquetePage;
				this.view.OnHomePageEvent += OnHomePage;
				this.view.OnTwitterPageEvent += OnTwitterPage;
				this.view.OnNewsPageEvent += OnNewsPage;
				this.view.OnShopPageEvent += OnShopPage;
				this.view.OnGoOnePageEvent += OnGoOnePage;
				this.view.OnAsobimoAccountEvent += OnAsobimoAccount;
				this.view.OnInquiryEvent += OnInquiry;

				// アプリバージョンセット
				this.view.AppVersion = this.model.AppVersion;

				// 生成時はタイトルロゴ状態にする
				ChangeState(State.StartTitle);

				// お知らせメッセージセット
				this.view.SetNewsMessage(MasterData.GetText(TextType.TX50_Title_News));
			}
			#endregion

			#region 開く
			/// <summary>
			/// ロゴ状態のタイトルを開く
			/// </summary>
			public void OpenStartTitle()
			{
				// タイトルの状態をロゴ表示状態にする
				ChangeState(State.StartTitle);
			}

			/// <summary>
			/// 情報状態のタイトルを開く
			/// </summary>
			public void OpenInfo()
			{
				ChangeState(State.Info);
			}
			#endregion

			#region 閉じる
			/// <summary>
			/// タイトルを閉じる
			/// </summary>
			public void Close()
			{
				// タイトルの状態を無効にする
				ChangeState(State.None);
			}
			#endregion

			#region タイトルの状態変更
			/// <summary>
			/// タイトルの状態を変更させる
			/// </summary>
			/// <param name="nextState"></param>
			private void ChangeState(State nextState)
			{
				switch (nextState)
				{
					case State.StartTitle:
						ChangeStartTitle();
						break;
					case State.Info:
						ChangeInfo();
						break;
					case State.Connect:
						ChangeConnect();
						break;
					case State.None:
					default:
						this.view.Close();
						break;
				}

				this.state = nextState;
			}
			#endregion

			#region 開始時のタイトル
			/// <summary>
			/// 開始時のタイトルを表示させる
			/// </summary>
			private void ChangeStartTitle()
			{
				// 開始時のタイトルとロゴを表示
				this.view.CloseInfo();
				this.view.OpenStartTitle();
				this.view.OpenLogo();
			}
			#endregion

			#region 情報
			/// <summary>
			/// タイトル情報を表示
			/// </summary>
			private void ChangeInfo()
			{
				this.view.CloseStartTitle();
				this.view.OpenInfo();
				this.view.OpenLogo();

				// お知らせページを強制表示
				GUIWebView.OpenNewsStartAppOnly();
			}
			#endregion

			#region サーバ接続
			/// <summary>
			/// サーバ接続開始
			/// </summary>
			private void ChangeConnect()
			{
				// ビュー閉じる
				this.view.Close();

				// WebView閉じる
				GUIWebView.Close();

				// ゲームにログイン開始
				var gameLoginController = new GameLoginController(null);
				FiberController.AddFiber(gameLoginController.Execute().GetEnumerator());

				// Xigncode対処処理版(Xigncode失敗した場合は認証シーンに飛ばすようにしたので必要なくなった)
				//this.Connect();
			}

			/// <summary>
			/// 接続処理
			/// </summary>
			private void Connect()
			{
				FiberController.AddFiber(this.ConnectExecute());
			}

			/// <summary>
			/// サーバ接続実行処理
			/// 認証処理を通してからゲームログイン処理を行う
			/// </summary>
			private IEnumerator ConnectExecute()
			{
				// 認証開始
				bool isAuthSuccess = false;
				AuthRequest.StartRequest();
				while(true)
				{
					if (AuthRequest.State >= AuthRequest.AuthState.Fail)
					{
						// 失敗
						isAuthSuccess = false;
						break;
					}
					else if( AuthRequest.State == AuthRequest.AuthState.Complete)
					{
						// 成功
						isAuthSuccess = true;
						break;
					}
				}

				if(!isAuthSuccess)
				{
					// 認証失敗時はタイトル情報表示に戻る
					this.ChangeState(State.Info);
					yield return null;
				}

				// ゲームにログイン開始
				var gameLoginController = new GameLoginController(this.GameLoginFinished);
				FiberController.AddFiber(gameLoginController.Execute().GetEnumerator());

				yield return null;
			}

			/// <summary>
			/// ゲームログイン終了時処理
			/// </summary>
			private void GameLoginFinished(GameLoginController.ErrorType errorState)
			{
				// サーバからXigncode失敗が返ってきていたら強制認証処理を行い再度ゲームログイン処理を行う
				if(errorState == GameLoginController.ErrorType.Xigncode)
				{
					FiberController.AddFiber(this.AuthForceConnectExecute());
				}
			}

			/// <summary>
			/// 強制認証処理と接続処理を行う
			/// </summary>
			private IEnumerator AuthForceConnectExecute()
			{
				// 認証開始
				bool isAuthSuccess = false;
				AuthRequest.StartForceRequest();
				while(true)
				{
					if (AuthRequest.State >= AuthRequest.AuthState.Fail)
					{
						// 失敗
						isAuthSuccess = false;
						break;
					}
					else if( AuthRequest.State == AuthRequest.AuthState.Complete)
					{
						// 成功
						isAuthSuccess = true;
						break;
					}
				}

				if (!isAuthSuccess)
				{
					// 認証失敗時はタイトル情報表示に戻る
					this.ChangeState(State.Info);
					yield return null;
				}

				// ゲームにログイン開始
				var gameLoginController = new GameLoginController(this.GameLoginFinished);
				FiberController.AddFiber(gameLoginController.Execute().GetEnumerator());

				yield return null;
			}
			#endregion

			#region イベント処理
			/// <summary>
			/// 遷移変更が押された時
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public void OnTouchChangeState(object sender, EventArgs e)
			{
				if (this.state == State.None) { return; }
				// 現在の状態から次の状態へ遷移させる
			    if (this.state == State.StartTitle || this.state == State.Info)
			    {
                    SoundController.PlaySe(SoundController.SeID.LockOn);
			    }
				ChangeState(this.state + 1);
			}

			/// <summary>
			/// アンケートページボタンが押された時
			/// </summary>
			public void OnEnquetePage(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenEnquete();
			}

			/// <summary>
			/// 公式ページボタンが押された時
			/// </summary>
			public void OnHomePage(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenHome();
			}

			/// <summary>
			/// ツイッターページボタンが押された時
			/// </summary>
			public void OnTwitterPage(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenTwitter();
			}

			/// <summary>
			/// お知らせページボタンが押された時
			/// </summary>
			public void OnNewsPage(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenNews();
			}

			/// <summary>
			/// ショップページボタンが押された時
			/// </summary>
			public void OnShopPage(object sender, EventArgs e) { }

			/// <summary>
			/// GoOneページボタンが押された時
			/// </summary>
			public void OnGoOnePage(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenGoOne();
			}

			/// <summary>
			/// アソビモアカウントボタンが押された時
			/// </summary>
			public void OnAsobimoAccount(object sender, EventArgs e)
			{
                AuthEntry.Instance.AuthMethod.ShowMenu();
            }

			/// <summary>
			/// お問い合わせボタンが押された時
			/// </summary>
			public void OnInquiry(object sender, EventArgs e)
			{
				// WebView表示
				GUIWebView.OpenContact();
			}
			#endregion
		}
	}
}