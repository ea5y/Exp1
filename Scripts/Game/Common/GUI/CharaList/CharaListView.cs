/// <summary>
/// キャラリスト表示インターフェイス
/// 
/// 2016/01/12
/// </summary>
using UnityEngine;
using System;
using System.Collections;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// キャラリスト表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			void SetActive(bool isActive);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			GUIViewBase.ActiveState GetActiveState();

			/// <summary>
			/// ページ付スクロールビューのデータ
			/// </summary>
			PageScrollViewAttach PageScrollViewAttach { get; }
		}

		/// <summary>
		/// キャラリスト表示
		/// </summary>
		public class CharaListView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// ページ付スクロールビューのデータ
			/// </summary>
			[SerializeField]
			private PageScrollViewAttach _pageScrollViewAttach = null;
			public PageScrollViewAttach PageScrollViewAttach { get { return _pageScrollViewAttach; } }
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			public void SetActive(bool isActive)
			{
				this.SetRootActive(isActive, true);
			}

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion
		}
	}
}
