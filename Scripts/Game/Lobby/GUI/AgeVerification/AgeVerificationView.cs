/// <summary>
/// 年齢認証画面
/// 
/// 2016/06/27
/// </summary>

using UnityEngine;
using System;

namespace XUI.AgeVerification {

	/// <summary>
	/// インターフェイス
	/// </summary>
	public interface IView {

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion ==== アクティブ ====

		#region ==== ホーム、閉じるボタンイベント ====

		event EventHandler<EventArgs> OnHome;
		event EventHandler<EventArgs> OnClose;

		#endregion ==== ホーム、閉じるボタンイベント ====

		#region ==== イベント ====

		/// <summary>
		/// ショップオープンイベント
		/// </summary>
		event EventHandler<EventArgs> OnOpenShop;

		/// <summary>
		/// スキップ設定イベント
		/// </summary>
		event EventHandler<ToggleEventArgs> OnSkipAgeVerification;

		#endregion ==== イベント ====

		#region ==== アクション ====

		#endregion ==== アクション ====
	}

	/// <summary>
	/// ビュー
	/// </summary>
	public class AgeVerificationView : GUIScreenViewBase, IView {

		#region ==== フィールド ====

		/// <summary>
		/// 年齢確認画面
		/// </summary>
		[SerializeField]
		private GameObject ageVerification = null;

		/// <summary>
		/// 年齢認証チェックボックス
		/// </summary>
		[SerializeField]
		private UIToggle ageVerificationCheck = null;

		/// <summary>
		/// 年齢最終確認画面
		/// </summary>
		[SerializeField]
		private GameObject fullAgeVerification = null;

		/// <summary>
		/// 保護者確認画面
		/// </summary>
		[SerializeField]
		private GameObject nonAgeVerification = null;

		#endregion ==== フィールド ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			this.OnHome = null;
			this.OnClose = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );

			// 画面を開く
			setViewMode( isActive? ViewMode.AgeVerification : ViewMode.None );

			// チェックボックス設定
			ageVerificationCheck.value = ConfigFile.Instance.Data.System.AgeVerificationSkip;
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState() {

			return this.GetRootActiveState();
		}

		#endregion ==== アクティブ ====

		#region ==== ホーム、閉じるボタンイベント ====

		public event EventHandler<EventArgs> OnHome = ( sender, e ) => { };
		public event EventHandler<EventArgs> OnClose = ( sender, e ) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent() {

			// 通知
			this.OnHome( this, EventArgs.Empty );
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent() {

			// 通知
			this.OnClose( this, EventArgs.Empty );
		}

		#endregion ==== ホーム、閉じるボタンイベント ====

		#region ==== イベント ====

		/// <summary>
		/// ショップオープンイベント
		/// </summary>
		public event EventHandler<EventArgs> OnOpenShop = ( sender, args ) => { };

		/// <summary>
		/// スキップ設定イベント
		/// </summary>
		public event EventHandler<ToggleEventArgs> OnSkipAgeVerification = ( sender, args ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// 指定画面を表示する
		/// </summary>
		/// <param name="mode"></param>
		private void setViewMode( ViewMode mode ) {

			ageVerification.SetActive( false );
			fullAgeVerification.SetActive( false );
			nonAgeVerification.SetActive( false );

			switch( mode ) {
				case ViewMode.None:	break;
				case ViewMode.AgeVerification:		ageVerification.SetActive( true );		break;
				case ViewMode.FullAgeVerification:	fullAgeVerification.SetActive( true );	break;
				case ViewMode.NonAgeVerification:	nonAgeVerification.SetActive( true );	break;
			}
		}

		#endregion ==== アクション ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// 20歳以上ボタン
		/// </summary>
		public void OnOver20YearsOldButton() {

			// 年齢最終確認画面を開く
			setViewMode( ViewMode.FullAgeVerification );
		}

		/// <summary>
		/// 19歳以下ボタン
		/// </summary>
		public void OnUnder19YearsOldButton() {

			// スキップフラグをOFFにする
			ageVerificationCheck.value = false;

			// 変更を通知
			OnSkipAgeVerificationCheck();

			// 保護者確認画面を開く
			setViewMode( ViewMode.NonAgeVerification );
		}

		/// <summary>
		/// 年齢確認Yesボタン
		/// </summary>
		public void OnAgeVerificationYesButton() {

			// ショップへ移動
			OnOpenShop( this, EventArgs.Empty );
		}

		/// <summary>
		/// 年齢確認Noボタン
		/// </summary>
		public void OnAgeVerificationNoButton() {

			// 年齢確認画面へ移動
			setViewMode( ViewMode.AgeVerification );
		}

		/// <summary>
		/// 決済承諾ボタン
		/// </summary>
		public void OnSettlementConsentButton() {

			// ショップへ移動
			OnOpenShop( this, EventArgs.Empty );
		}

		/// <summary>
		/// 決済拒否ボタン
		/// </summary>
		public void OnSettlementRejectionButton() {

			// 画面を閉じる
			OnCloseEvent();
		}

		/// <summary>
		/// 年齢認証スキップチェック
		/// </summary>
		public void OnSkipAgeVerificationCheck() {

			// イベント作成
			ToggleEventArgs args = new ToggleEventArgs() {
				Toggle = ageVerificationCheck.value
			};

			OnSkipAgeVerification( this, args );
		}

		#endregion ==== NGUIリフレクション ====
	}
}