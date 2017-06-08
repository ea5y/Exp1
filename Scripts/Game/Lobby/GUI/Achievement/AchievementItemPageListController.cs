/// <summary>
/// アチーブメントアイテムページリスト制御
/// 
/// 2016/05/16
/// </summary>

using System;
using System.Collections.Generic;
using Scm.Common.Packet;
using UnityEngine;

namespace XUI.AchievementItemPageList {

	#region ==== イベント引数 ====

	/// <summary>
	/// ページ変更イベント引数
	/// </summary>
	public class PageChangeEventArgs : EventArgs {
		/// <summary>
		/// 変更後のページ
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// 変更後のページの最初のインデックス
		/// </summary>
		public int ItemIndex { get; set; }

		/// <summary>
		/// 件数
		/// </summary>
		public int ItemCount { get; set; }

		public PageChangeEventArgs( int page, int itemIndex, int itemCount ) {

			Page = page;
			ItemIndex = itemIndex;
			ItemCount = itemCount;
		}
	}

	#endregion ==== イベント引数 ====

	public interface IController {

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweenSkip"></param>
		void SetActive( bool isActive, bool isTweenSkip );

		#endregion ==== アクティブ ====

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

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler<PageChangeEventArgs> OnPageChange;

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// 指定ページにセット
		/// </summary>
		void SetPage( int page );

		/// <summary>
		/// 今のページを再度開く
		/// </summary>
		void ReopenPage();

		/// <summary>
		/// トータルのアイテム数をセット
		/// </summary>
		void SetTotalItemCount( int count );

		/// <summary>
		/// 表示するアイテムをセット
		/// </summary>
		void SetViewItem( List<AchievementInfo> infos );

		/// <summary>
		/// 現在のリストを更新
		/// </summary>
		void UpdateCurrentList();

		#endregion ==== アクション ====
	}

	public class Controller : IController {

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
		private bool CanUpdate {
			get {
				if( this.Model == null ) return false;
				if( this.View == null ) return false;
				return true;
			}
		}

		#endregion ==== プロパティ ====

		#region ==== コンストラクタ ====

		public Controller( IModel model, IView view ) {

			if( model == null || view == null ) return;

			// ビュー設定
			this._view = view;
			View.OnBackPage		+= HandleBackPage;
			View.OnBackEndPage	+= HandleBackEndPage;
			View.OnNextPage		+= HandleNextPage;
			View.OnNextEndPage	+= HandleNextEndPage;

			// モデル設定
			this._model = model;
			Model.OnPageChange							+= HandlePageChange;
			Model.OnCurrentAchievementInfoListChange	+= HandleCurrentAchievementInfoListChange;
		}

		#endregion ==== コンストラクタ ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweenSkip"></param>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( CanUpdate ) {
				View.SetActive( isActive, isTweenSkip );
			}
		}

		#endregion ==== アクティブ ====

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			Model.Setup();
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
		}

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler<PageChangeEventArgs> OnPageChange = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== ページイベント ====

		/// <summary>
		/// 戻る
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBackPage( object sender, EventArgs e ) {

			if( !CanUpdate ) return;
			if( !Model.PageButtonEnabele ) return;

			Model.BackPage();
		}

		/// <summary>
		/// 一番最初に戻る
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBackEndPage( object sender, EventArgs e ) {

			if( !CanUpdate ) return;
			if( !Model.PageButtonEnabele ) return;

			Model.BackEndPage();
		}

		/// <summary>
		/// 進む
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleNextPage( object sender, EventArgs e ) {

			if( !CanUpdate ) return;
			if( !Model.PageButtonEnabele ) return;

			Model.NextPage();
		}

		/// <summary>
		/// 一番最後に進む
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleNextEndPage( object sender, EventArgs e ) {

			if( !CanUpdate ) return;
			if( !Model.PageButtonEnabele ) return;

			Model.NextEndPage();
		}

		/// <summary>
		/// ページ変更したとき
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandlePageChange( object sender, EventArgs e ) {

			if( !CanUpdate ) return;

			var args = new PageChangeEventArgs(
				Model.AchievementItemScrollView.PageIndex,
				Model.AchievementItemScrollView.NowPageStartIndex,
				Model.AchievementItemScrollView.NowPageItemMax
			);

			OnPageChange( this, args );
		}

		#endregion ==== ページイベント ====

		#region ==== アクション ====

		/// <summary>
		/// ページ切り替えの有効を切り替える
		/// </summary>
		/// <param name="enable"></param>
		private void SetPageButtonEnable( bool enable ) {

			if( !CanUpdate ) return;

			Model.AchievementItemScrollView.SetPageButtonEnable( enable );
			Model.PageButtonEnabele = enable;
			View.SetActive( enable, false );
		}

		/// <summary>
		/// 指定ページにセット
		/// </summary>
		public void SetPage( int page ) {

			if( !CanUpdate ) return;

			Model.SetPage( page );
		}

		/// <summary>
		/// 今のページを再度開く
		/// </summary>
		public void ReopenPage() {

			if( !CanUpdate ) return;

			Model.SetPage( Model.CurrentPage );
		}

		/// <summary>
		/// トータルのアイテム数をセット
		/// </summary>
		/// <param name="count"></param>
		public void SetTotalItemCount( int count ) {

			if( !CanUpdate ) return;

			Model.SetTotalItemCount( count );
		}

		/// <summary>
		/// 表示アイテムをセット
		/// </summary>
		/// <param name="infos"></param>
		public void SetViewItem( List<AchievementInfo> infos ) {

			if( !CanUpdate ) return;

			Model.SetCurrentAchievementInfoList( infos );
		}


		/// <summary>
		/// 表示リストが更新時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCurrentAchievementInfoListChange( object sender, EventArgs e ) {

			UpdateItem();
		}

		/// <summary>
		/// リストのアイテムを更新する
		/// </summary>
		private void UpdateItem() {

			if( !CanUpdate ) return;

			// 全アイテムリスト
			var items = Model.GetAchievementItemList();

			int infoCount = Model.CurrentAchievementInfoList.Count;
			for( int i = 0 ; i < items.Count ; i++ ) {
				if( i < infoCount ) {
					items[i].SetAchievementInfo( Model.CurrentAchievementInfoList[i] );

				} else {
					items[i].SetAchievementInfo( null );
				}
			}

			SetPageButtonEnable( true );

			Model.Reposition();
		}

		/// <summary>
		/// 現在のリストを更新
		/// </summary>
		public void UpdateCurrentList() {

			if( !CanUpdate ) return;

			// 全アイテムリスト
			var items = Model.GetAchievementItemList();

			for( int i = 0 ; i < items.Count ; i++ ) {
				items[i].UpdateAchievementInfo();
			}
		}

		#endregion ==== アクション ====
	}
}
