/// <summary>
/// キャラクターショップアイテムページリストビュー
/// 
/// 2016/06/16
/// </summary>

using System;
using UnityEngine;

namespace XUI.CharaShopItemPageList {

	/// <summary>
	/// キャラクターショップアイテムページリストビューインターフェイス
	/// </summary>
	public interface IView {

		#region ==== プロパティ ====

		/// <summary>
		/// ページ付スクロールビューのデータ
		/// </summary>
		PageScrollViewAttach PageScrollViewAttach { get; }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// 次のページボタンが押された時の通知用
		/// </summary>
		event EventHandler OnNextPage;

		/// <summary>
		/// 最後のページボタンが押された時の通知用
		/// </summary>
		event EventHandler OnNextEndPage;

		/// <summary>
		/// 戻るページボタンが押された時の通知用
		/// </summary>
		event EventHandler OnBackPage;

		/// <summary>
		/// 最初のページボタンが押された時の通知用
		/// </summary>
		event EventHandler OnBackEndPage;

		#endregion ==== イベント ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態を取得する
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion ==== アクティブ ====	}
	}

	/// <summary>
	/// キャラクターショップアイテムページリストビュー
	/// </summary>
	public class CharaShopItemPageListView : GUIViewBase, IView {

		#region ==== フィールド ====

		[SerializeField]
		private PageScrollViewAttach _pageScrollViewAttach = null;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// ページ付スクロールビューのデータ
		/// </summary>
		public PageScrollViewAttach PageScrollViewAttach { get { return _pageScrollViewAttach; } }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// 次のページボタンが押された時の通知用
		/// </summary>
		public event EventHandler OnNextPage = (sender, e) => { };

		/// <summary>
		/// 最後のページボタンが押された時の通知用
		/// </summary>
		public event EventHandler OnNextEndPage = (sender, e) => { };

		/// <summary>
		/// 戻るページボタンが押された時の通知用
		/// </summary>
		public event EventHandler OnBackPage = (sender, e) => { };

		/// <summary>
		/// 最初のページボタンが押された時の通知用
		/// </summary>
		public event EventHandler OnBackEndPage = (sender, e) => { };

		#endregion ==== イベント ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			SetRootActive( isActive, isTweenSkip );
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public ActiveState GetActiveState() {

			return GetRootActiveState();
		}

		#endregion === アクティブ ===

		#region ==== 破棄 ====

		/// <summary>
		/// オブジェクトが破棄されたとき
		/// </summary>
		private void OnDestroy() {

			OnNextPage = null;
			OnNextEndPage = null;
			OnBackPage = null;
			OnBackEndPage = null;
		}
		#endregion ==== 破棄 ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// 次ボタンクリック時
		/// </summary>
		public void OnNextClick() {
			OnNextPage( this, EventArgs.Empty );
		}

		/// <summary>
		/// 最終ページボタンクリック時
		/// </summary>
		public void OnNextEndClick() {
			OnNextEndPage( this, EventArgs.Empty );
		}

		/// <summary>
		/// 戻るボタンクリック時
		/// </summary>
		public void OnBackClick() {
			OnBackPage( this, EventArgs.Empty );
		}

		/// <summary>
		/// 最前ページボタンクリック時
		/// </summary>
		public void OnBackEndClick() {
			OnBackEndPage( this, EventArgs.Empty );
		}

		#endregion === NGUIリフレクション ===
	}
}