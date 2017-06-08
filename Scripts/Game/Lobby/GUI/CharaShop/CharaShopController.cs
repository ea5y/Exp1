/// <summary>
/// キャラクターショップ制御
/// 
/// 2016/06/15
/// </summary>

using System;
using System.Collections.Generic;
using XUI.CharaShopItemPageList;

namespace XUI.CharaShop {

	/// <summary>
	/// インターフェイス
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

		#region ==== リスト更新 ====

		/// <summary>
		/// リスト更新
		/// </summary>
		/// <param name="list"></param>
		void UpdateList( List<CharaShopItemInfo> list );

		#endregion ==== リスト更新 ====
	}

	/// <summary>
	/// コントローラ
	/// </summary>
	public class Controller : IController {

		#region ==== 文字列 ====

		private string screenTitle { get { return MasterData.GetText( TextType.TX521_CharaShop_Title ); } }
		private string helpMessage { get { return MasterData.GetText( TextType.TX522_CharaShop_HelpMessage ); } }

		#endregion ==== 文字列 ====

		#region ==== フィールド ====

		// モデル
		private readonly IModel _model;

		// ビュー
		private readonly IView _view;

		// リスト
		private GUICharaShopItemPageList charaShopItemPageList;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		// モデル
		private IModel Model { get { return _model; } }

		// ビュー
		private IView View { get { return _view; } }

		// リスト
		private GUICharaShopItemPageList CharaShopItemPageList { get { return charaShopItemPageList; } }

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
		public Controller( IModel model, IView view, GUICharaShopItemPageList pageList ) {

			if( model == null || view == null || pageList == null ) return;

			// 初期化
			MemberInit();

			// モデル設定
			_model = model;
			Model.OnChangeInfoList += handleChangeInfoList;
			Model.OnChangeTicketNum	+= handleChangeTicketNum;

			// ビュー設定
			_view = view;
			View.OnHome		+= HandleHome;
			View.OnClose	+= HandleClose;

			// ページリスト
			charaShopItemPageList = pageList;
			CharaShopItemPageList.OnPageChange += handlePageChange;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			if( !CanUpdate ) return;

			// シリアライズされていないメンバーの初期化
			MemberInit();

			// セットアップ
			CharaShopItemPageList.Setup();
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
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( this.CanUpdate ) {
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

		#region ==== リスト更新 ====

		/// <summary>
		/// リスト更新
		/// </summary>
		/// <param name="infos"></param>
		public void UpdateList( List<CharaShopItemInfo> infos ) {

			// 一覧の退避
			Model.InfoList = infos;
		}

		#endregion

		#region ==== アクション ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handlePageChange( object sender, PageChangeEventArgs e ) {

			// ページ変更
			CharaShopItemPageList.SetViewCharaShopItemInfoList( Model.GetTabList( e.ItemIndex, e.ItemCount ) );
		}

		/// <summary>
		/// リスト更新イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeInfoList( object sender, EventArgs args ) {

			CharaShopItemPageList.SetViewCharaShopItemInfoList( Model.GetTabList( 0, 20 ) );
		}

		/// <summary>
		/// 所持チケット枚数変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleChangeTicketNum( object sender, EventArgs args ) {

			View.SetTicketNum( Model.TicketNum );
		}

		#endregion ==== アクション ====
	}
}