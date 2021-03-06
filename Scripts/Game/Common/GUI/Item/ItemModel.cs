/// <summary>
/// アイテムデータ
/// 
/// 2016/03/16
/// </summary>
using UnityEngine;
using System;

namespace XUI.Item
{
	/// <summary>
	/// アイテムデータインターフェイス
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		
		/// <summary>
		/// アイテム情報
		/// </summary>
		event EventHandler OnSetItemInfoChange;
		ItemInfo ItemInfo { get; set; }

		/// <summary>
		/// 選択フラグ
		/// </summary>
		event EventHandler OnSelectChange;
		bool IsSelect { get; set; }
	}

	/// <summary>
	/// アイテムデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnSetItemInfoChange = null;
			this.OnSelectChange = null;
		}
		#endregion

		#region ItemInfo
		public event EventHandler OnSetItemInfoChange = (sender, e) => { };
		private ItemInfo _itemInfo = null;
		public ItemInfo ItemInfo
		{
			get { return _itemInfo; }
			set
			{
				if(value == null)
				{
					if(_itemInfo != value)
					{
						_itemInfo = value;

						// 変更通知
						this.OnSetItemInfoChange(this, EventArgs.Empty);
					}
				}
				else
				{
					if(_itemInfo == null)
					{
						// 保持しているアイテム情報がNULLなのでセット
						_itemInfo = value;

						// 変更通知
						this.OnSetItemInfoChange(this, EventArgs.Empty);
					}
					else if(CanItemInfoChange(value))
					{
						// アイテム情報の中身に変更があったのでセット
						_itemInfo = value;

						// 変更通知
						this.OnSetItemInfoChange(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// アイテム情報に変化があったかどうか
		/// </summary>
		private bool CanItemInfoChange(ItemInfo itemInfo)
		{
			if(this._itemInfo.Equals(itemInfo))
			{
				return true;
			}

			return false;
		}
		#endregion

		#region 選択フラグ
		/// <summary>
		/// 選択フラグ
		/// </summary>
		public event EventHandler OnSelectChange = (sender, e) => { };
		private bool _isSelect = false;
		public bool IsSelect
		{
			get { return _isSelect; }
			set
			{
				if (_isSelect != value)
				{
					_isSelect = value;

					// 通知
					this.OnSelectChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}
