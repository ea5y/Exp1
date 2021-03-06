/// <summary>
/// キャラページ付表示
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
		/// ページ付表示インターフェイス
		/// </summary>
		public interface ICharaPageListView
		{
			/// <summary>
			/// 所有数セット
			/// </summary>
			/// <param name="capacityFormat"></param>
			void SetCapacity(string countFormat, string capacityFormat, StatusColor.Type colorType);

			/// <summary>
			/// 所持数追加の有効設定
			/// </summary>
			void SetAddCapacityEnable(bool isEnable);

			/// <summary>
			/// ソート項目名設定
			/// </summary>
			void SetSortTypeName(string typeName);

			/// <summary>
			/// 次のページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnNextPageClickEvent;

			/// <summary>
			/// 最後のページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnNextEndClickEvent;

			/// <summary>
			/// 戻るページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnBackPageClickEvent;

			/// <summary>
			/// 最初のページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnBackEndClickEvent;

			/// <summary>
			/// 所持数追加ボタンが押された時の通知用
			/// </summary>
			event EventHandler OnAddCapacityClickEvent;

			/// <summary>
			/// ソートボタンが押された時の通知用
			/// </summary>
			event EventHandler OnSortClickEvent;
		}

		/// <summary>
		/// ページ付表示
		/// </summary>
		public class CharaPageListView : CharaListView, ICharaPageListView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// 所有数ラベル
			/// </summary>
			[SerializeField]
			private UILabel _countLable = null;
			private UILabel CountLable { get { return _countLable; } }

			/// <summary>
			/// 所有数ラベルエフェクト用
			/// </summary>
			[SerializeField]
			private UILabel _countGlowLabel = null;
			private UILabel CountGlowLabel { get { return _countGlowLabel; } }

			/// <summary>
			/// 所有数最大値ラベル
			/// </summary>
			[SerializeField]
			private UILabel _capacityLable = null;
			private UILabel CapacityLable { get { return _capacityLable; } }

			/// <summary>
			/// 所持数追加ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _addCapacityButton = null;
			private XUIButton AddCapacityButton { get { return _addCapacityButton; } }

			/// <summary>
			/// ソートボタン
			/// </summary>
			[SerializeField]
			private UIButton _sortButton = null;
			private UIButton SortButton { get { return _sortButton; } }

			/// <summary>
			/// ソート種類名ラベル
			/// </summary>
			[SerializeField]
			private UILabel _sortTypeNameLabel = null;
			private UILabel SortTypeNameLabel { get { return _sortTypeNameLabel; } }

			/// <summary>
			/// 次のページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnNextPageClickEvent = (sender, e) => { };

			/// <summary>
			/// 最後のページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnNextEndClickEvent = (sender, e) => { };

			/// <summary>
			/// 戻るページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnBackPageClickEvent = (sender, e) => { };

			/// <summary>
			/// 最初のページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnBackEndClickEvent = (sender, e) => { };

			/// <summary>
			/// 所持数追加ボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnAddCapacityClickEvent = (sender, e) => { };

			/// <summary>
			/// ソートボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnSortClickEvent = (sender, e) => { };
			#endregion

			#region 初期化
			/// <summary>
			/// 初期化
			/// </summary>
			protected override void Awake()
			{
				base.Awake();

				if (this.SortButton != null) EventDelegate.Add(this.SortButton.onClick, this.HandleSortClick);
			}
			#endregion

			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnNextPageClickEvent = null;
				this.OnNextEndClickEvent = null;
				this.OnBackPageClickEvent = null;
				this.OnBackEndClickEvent = null;
				this.OnAddCapacityClickEvent = null;
				this.OnSortClickEvent = null;
			}
			#endregion

			#region 所有数
			/// <summary>
			/// 所有数セット
			/// </summary>
			/// <param name="capacityFormat"></param>
			public void SetCapacity(string countFormat, string capacityFormat, StatusColor.Type colorType)
			{
				if (this.CountLable != null)
				{
					this.CountLable.text = countFormat;
					// 色設定
					if(this.CountGlowLabel != null)
					{
						StatusColor.Set(colorType, this.CountLable, this.CountGlowLabel);
					}
				}
				if (this.CapacityLable != null)
				{
					this.CapacityLable.text = capacityFormat;
				}
			}
			#endregion

			#region 所持数追加
			/// <summary>
			/// 所持数追加の有効設定
			/// </summary>
			public void SetAddCapacityEnable(bool isEnable)
			{
				if (this.AddCapacityButton == null) { return; }
				this.AddCapacityButton.isEnabled = isEnable;
			}
			#endregion

			#region ソート
			/// <summary>
			/// ソートボタンが押された時のハンドラー
			/// </summary>
			private void HandleSortClick()
			{
				this.OnSortClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// ソート項目名設定
			/// </summary>
			public void SetSortTypeName(string typeName)
			{
				if (this.SortTypeNameLabel == null) { return; }
				this.SortTypeNameLabel.text = typeName;
			}
			#endregion

			#region NGUIリフレクション
			/// <summary>
			/// 次のページボタン
			/// </summary>
			public void OnNext()
			{
				this.OnNextPageClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 最後のページボタン
			/// </summary>
			public void OnNextEnd()
			{
				this.OnNextEndClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 戻るページボタン
			/// </summary>
			public void OnBack()
			{
				this.OnBackPageClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 最初のページボタン
			/// </summary>
			public void OnBackEnd()
			{
				this.OnBackEndClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 所持数追加ボタンが押された時
			/// </summary>
			public void OnAddCapacity()
			{
				this.OnAddCapacityClickEvent(this, EventArgs.Empty);
			}
			#endregion
		}
	}
}
