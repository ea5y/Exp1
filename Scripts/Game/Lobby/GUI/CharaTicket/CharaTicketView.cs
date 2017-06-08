/// <summary>
/// キャラクターチケット表示
/// 
/// 2016/06/14
/// </summary>

using System;
using UnityEngine;

namespace XUI.CharaTicket {

	/// <summary>
	/// キャラクターチケット表示インターフェイス
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
		/// キャラクター交換ボタンイベント
		/// </summary>
		event EventHandler<EventArgs> OnCharaExchange;

		/// <summary>
		/// キャラクターチケット購入ボタンイベント
		/// </summary>
		event EventHandler<EventArgs> OnCharaTicketPurchase;

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// チケット変更時の設定
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="num"></param>
		void ChangeTicket( int limit, int num );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// キャラクターチケット表示
	/// </summary>
	public class CharaTicketView : GUIScreenViewBase, IView {

		#region ==== フィールド ====

		/// <summary>
		/// 所持チケットラベル
		/// </summary>
		[SerializeField]
		UILabel ticketNumLabel = null;

		/// <summary>
		/// 購入上限ラベル
		/// </summary>
		[SerializeField]
		UILabel ticketPurchaseLimitLabel = null;

		/// <summary>
		/// キャラクター交換ボタン
		/// </summary>
		[SerializeField]
		private UIButton CharaExchangeButton = null;

		/// <summary>
		/// キャラクターチケット購入ボタン
		/// </summary>
		[SerializeField]
		private UIButton CharaTicketPurchaseButton = null;

		/// <summary>
		/// チケット購入無効
		/// </summary>
		[SerializeField]
		private GameObject buyingTicketDisable = null;

		/// <summary>
		/// キャラクター交換無効
		/// </summary>
		[SerializeField]
		private GameObject charaExchangeDisable = null;

		/// <summary>
		/// 所持チケット枚数フォーマット
		/// </summary>
		[SerializeField]
		private string ticketNumFormat = "{0}";

		/// <summary>
		/// チケット購入上限フォーマット
		/// </summary>
		[SerializeField]
		private string ticketPurchaseLimitFormat = "{0}";

		#endregion ==== フィールド ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			this.OnHome = null;
			this.OnClose = null;
			this.OnCharaExchange = null;
			this.OnCharaTicketPurchase = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );

			// ボタンを無効化
			CharaExchangeButton.isEnabled = true;
			CharaTicketPurchaseButton.isEnabled = true;
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
		/// キャラクター交換ボタンイベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaExchange = ( sender, e ) => { };

		/// <summary>
		/// キャラクターチケット購入ボタンイベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaTicketPurchase = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// チケット変更時の設定
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="num"></param>
		public void ChangeTicket( int limit, int num ) {

            bool charaEnabled = true;// (num > 0);
            bool limitEnabled = true;// (limit > 0);

			// キャラクター交換ボタン有効設定
			CharaExchangeButton.isEnabled = charaEnabled || limitEnabled;

			// キャラクター交換無効設定
			charaExchangeDisable.SetActive( !limitEnabled && !charaEnabled );

			// 所持チケット枚数ラベル変更
			ticketNumLabel.text = string.Format( ticketNumFormat, System.Math.Max(num, 0) );



			// チケット購入ボタン有効設定
			CharaTicketPurchaseButton.isEnabled = limitEnabled;

			// チケット購入無効設定
			buyingTicketDisable.SetActive( !limitEnabled );

			// チケット購入制限ラベル変更
			ticketPurchaseLimitLabel.text = string.Format( ticketPurchaseLimitFormat, System.Math.Max(limit, 0) );
		}

		#endregion ==== アクション ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// キャラクター交換ボタンクリック
		/// </summary>
		public void OnCharaExchangeButtonClick() {

			OnCharaExchange( this, EventArgs.Empty );
		}

		/// <summary>
		/// キャラクターチケット購入ボタンクリック
		/// </summary>
		public void OnCharaTicketPurchaseButtonClick() {

			OnCharaTicketPurchase( this, EventArgs.Empty );
		}

		#endregion ==== NGUIリフレクション ====
	}
}
