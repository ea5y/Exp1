/// <summary>
/// チケットショップ表示
/// 
/// 2016/06/15
/// </summary>

using System;
using UnityEngine;
using Asobimo.Purchase;
using Scm.Common.XwMaster;

namespace XUI.TicketShop {

	/// <summary>
	/// チケットショップ表示インターフェイス
	/// </summary>
	public interface IView {

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
		/// チケット購入ボタンイベント
		/// </summary>
		event EventHandler<TicketPurchaseEventArgs> OnTicketPurchaseButtonEvent;

		/// <summary>
		/// 規約ボタンイベント
		/// </summary>
		event EventHandler<EventArgs> OnTermsButtonEvent;

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// チケット変更設定
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="num"></param>
		void ChangeTicket( int limit, int num );

		/// <summary>
		/// マーケット商品設定
		/// </summary>
		/// <param name="list"></param>
		void ChangeProductList( ProductData[] list );

		#endregion ==== アクション ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====
	}

	/// <summary>
	/// チケットショップ表示
	/// </summary>
	public class TicketShopView : GUIScreenViewBase, IView {

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
		/// チケット購入ボタン
		/// </summary>
		[SerializeField]
		private UIButton[] ticketPurchaseButton = null;

		/// <summary>
		/// チケット購入ボタンラベル
		/// </summary>
		[SerializeField]
		private UILabel[] ticketPurchaseButtonLabel = null;

		/// <summary>
		/// チケット購入ボタン価格ラベル
		/// </summary>
		[SerializeField]
		private UILabel[] ticketPurchaseButtonPriceLabel = null;

		/// <summary>
		/// チケット購入ボタン購入済みフィルター
		/// </summary>
		[SerializeField]
		private UISprite[] ticketPurchaseButtonSprite = null;

        /// <summary>
        /// 购买次数限制
        /// </summary>
        [SerializeField]
        private UILabel[] ticketPurchaseButtonBuyLimitLabel = null;

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

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );
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
		/// チケット購入ボタンイベント
		/// </summary>
		public event EventHandler<TicketPurchaseEventArgs> OnTicketPurchaseButtonEvent = ( sender, e ) => { };

		/// <summary>
		/// 規約ボタンイベント
		/// </summary>
		public event EventHandler<EventArgs> OnTermsButtonEvent = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// チケット変更設定
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="num"></param>
		public void ChangeTicket( int limit, int num ) {

			bool check = false;
			bool active = false;
			int count = 0;
			int loop = 0;
			ShopTicketMasterData data = null;

			// チケット設定
			for( loop = 0 ; loop < ticketPurchaseButton.Length ; loop++ ) {

				// ボタン数以上は無視
				if( loop >= ticketPurchaseButton.Length )	break;

				// 購入可能かどうかのチェック
				active = MasterData.TryGetTicket( loop + 1, out data );
				if( active ) {
					// 枚数設定
					count = data.Num;

					// 上限に達していないかチェック
					check = ( limit >= count );

				} else {
					// ボタン非表示
					check = true;
				}
                active = true;
                check = true;
				// チケット購入ボタン有効設定
				ticketPurchaseButton[loop].gameObject.SetActive( active );
				//ticketPurchaseButton[loop].isEnabled = check;
				//ticketPurchaseButtonSprite[loop].gameObject.SetActive( !check );
			}

			// 所持チケット枚数ラベル変更
			ticketNumLabel.text = string.Format( ticketNumFormat, System.Math.Max(num, 0) );

			// チケット購入可能枚数ラベル変更
			ticketPurchaseLimitLabel.text = string.Format( ticketPurchaseLimitFormat, System.Math.Max(limit, 0) );
		}

		/// <summary>
		/// チケット購入
		/// </summary>
		/// <param name="no"></param>
		private void ticketPurchase( int no ) {

			// イベント引数設定
			TicketPurchaseEventArgs args = new TicketPurchaseEventArgs() {
				TicketNo = no,
				ProductID = ""
			};

			OnTicketPurchaseButtonEvent( this, args );
		}

		/// <summary>
		/// マーケット商品リスト設定
		/// </summary>
		/// <param name="list"></param>
		public void ChangeProductList( ProductData[] list ) {

			if( list == null ) return;

			// チケット設定
			for( int loop = 0 ; loop < ticketPurchaseButtonLabel.Length ; loop++ ) {
				// 配列チェック
				if( loop >= list.Length ) break;
				// null チェック
				if (list[loop] == null) break;

				// チケット購入ボタン設定

				// ItemNameだとGooglePlayDeveloperConsoleで設定しているタイトル名が
				// 「アプリ内アイテムのタイトル名」＋「ストア掲載情報のタイトル名」で取得できてしまうので
				// Description の方をゲーム側は表示するようにする
				// ItemName = 「アプリ内アイテムのタイトル名」(「ストア掲載情報のタイトル名」)
				// という形で取得される
				//ticketPurchaseButtonLabel[loop].text = list[loop].ItemName;
				ticketPurchaseButtonLabel[loop].text = list[loop].Description;

				ticketPurchaseButtonPriceLabel[loop].text = list[loop].PriceString;
                ticketPurchaseButtonBuyLimitLabel[loop].text = "今日可购买/已购买:" + list[loop].DailyLimit + "/" + list[loop].TodayPaidCount +"次";
                ticketPurchaseButtonSprite[loop].gameObject.SetActive(list[loop].DailyLimit <= list[loop].TodayPaidCount);
                ticketPurchaseButton[loop].isEnabled = list[loop].DailyLimit > list[loop].TodayPaidCount;
			}
		}

		#endregion ==== アクション ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// チケット購入ボタンクリック
		/// </summary>
		public void OnPurchaseClick_01() {
			ticketPurchase( 0 );
		}
		public void OnPurchaseClick_02() {
			ticketPurchase( 1 );
		}
		public void OnPurchaseClick_03() {
			ticketPurchase( 2 );
		}
		public void OnPurchaseClick_04() {
			ticketPurchase( 3 );
		}
		public void OnPurchaseClick_05() {
			ticketPurchase( 4 );
		}


		/// <summary>
		/// 規約ボタンクリック
		/// </summary>
		public void OnTermsButtonClick() {

			OnTermsButtonEvent( this, EventArgs.Empty );
		}

		#endregion ==== NGUIリフレクション ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			OnHome = null;
			OnClose = null;
			OnTicketPurchaseButtonEvent = null;
			OnTermsButtonEvent = null;
		}

		#endregion ==== 破棄 ====
	}
}