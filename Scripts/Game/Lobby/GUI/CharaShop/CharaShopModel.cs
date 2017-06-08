/// <summary>
/// キャラクターショップデータ
/// 
/// 2016/06/15
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

namespace XUI.CharaShop {

	/// <summary>
	/// インターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// 全リスト
		/// </summary>
		List<CharaShopItemInfo> InfoList { get; set; }

		/// <summary>
		/// 所持チケット枚数
		/// </summary>
		int TicketNum { get; set; }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// リスト更新通知
		/// </summary>
		event EventHandler<EventArgs> OnChangeInfoList;

		/// <summary>
		/// 所持チケット枚数の変更を通知
		/// </summary>
		event EventHandler<EventArgs> OnChangeTicketNum;

		#endregion ==== イベント ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクション ====

		/// <summary>
		/// 指定範囲の情報を取得
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		List<CharaShopItemInfo> GetTabList( int start, int count );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// データ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private List<CharaShopItemInfo> infoList = null;
		private int ticketNum = 0;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// 全リスト
		/// </summary>
		public List<CharaShopItemInfo> InfoList {
			get {
				return infoList;
			}
			set {
				infoList = value;

				// リスト更新通知
				OnChangeInfoList( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// 所持チケット枚数
		/// </summary>
		public int TicketNum {
			get {
				return ticketNum;
			}
			set {
				ticketNum = value;

				// 所持チケット枚数の変更を通知
				OnChangeTicketNum( this, EventArgs.Empty );
			}
		}

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// リスト更新通知
		/// </summary>
		public event EventHandler<EventArgs> OnChangeInfoList = ( sender, args ) => { };

		/// <summary>
		/// 所持チケット枚数の変更を通知
		/// </summary>
		public event EventHandler<EventArgs> OnChangeTicketNum = ( sender, args ) => { };

		#endregion ==== イベント ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			OnChangeTicketNum = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクション ====

		/// <summary>
		/// 指定範囲の情報を取り出す
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<CharaShopItemInfo> GetTabList( int start, int count ) {

			int end = start + count;
			List<CharaShopItemInfo> infos = new List<CharaShopItemInfo>();

			infos.AddRange( infoList.Where( x => x.Index >= start && x.Index < end ) );

			return infos;
		}

		#endregion ==== アクション ====
	}
}