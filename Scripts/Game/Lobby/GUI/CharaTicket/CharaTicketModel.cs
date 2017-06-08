/// <summary>
/// キャラクターチケットデータ
/// 
/// 2016/06/14
/// </summary>

using System;
using System.Collections.Generic;

namespace XUI.CharaTicket {

	/// <summary>
	/// キャラクターチケットデータインターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// チケット所持数
		/// </summary>
		int TicketNum { get; set; }

		/// <summary>
		/// チケット購入上限
		/// </summary>
		int TicketPurchaseLimit { get; set; }

		#endregion ==== プロパティ ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// 所持チケット数が変化した時
		/// </summary>
		event EventHandler<EventArgs> OnChangeTicketNum;

		/// <summary>
		/// 購入上限が変化した時
		/// </summary>
		event EventHandler<EventArgs> OnChangeTicketPurchaseLimit;

		#endregion ==== イベント ====
	}

	/// <summary>
	/// キャラクターチケットデータ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private int ticketNum = 0;
		private int ticketPurchaseLimit = 0;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// チケット所持数
		/// </summary>
		public int TicketNum {
			get {
				return ticketNum;
			}
			set {
				ticketNum = value;

				// 所持枚数変更を通知
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

				// チケット上限変更を通知
				OnChangeTicketPurchaseLimit( this, EventArgs.Empty );
			}
		}

		#endregion ==== プロパティ =====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			OnChangeTicketNum = null;
			OnChangeTicketPurchaseLimit = null;
		}

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// 所持チケット数が変化した時
		/// </summary>
		public event EventHandler<EventArgs> OnChangeTicketNum = ( sender, args ) => { };

		/// <summary>
		/// 購入上限が変化した時
		/// </summary>
		public event EventHandler<EventArgs> OnChangeTicketPurchaseLimit = ( sender, args ) => { };

		#endregion ==== イベント ====
	}

	/// <summary>
	/// チケット情報
	/// </summary>
	public class TicketInfo {

		public int id;
		public string name;
		public string info;
	}
}
