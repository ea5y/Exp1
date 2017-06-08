using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailItemPageList
{

	public interface IView
	{
		#region === Property ===

		/// <summary>
		/// ページ付スクロールビューのデータ
		/// </summary>
		PageScrollViewAttach PageScrollViewAttach { get; }
		
		#endregion === Property ===

		#region === Event ===

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

		#endregion === Event ===

		
		#region === アクティブ ===

		/// <summary>
		/// アクティブ状態を取得する
		/// </summary>
		bool IsActive { get; }
		
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion === アクティブ ===
		
	}

	[DisallowMultipleComponent]
	public class MailItemPageListView : GUIViewBase, IView
	{
		#region === Field ===

		[SerializeField]
		private PageScrollViewAttach _pageScrollViewAttach = null;

		#endregion === Field ===

		#region === Property ===
		
		/// <summary>
		/// ページ付スクロールビューのデータ
		/// </summary>
		public PageScrollViewAttach PageScrollViewAttach { get { return _pageScrollViewAttach; } }
		
		#endregion === Property ===
		
		#region === Event ===

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
		
		#endregion === Event ===


		#region === アクティブ ===

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			this.SetRootActive(isActive, isTweenSkip);
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}

		#endregion === アクティブ ===

		/// <summary>
		/// オブジェクトが破棄されたとき
		/// </summary>
		private void OnDestroy()
		{
			OnNextPage = null;
			OnNextEndPage = null;
			OnBackPage = null;
			OnBackEndPage = null;
		}

		#region === NGUIリフレクション ===

		/// <summary>
		/// 次ボタンクリック時
		/// </summary>
		public void OnNextClick()
		{
			OnNextPage(this, EventArgs.Empty);
		}

		/// <summary>
		/// 最終ページボタンクリック時
		/// </summary>
		public void OnNextEndClick()
		{
			OnNextEndPage(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// 戻るボタンクリック時
		/// </summary>
		public void OnBackClick()
		{
			OnBackPage(this, EventArgs.Empty);
		}

		/// <summary>
		/// 最前ページボタンクリック時
		/// </summary>
		public void OnBackEndClick()
		{
			OnBackEndPage(this, EventArgs.Empty);
		}

		#endregion === NGUIリフレクション ===


	}
}

