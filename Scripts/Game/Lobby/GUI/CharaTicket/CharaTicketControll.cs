/// <summary>
/// キャラクターチケット制御
/// 
/// 2016/06/14
/// </summary>

using System;

namespace XUI.CharaTicket {

	/// <summary>
	/// キャラクターチケット制御インターフェイス
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
		/// キャラクター交換ボタンイベント
		/// </summary>
		event EventHandler<EventArgs> OnCharaExchange;

		/// <summary>
		/// キャラクターチケット購入イベント
		/// </summary>
		event EventHandler<EventArgs> OnCharaTicketPurchase;

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// 所持チケット枚数の設定
		/// </summary>
		/// <param name="num"></param>
		void SetTicketNum( int num );

		/// <summary>
		/// チケット購入上限の設定
		/// </summary>
		/// <param name="limit"></param>
		void SetTicketPurchaseLimit( int limit );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// アチーブメント制御
	/// </summary>
	public class Controller : IController {

		#region ==== 文字列 ====

		private string screenTitle = MasterData.GetText( TextType.TX494_CharaTicket_Title );
		private string helpMessage = MasterData.GetText( TextType.TX495_CharaTicket_HelpMessage );

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
			Model.TicketNum = 0;
			Model.TicketPurchaseLimit = 0;

			// イベント登録
			Model.OnChangeTicketNum				+= handleChangeTicketNum;
			Model.OnChangeTicketPurchaseLimit	+= handleChangeTicketPurchaseLimit;

			// ビュー設定
			_view = view;

			// イベント登録
			View.OnHome					+= HandleHome;
			View.OnClose				+= HandleClose;
			View.OnCharaExchange		+= handleCharaExchange;
			View.OnCharaTicketPurchase	+= handleCharaTicketPurchase;
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

			OnCharaExchange = null;
			OnCharaTicketPurchase = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( this.CanUpdate ) {
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
		/// 所持チケット枚数の設定
		/// </summary>
		/// <param name="num"></param>
		public void SetTicketNum( int num ) {

			Model.TicketNum = num;
		}

		/// <summary>
		/// チケット購入上限数の設定
		/// </summary>
		/// <param name="limit"></param>
		public void SetTicketPurchaseLimit( int limit ) {

			Model.TicketPurchaseLimit = limit;
		}
		
		/// <summary>
		/// キャラクター交換ボタンイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleCharaExchange( object sender, EventArgs args ) {

			OnCharaExchange( sender, args );
		}

		/// <summary>
		/// キャラクターチケット購入ボタンイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleCharaTicketPurchase( object sender, EventArgs args ) {

			OnCharaTicketPurchase( sender, args );
		}

		/// <summary>
		/// 所持チケット枚数が変化した時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeTicketNum( object sender, EventArgs args ) {

			View.ChangeTicket( Model.TicketPurchaseLimit, Model.TicketNum );
		}

		/// <summary>
		/// チケット購入上限が変化した時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeTicketPurchaseLimit( object sender, EventArgs args ) {

			View.ChangeTicket( Model.TicketPurchaseLimit, Model.TicketNum );
		}

		#endregion ==== アクション ====
	}
}