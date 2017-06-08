/// <summary>
/// 年齢認証制御
/// 
/// 2016/06/27
/// </summary>

using System;

namespace XUI.AgeVerification {

	#region ==== イベント ====

	/// <summary>
	/// トグルイベント
	/// </summary>
	public class ToggleEventArgs : EventArgs {

		/// <summary>
		/// トグルのチェック状態
		/// </summary>
		public bool Toggle { get; set; }
	}

	#endregion === イベント ====

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
	}

	/// <summary>
	/// コントローラ
	/// </summary>
	public class Controller : IController {

		#region ==== 文字列 ====

		private static string screenTitle = MasterData.GetText( TextType.TX540_AgeVerification_Title );
		private static string helpMessage = MasterData.GetText( TextType.TX541_AgeVerification_HelpMessage );

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

			// モデル設定
			_model = model;

			// ビュー設定
			_view = view;
			View.OnHome += HandleHome;
			View.OnClose += HandleClose;
			View.OnOpenShop += handleOpenShop;
			View.OnSkipAgeVerification += handleSkipAgeVerification;

			// 初期化
			MemberInit();
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
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( !this.CanUpdate )	return;

			// アクティブ設定
			this.View.SetActive( isActive, isTweenSkip );

			// その他UIの表示設定
			GUILobbyResident.SetActive( !isActive );
			GUIScreenTitle.Play( isActive, screenTitle );
			GUIHelpMessage.Play( isActive, helpMessage );
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

		#region ==== アクション ====

		/// <summary>
		/// ショップを開く
		/// </summary>
		private void handleOpenShop( object sender, EventArgs args ) {

			// スキップフラグを保存する
			if( Model.SkipFlag ) {
				ConfigFile.Instance.Data.System.AgeVerificationSkip = Model.SkipFlag;
				ConfigFile.Instance.Write();
			}

			// ショップを開く
			GUIController.Open( new GUIScreen( GUICharaTicket.Open, GUICharaTicket.Close, GUICharaTicket.ReOpen ) );
		}

		/// <summary>
		/// スキップ設定
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void handleSkipAgeVerification( object sender, ToggleEventArgs args ) {

			// チェック状態を設定
			Model.SkipFlag = args.Toggle;
		}

		#endregion ==== アクション ====
	}
}
