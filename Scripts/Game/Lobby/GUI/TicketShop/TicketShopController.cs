/// <summary>
/// チケットショップ制御
/// 
/// 2016/06/15
/// </summary>

using System;
using Asobimo.Purchase;

namespace XUI.TicketShop {

	#region ==== イベント ====

	/// <summary>
	/// チケット購入イベント
	/// </summary>
	public class TicketPurchaseEventArgs : EventArgs {

		public int TicketNo { get; set; }
		public string ProductID { get; set; }
	}

	#endregion ==== イベント ====

	/// <summary>
	/// チケットショップ制御インターフェイス
	/// </summary>
	public interface IController {

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		#endregion ==== アクティブ設定 ====

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
		/// 所持チケット枚数の設定
		/// </summary>
		/// <param name="num"></param>
		void SetTicketNum( int num );

        /// <summary>
        /// 获得角色券总数
        /// </summary>
        /// <returns></returns>
        int GetTicketNum();

		/// <summary>
		/// チケット購入上限の設定
		/// </summary>
		/// <param name="limit"></param>
		void SetTicketPurchaseLimit( int limit );

		/// <summary>
		/// マーケット商品リストの設定
		/// </summary>
		/// <param name="list"></param>
		void SetProductList( ProductData[] list );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// チケットショップ制御
	/// </summary>
	public class Controller : IController {

		#region ==== 文字列 ====

		private string screenTitle = MasterData.GetText( TextType.TX513_TicketShop_Totle );
		private string helpMessage = MasterData.GetText( TextType.TX514_TicketShop_HelpMessage );

		#endregion ==== 文字列 ====

		#region ==== フィールド ====

		// モデル
		private readonly IModel _model;

		// ビュー
		private readonly IView _view;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		// モデル
		private IModel Model { get { return _model; } }

		// ビュー
		private IView View { get { return _view; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate {
			get {
				if( this.Model == null ) return false;
				if( this.View == null ) return false;
				return true;
			}
		}

		#endregion ==== プロパティ ====

		#region ==== 初期化 ====

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller( IModel model, IView view ) {

			if( model == null || view == null ) return;

			// 初期化
			MemberInit();

			// モデル設定
			_model = model;
			Model.OnChangeTicketNum				+= handleChangeTicket;
			Model.OnChangeTicketPurchaseLimit	+= handleChangeTicket;
			Model.OnChangeProductList			+= handleChangeProductList;

			// ビュー設定
			_view = view;
			View.OnHome							+= HandleHome;
			View.OnClose						+= HandleClose;
			View.OnTicketPurchaseButtonEvent	+= handleTicketPurchaseButtonEvent;
			View.OnTermsButtonEvent				+= handleTermsButtonEvent;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			if( !CanUpdate ) return;

			// シリアライズされていないメンバーの初期化
			MemberInit();
		}

		/// <summary>
		/// シリアライズされていないメンバーの初期化
		/// </summary>
		private void MemberInit() {
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			if( View != null ) {
				View.Dispose();
			}
			if( Model != null ) {
				Model.Dispose();
			}

			OnTicketPurchaseButtonEvent = null;
			OnTermsButtonEvent = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( this.CanUpdate ) {
				// データ初期化
				this.Model.TicketNum = 0;
				this.Model.TicketPurchaseLimit = 0;

				// アクティブ設定
				this.View.SetActive( isActive, isTweenSkip );

				// その他UIの表示設定
				GUILobbyResident.SetActive( !isActive );
				GUIScreenTitle.Play( isActive, screenTitle );
				GUIHelpMessage.Play( isActive, helpMessage );
			}
		}

		#endregion ==== アクティブ設定 ====

		#region ==== ホーム、閉じるボタンイベント ====

		void HandleHome( object sender, EventArgs e ) {

			GUIController.Clear();
		}

		void HandleClose( object sender, EventArgs e ) {

			GUIController.Back();
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
		/// 所持チケット枚数の設定
		/// </summary>
		/// <param name="num"></param>
		public void SetTicketNum( int num ) {

			Model.TicketNum = num;
		}

        /// <summary>
        /// 获得角色券总数
        /// </summary>
        /// <returns></returns>
        public int GetTicketNum()
        {
            return Model.TicketNum;
        }

		/// <summary>
		/// チケット購入上限数の設定
		/// </summary>
		/// <param name="limit"></param>
		public void SetTicketPurchaseLimit( int limit ) {

			Model.TicketPurchaseLimit = limit;
		}

		/// <summary>
		/// マーケット商品リストの設定
		/// </summary>
		/// <param name="list"></param>
		public void SetProductList( ProductData[] list ) {

			Model.ProductList = list;
		}

		/// <summary>
		/// チケット購入ボタンイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleTicketPurchaseButtonEvent( object sender, TicketPurchaseEventArgs args ) {

			if( Model.ProductList == null ) return;

			// プロダクトIDの設定
			if (Model.ProductList[args.TicketNo] != null)
			{
				args.ProductID = Model.ProductList[args.TicketNo].ProductId;
			}

			OnTicketPurchaseButtonEvent( sender, args );
		}

		/// <summary>
		/// 規約ボタンイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleTermsButtonEvent( object sender, EventArgs args ) {

			OnTermsButtonEvent( sender, args );
		}

		/// <summary>
		/// 所持チケットが変化した時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeTicket( object sender, EventArgs args ) {

			View.ChangeTicket( Model.TicketPurchaseLimit, Model.TicketNum );
		}

		/// <summary>
		/// マーケット商品リストが変化した時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeProductList( object sender, EventArgs args ) {

			View.ChangeProductList( Model.ProductList );
		}

		#endregion ==== アクション ====
	}
}
