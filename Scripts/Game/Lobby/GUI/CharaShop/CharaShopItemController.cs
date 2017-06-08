/// <summary>
/// キャラクターショップアイテム制御
/// 
/// 2016/06/15
/// </summary>

using System;

namespace XUI.CharaShopItem {

	#region ==== イベント ====

	/// <summary>
	/// チケット交換イベント
	/// </summary>
	public class TicketExchangeEventArgs : EventArgs {

		public int ProductID { get; set; }
		public string Name { get; set; }
	}

	#endregion ==== イベント ====

	/// <summary>
	/// インターフェイス
	/// </summary>
	public interface IController {

		#region ==== イベント ====

		/// <summary>
		/// チケット交換ボタンイベント
		/// </summary>
		event EventHandler<TicketExchangeEventArgs> OnTicketExchangeButtonEvent;

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		void SetActive( bool isActive );

		#endregion ==== アクティブ ====

		#region ==== アクション ====

		/// <summary>
		/// ショップ情報のセット
		/// </summary>
		/// <param name="info"></param>
		void SetShopInfo( CharaShopItemInfo info );

		/// <summary>
		/// ショップデータの同期
		/// </summary>
		void UpdateShopInfo();

		#endregion ==== アクション ====
	}

	/// <summary>
	/// コントローラ
	/// </summary>
	public class Controller : IController {

		#region ==== フィールド ====

		private readonly IModel model;

		private readonly IView view;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }

		private bool CanUpdate {
			get {
				if( this.Model == null ) return false;
				if( this.View == null ) return false;
				return true;
			}
		}
		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// チケット交換ボタンイベント
		/// </summary>
		public event EventHandler<TicketExchangeEventArgs> OnTicketExchangeButtonEvent = ( sender, e ) => { };

		/// <summary>
		/// 交換ボタンが押下された時
		/// </summary>
		private void HandleOnButtonClick( object sender, EventArgs e ) {

			// 交換するキャラクターの商品IDを設定
			TicketExchangeEventArgs args = new TicketExchangeEventArgs() {
				ProductID = Model.Info.ProductID,
				Name = Model.Info.CharaName
			};

			OnTicketExchangeButtonEvent( sender, args );
		}

		/// <summary>
		/// ショップ情報が更新された時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handleShopInfoChange( object sender, EventArgs e ) {

			SyncShopInfo();
		}

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model"></param>
		/// <param name="view"></param>
		public Controller( IModel model, IView view ) {

			if( model == null || view == null ) return;

			// ビュー設定
			this.view = view;
			View.OnTicketExchangeButtonEvent += HandleOnButtonClick;

			// モデル設定
			this.model = model;
			Model.OnCharaShopInfoChange += handleShopInfoChange;
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup() {

			if( !CanUpdate ) return;

			Model.Setup();
			View.Setup();
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			if( Model != null ) {
				Model.Dispose();
			}

			OnTicketExchangeButtonEvent = null;
		}
		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		public void SetActive( bool isActive ) {

			if( !CanUpdate ) return;

			this.View.SetActive( isActive, true );
		}

		#endregion ==== アクティブ ====

		#region ==== アクション ====

		/// <summary>
		/// ショップ情報のセット
		/// </summary>
		/// <param name="info"></param>
		public void SetShopInfo( CharaShopItemInfo info ) {

			if( !CanUpdate ) return;

			Model.Info = info;
		}

		/// <summary>
		/// ショップ情報の同期
		/// </summary>
		public void UpdateShopInfo() {

			SyncShopInfo();
		}

		/// <summary>
		/// データをビューに反映
		/// </summary>
		private void SyncShopInfo() {

			if( !CanUpdate ) return;
			if( Model.Info == null ) return;

			View.SetCharaShopInfo( Model.Info );
		}

		#endregion ==== アクション ====
	}
}
