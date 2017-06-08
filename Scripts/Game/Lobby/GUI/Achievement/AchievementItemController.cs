/// <summary>
/// メールアイテム制御
/// 
/// 2016/05/16
/// </summary>

using System;

namespace XUI.AchievementItem {

	#region ==== イベント ====

	/// <summary>
	/// リワード取得イベント
	/// </summary>
	public class GetRewardEventArgs : EventArgs {

		public int AchieveMasterID { get; set; }
	}

	#endregion ==== イベント ====

	public interface IController {

		#region ==== イベント ====

		/// <summary>
		/// 取得ボタンが押下された時
		/// </summary>
		event EventHandler<GetRewardEventArgs> OnGetButtonClick;

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
		/// アチーブメント情報のセット
		/// </summary>
		/// <param name="param"></param>
		void SetAchievementInfo( AchievementInfo info );

		/// <summary>
		/// アチーブの同期
		/// </summary>
		void UpdateAchievementInfo();

		#endregion ==== アクション ====
	}

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
		/// 取得ボタンが押下された時
		/// </summary>
		public event EventHandler<GetRewardEventArgs> OnGetButtonClick = ( sender, e ) => { };

		/// <summary>
		/// 取得ボタンが押下された時
		/// </summary>
		private void HandleOnGetButtonClick( object sender, EventArgs e ) {

			// 取得するアチーブのIDを設定
			GetRewardEventArgs args = new GetRewardEventArgs() {
				AchieveMasterID = Model.Info.MasterID
			};

			OnGetButtonClick( sender, args );
		}

		/// <summary>
		/// アチーブメント情報が更新された時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handleAchievementInfoChange( object sender, EventArgs e ) {

			SyncAchievementInfo();
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
			View.OnGetButtonClick += HandleOnGetButtonClick;

			// モデル設定
			this.model = model;
			Model.OnAchievementInfoChange += handleAchievementInfoChange;
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

			OnGetButtonClick = null;
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
		/// アチーブ情報のセット
		/// </summary>
		/// <param name="info"></param>
		public void SetAchievementInfo( AchievementInfo info ) {

			if( !CanUpdate ) return;

			Model.Info = info;
		}

		/// <summary>
		/// アチーブの同期
		/// </summary>
		public void UpdateAchievementInfo() {

			SyncAchievementInfo();
		}

		/// <summary>
		/// データをビューに反映
		/// </summary>
		private void SyncAchievementInfo() {

			if( !CanUpdate ) return;
			if( Model.Info == null ) return;

			View.SetAchievementInfo( Model.Info );
		}

		#endregion ==== アクション ====
	}
}
