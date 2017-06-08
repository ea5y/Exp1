/// <summary>
/// キャラクターショップ表示
/// 
/// 2016/06/15
/// </summary>

using System;
using UnityEngine;

namespace XUI.CharaShop {

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

		#region ==== アクション ====

		/// <summary>
		/// 所持チケット枚数設定
		/// </summary>
		/// <param name="num"></param>
		void SetTicketNum( int num );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// ビュー
	/// </summary>
	public class CharaShopView : GUIScreenViewBase, IView {

		#region ==== フィールド ====

		/// <summary>
		/// 所持チケット枚数ラベル
		/// </summary>
		[SerializeField]
		private UILabel ticketNumLabel = null;

		/// <summary>
		/// 所持チケット枚数フォーマット
		/// </summary>
		[SerializeField]
		private string ticketNumFormat = "{0}";

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

		#region ==== アクション ====

		/// <summary>
		/// 所持チケット枚数の設定
		/// </summary>
		/// <param name="num"></param>
		public void SetTicketNum( int num ) {

			ticketNumLabel.text = string.Format( ticketNumFormat, num );
		}

		#endregion ==== アクション ====
	}
}
