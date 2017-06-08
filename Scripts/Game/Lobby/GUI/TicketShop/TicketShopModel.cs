/// <summary>
/// チケットショップデータ
/// 
/// 2016/06/15
/// </summary>

using System;
using Asobimo.Purchase;

namespace XUI.TicketShop {

	/// <summary>
	/// チケットショップデータインターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// 所持チケット枚数
		/// </summary>
		int TicketNum { get; set; }

		/// <summary>
		/// チケット購入上限数
		/// </summary>
		int TicketPurchaseLimit { get; set; }

		/// <summary>
		/// マーケット商品リスト
		/// </summary>
		ProductData[] ProductList { get; set; }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// 所持チケット枚数更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnChangeTicketNum;

		/// <summary>
		/// チケット購入上限更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnChangeTicketPurchaseLimit;

		/// <summary>
		/// マーケット商品リスト更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnChangeProductList;

		#endregion ==== イベント ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====
	}

	/// <summary>
	/// チケットショップデータ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private int ticketNum = 0;
		private int ticketPurchaseLimit = 0;
		private ProductData[] productList = null;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// 所持チケット枚数
		/// </summary>
		public int TicketNum {
			get {
				return ticketNum;
			}
			set {
				ticketNum = value;

				OnChangeTicketNum( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// チケット購入上限
		/// </summary>
		public int TicketPurchaseLimit {
			get {
				return ticketPurchaseLimit;
			}
			set {
				ticketPurchaseLimit = value;

				OnChangeTicketPurchaseLimit( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// マーケット商品リスト
		/// </summary>
		public ProductData[] ProductList {
			get {
				return productList;
			}
			set {
				productList = value;

				OnChangeProductList( this, EventArgs.Empty );
			}
		}

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// 所持チケット枚数更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnChangeTicketNum = ( sender, args ) => { };

		/// <summary>
		/// チケット購入上限更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnChangeTicketPurchaseLimit = ( sender, args ) => { };

		/// <summary>
		/// マーケット商品リスト更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnChangeProductList = ( sender, args ) => { };

		#endregion ==== イベント ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			OnChangeTicketNum = null;
			OnChangeTicketPurchaseLimit = null;
			OnChangeProductList = null;
		}

		#endregion ==== 破棄 ====
	}
}