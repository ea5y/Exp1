/// <summary>
/// 選択キャラリスト表示
/// 
/// 2016/01/14
/// </summary>
using UnityEngine;
using System;
using System.Collections;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// 選択キャラリスト表示インターフェイス
		/// </summary>
		public interface ISelectCharaListView
		{
			/// <summary>
			/// 選択外すボタンが押された時の通知用
			/// </summary>
			event EventHandler OnAllClearClickEvent;
		}

		/// <summary>
		/// 選択キャラリスト表示
		/// </summary>
		public class SelectCharaListView : CharaListView, ISelectCharaListView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// 選択外すボタン
			/// </summary>
			[SerializeField]
			private UIButton _clearButton = null;
			public UIButton ClearButton { get { return _clearButton; } }

			/// <summary>
			/// 選択外すボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnAllClearClickEvent = (sender, e) => { };
			#endregion

			#region NGUIリフレクション
			/// <summary>
			/// 選択を外すボタンが押された時
			/// </summary>
			public void OnCancelClick()
			{
				// 通知
				this.OnAllClearClickEvent(this, EventArgs.Empty);
			}
			#endregion
		}
	}
}